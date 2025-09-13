using MoonSharp.Interpreter;
using System;
using System.IO;
using UnityEngine;
using BepInEx;
using SleddingEngineTweaks.API;
using SleddingEngineTweaks.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Linq;

namespace SleddingEngineTweaks.Scripting
{
    public class LuaManager : IDisposable
    {
        private static LuaManager _instance;
        private bool _disposed = false;
        private bool _isReloading = false;

        public static LuaManager Instance
        {
            get
            {
                if (_instance == null || _instance._disposed)
                    _instance = new LuaManager();
                return _instance;
            }
        }
        
        private Script _luaScript;
        private string _scriptPath;
        public event Action<string> OnScriptOutput;
        private FileSystemWatcher _scriptWatcher;
        private SynchronizationContext _mainThreadContext;

        public LuaManager()
        {
            // Get the path to the scripts directory
            _scriptPath = Path.Combine(Paths.PluginPath, "SleddingEngineTweaks", "Scripts");
            
            // create scripts dir
            if (!Directory.Exists(_scriptPath))
            {
                Directory.CreateDirectory(_scriptPath);
            }
            
            InitializeLua();
            _mainThreadContext = SynchronizationContext.Current;
            SetupFileWatcher();
        }

        private void InitializeLua()
        {
            // Create a hard sandboxed Lua VM: disables IO/OS/DLL access
            // Only explicitly whitelisted CLR types/methods will be available
            _luaScript = new Script(CoreModules.Preset_HardSandbox | CoreModules.TableIterators | CoreModules.Basic);

            // register core types and create a standard environment for Lua scripts
            SetupStandardLuaEnvironment();
        }

        /// <summary>
        /// Registers essential Unity and custom types with the Lua env
        /// </summary>
        private void SetupStandardLuaEnvironment()
        {
            // SECURITY: Whitelist only the types we want to expose to Lua
            // Do NOT broadly register assemblies.
            UserData.RegisterType<GameAPI>();
            UserData.RegisterType<SleddingAPI>();
            UserData.RegisterType<PrefabAPI>();
            UserData.RegisterType<SleddingAPIStatus>();
            UserData.RegisterType<OptionType>();
            UserData.RegisterType<Vector3>();
            UserData.RegisterType<Quaternion>();
            UserData.RegisterType<Color>();
            UserData.RegisterType<SafeGameObject>();
            UserData.RegisterType<RaycastResult>();

            // Expose safe static utility classes/values to Lua (no IO/OS)
            _luaScript.Globals["Time"] = UserData.CreateStatic<Time>();
            _luaScript.Globals["Mathf"] = UserData.CreateStatic<Mathf>();
            _luaScript.Globals["Color"] = UserData.CreateStatic<Color>();
            
            // Provide a constructor for Vector3. Lua scripts can use it like:
            // local myVector = Vector3(1, 2, 3)
            // Note: Static properties of Vector3 (e.g., Vector3.zero) are automatically
            // exposed to Lua scripts
            _luaScript.Globals["Vector3"] = new Func<float, float, float, Vector3>((x, y, z) => new Vector3(x, y, z));
            
            // Provide a constructor for Quaternion. Lua scripts can use it like:
            // local myQuat = Quaternion(x, y, z, w) or Quaternion.Euler(x, y, z)
            _luaScript.Globals["Quaternion"] = new Func<float, float, float, float, Quaternion>((x, y, z, w) => new Quaternion(x, y, z, w));
            
            // Define basic global functions
            _luaScript.Globals["log"] = new Action<DynValue>((message) =>
            {
                OutputMessage(message.ToString());
            });
            
            _luaScript.Globals["help"] = new Action(() =>
            {
                OutputMessage("Available commands:");
                OutputMessage("help() - Shows this help message");
                OutputMessage("log(message) - Logs a message to the console");
                // TODO: add more
            });
        }

        public DynValue ExecuteScript(string luaCode)
        {
            try
            {
                return _luaScript.DoString(luaCode);
            }
            catch (ScriptRuntimeException e)
            {
                Plugin.StaticLogger.LogError($"Lua Runtime Error: {e.Message}");
                return DynValue.Nil;
            }
            catch (Exception e)
            {
                Plugin.StaticLogger.LogError($"Lua Error: {e.Message}");
                return DynValue.Nil;
            }
        }

        public DynValue ExecuteFile(string fileName)
        {
            try
            {
                // SECURITY: Validate filename to prevent path traversal
                if (string.IsNullOrEmpty(fileName) || fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    Plugin.StaticLogger.LogError($"Invalid filename: {fileName}");
                    return DynValue.Nil;
                }

                // SECURITY: Only allow .lua files
                if (!fileName.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
                {
                    Plugin.StaticLogger.LogError($"Only .lua files are allowed: {fileName}");
                    return DynValue.Nil;
                }

                string filePath = Path.Combine(_scriptPath, fileName);
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"Lua script file not found: {filePath}");
                    return DynValue.Nil;
                }
                
                // SECURITY: Check file size to prevent memory exhaustion
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 1024 * 1024) // 1MB limit
                {
                    Plugin.StaticLogger.LogError($"Script file too large: {fileName} ({fileInfo.Length} bytes)");
                    return DynValue.Nil;
                }
                
                string script = File.ReadAllText(filePath);
                return ExecuteScript(script);
            }
            catch (Exception e)
            {
                Plugin.StaticLogger.LogError($"Error loading Lua file {fileName}: {e.Message}");
                return DynValue.Nil;
            }
        }
        
        public void RegisterGlobal<T>(string name, T obj)
        {
            // SECURITY: Validate global name to prevent overwriting critical functions
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Global name cannot be null or empty", nameof(name));
            }

            // SECURITY: Prevent overwriting critical Lua functions
            // Note: "game", "set", "prefab" are allowed to be overwritten during Plugin initialization
            string[] protectedNames = { "log", "help", "Vector3", "Quaternion", "Time", "Mathf", "Color" };
            if (protectedNames.Contains(name))
            {
                Plugin.StaticLogger.LogWarning($"Attempted to overwrite protected global: {name}");
                return;
            }

            _luaScript.Globals[name] = obj;
        }
        
        public void LoadAllScripts()
        {
            try
            {
                string[] scriptFiles = Directory.GetFiles(_scriptPath, "*.lua");
                int loaded = 0;
                foreach (string scriptFile in scriptFiles)
                {
                    ExecuteFile(Path.GetFileName(scriptFile));
                    loaded++;
                }
                OutputMessage($"Loaded {loaded} Lua script(s) from '{_scriptPath}'.");
            }
            catch (Exception e)
            {
                Plugin.StaticLogger.LogError($"Error loading Lua scripts: {e.Message}");
            }
        }
        
        public void CallLuaFunction(string functionName, params object[] args)
        {
            try
            {
                if (_luaScript.Globals[functionName] != null)
                {
                    _luaScript.Call(_luaScript.Globals[functionName], args);
                }
            }
            catch (ScriptRuntimeException e)
            {
                Plugin.StaticLogger.LogError($"Error calling Lua function {functionName}: {e.Message}");
            }
        }
        
        // TODO: Fix this
        // typing help does not work!
        // Error: unexpected symbol near '<eof>'
        public DynValue ExecuteCommand(string command)
        {
            try
            {
                // we first try to interpret the command as a function call if it matches a global function.
                // this allows users to type 'help' instead of 'help()'
                string trimmedCommand = command.Trim();
                if (string.IsNullOrWhiteSpace(trimmedCommand))
                {
                    return DynValue.Nil;
                }
                DynValue potentialFunc = _luaScript.Globals.Get(trimmedCommand);

                if (potentialFunc != null && potentialFunc.Type == DataType.Function)
                {
                    // it's a function. Let's call it and return
                    _luaScript.Call(potentialFunc);
                    return DynValue.Nil; // we've handled it
                }

                // if it's not a simple function name, execute it as a script
                // this will handle 'help()', 'a = 5', '2+2', etc
                DynValue result = _luaScript.DoString(command);

                // output the result if it's not nil and not from a print statement
                if (result != null && !result.IsNil() && !trimmedCommand.StartsWith("print"))
                {
                    OutputMessage($"=> {result}");
                }
                return result;
            }
            catch (Exception e)
            {
                OutputMessage($"Error: {e.Message} | {command}");
                return DynValue.Nil;
            }
        }
        
        public void OutputMessage(string message)
        {
            OnScriptOutput?.Invoke(message);
            Plugin.StaticLogger.LogInfo(message);
        }

        private void SetupFileWatcher()
        {
            try
            {
                _scriptWatcher = new FileSystemWatcher(_scriptPath, "*.lua");
                _scriptWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
                _scriptWatcher.Changed += OnScriptFolderChanged;
                _scriptWatcher.Created += OnScriptFolderChanged;
                _scriptWatcher.Deleted += OnScriptFolderChanged;
                _scriptWatcher.Renamed += OnScriptFolderChanged;
                _scriptWatcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                Plugin.StaticLogger.LogError($"Failed to setup file watcher: {e.Message}");
            }
        }

        // suggested by @Coderabbitai in #8
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        // suggested by @Coderabbitai in #8
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _scriptWatcher?.Dispose();
                    _scriptWatcher = null;
                    OnScriptOutput = null;
                    _luaScript = null;
                }
                _disposed = true;
            }
        }

        private DateTime _lastChangeEvent = DateTime.MinValue;
        private void OnScriptFolderChanged(object sender, FileSystemEventArgs e)
        {
            // Debounce rapid file system events
            var now = DateTime.UtcNow;
            if ((now - _lastChangeEvent).TotalMilliseconds < 250) return;
            _lastChangeEvent = now;

            // Ensure reload happens on main thread
            if (_mainThreadContext != null)
            {
                _mainThreadContext.Post(_ => ReloadAllScripts(), null);
            }
            else
            {
                ReloadAllScripts();
            }
        }

        public void ReloadAllScripts()
        {
            if (_isReloading)
            {
                OutputMessage("Reload already in progress, skipping...");
                return;
            }

            try
            {
                _isReloading = true;
                OutputMessage("Reloading Lua scripts...");

                // Just reload the scripts without destroying everything
                LoadAllScripts();
                
                OutputMessage("Lua scripts reloaded successfully.");
            }
            catch (Exception e)
            {
                Plugin.StaticLogger.LogError($"Failed to reload scripts: {e.Message}");
                OutputMessage($"Error during reload: {e.Message}");
            }
            finally
            {
                _isReloading = false;
            }
        }
    }
}