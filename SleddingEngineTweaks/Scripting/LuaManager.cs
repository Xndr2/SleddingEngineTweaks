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
            // Register the assembly containing all Unity types
            UserData.RegisterAssembly();
            
            UserData.RegisterType<Vector3>();
            
            _luaScript = new Script();
            
            // Register Unity types
            // Transform and GameObject stuff
            UserData.RegisterType<GameObject>();
            UserData.RegisterType<Transform>();
            UserData.RegisterType<Component>();
            UserData.RegisterType<MonoBehaviour>();


            // Vector types
            UserData.RegisterType<Vector2>();
            UserData.RegisterType<Vector4>();
            UserData.RegisterType<Quaternion>();
            
            // Physics
            UserData.RegisterType<Rigidbody>();
            UserData.RegisterType<Collider>();
            UserData.RegisterType<Physics>();
            
            // Rendering
            UserData.RegisterType<Camera>();
            UserData.RegisterType<Material>();
            UserData.RegisterType<MeshRenderer>();
            UserData.RegisterType<Renderer>();
            
            // Scene management
            UserData.RegisterType<SceneManager>();
            UserData.RegisterType<Scene>();
            
            // UI
            UserData.RegisterType<Canvas>();
            UserData.RegisterType<RectTransform>();
            
            // utils
            UserData.RegisterType<Time>();
            UserData.RegisterType<Mathf>();
            UserData.RegisterType<Debug>();
            UserData.RegisterType<Color>();
            
            // custom
            UserData.RegisterType<GameAPI>();
            UserData.RegisterType<SleddingAPIStatus>();
            UserData.RegisterType<OptionType>();
            
            // initialize standard lua env
            _luaScript.Globals["debug"] = new Action<string>(Debug.Log);
            _luaScript.Globals["Time"] = UserData.CreateStatic<Time>();
            _luaScript.Globals["Mathf"] = UserData.CreateStatic<Mathf>();
            _luaScript.Globals["Vector3"] = new Func<float, float, float, Vector3>((x, y, z) => new Vector3(x, y, z));
            _luaScript.Globals["Color"] = UserData.CreateStatic<Color>();

            
            _luaScript.Globals["Vector3_zero"] = Vector3.zero;
            _luaScript.Globals["Vector3_one"] = Vector3.one;
            _luaScript.Globals["Vector3_up"] = Vector3.up;
            _luaScript.Globals["Vector3_down"] = Vector3.down;
            _luaScript.Globals["Vector3_left"] = Vector3.left;
            _luaScript.Globals["Vector3_right"] = Vector3.right;
            _luaScript.Globals["Vector3_forward"] = Vector3.forward;
            _luaScript.Globals["Vector3_back"] = Vector3.back;

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

    }
}