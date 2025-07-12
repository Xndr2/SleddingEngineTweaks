using MoonSharp.Interpreter;
using System;
using System.IO;
using UnityEngine;
using BepInEx;
using SleddingEngineTweaks.API;
using SleddingEngineTweaks.UI;
using UnityEngine.SceneManagement;

namespace SleddingEngineTweaks.Scripting
{
    public class LuaManager
    {
        private static LuaManager _instance;

        public static LuaManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LuaManager();
                return _instance;
            }
        }
        
        private Script _luaScript;
        private string _scriptPath;
        public event Action<string> OnScriptOutput;

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
        }

        private void InitializeLua()
        {
            _luaScript = new Script();
            
            // register core types and create a standard environment for Lua scripts
            SetupStandardLuaEnvironment();
        }

        /// <summary>
        /// Registers essential Unity and custom types with the Lua env
        /// </summary>
        private void SetupStandardLuaEnvironment()
        {
            // registers all public types in the Unity assemblies
            // it makes most of the Unity API available
            UserData.RegisterAssembly();

            // Note: After calling RegisterAssembly, explicitly registering types like
            // GameObject, Transform, Vector3, etc., is redundant. MoonSharp handles this
            // We only need to keep custom type registrations
            UserData.RegisterType<GameAPI>();
            UserData.RegisterType<SleddingAPI>();
            UserData.RegisterType<SleddingAPIStatus>();
            UserData.RegisterType<OptionType>();

            // Expose static utility classes to Lua
            _luaScript.Globals["debug"] = new Action<string>(Debug.Log);
            _luaScript.Globals["Time"] = UserData.CreateStatic<Time>();
            _luaScript.Globals["Mathf"] = UserData.CreateStatic<Mathf>();
            _luaScript.Globals["Color"] = UserData.CreateStatic<Color>();
            
            // Provide a constructor for Vector3. Lua scripts can use it like:
            // local myVector = Vector3(1, 2, 3)
            // Note: Static properties of Vector3 (e.g., Vector3.zero) are automatically
            // exposed to Lua scripts
            _luaScript.Globals["Vector3"] = new Func<float, float, float, Vector3>((x, y, z) => new Vector3(x, y, z));
            
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
        }

        public DynValue ExecuteFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_scriptPath, fileName);
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"Lua script file not found: {filePath}");
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
            _luaScript.Globals[name] = obj;
        }
        
        public void LoadAllScripts()
        {
            try
            {
                string[] scriptFiles = Directory.GetFiles(_scriptPath, "*.lua");
                foreach (string scriptFile in scriptFiles)
                {
                    Plugin.StaticLogger.LogInfo($"Loading Lua script: {Path.GetFileName(scriptFile)}");
                    ExecuteFile(Path.GetFileName(scriptFile));
                }
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

    }
}