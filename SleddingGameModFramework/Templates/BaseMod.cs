using System;
using System.Reflection;
using HarmonyLib;
using SleddingGameModFramework.API;
using SleddingGameModFramework.Configuration;

namespace SleddingGameModFramework.Templates
{
    /// <summary>
    /// Base class for mods to extend
    /// </summary>
    public abstract class BaseMod : IMod, IModEvents
    {
        /// <summary>
        /// Unique identifier for the mod
        /// </summary>
        public abstract string Id { get; }
        
        /// <summary>
        /// Display name of the mod
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// Mod author name
        /// </summary>
        public abstract string Author { get; }
        
        /// <summary>
        /// Mod version
        /// </summary>
        public abstract Version Version { get; }
        
        /// <summary>
        /// Reference to the mod framework
        /// </summary>
        protected ModFramework Framework => ModFramework.Instance;
        
        /// <summary>
        /// The Harmony instance for this mod
        /// </summary>
        protected Harmony HarmonyInstance => Framework.PatchManager.GetHarmonyInstance(Id);
        
        /// <summary>
        /// Called when the mod is being initialized
        /// </summary>
        public virtual void Initialize()
        {
            // Default implementation does nothing
        }
        
        /// <summary>
        /// Called when the mod is being unloaded
        /// </summary>
        public virtual void Shutdown()
        {
            // Remove all patches when the mod is unloaded
            Framework.PatchManager.RemovePatches(Id);
        }
        
        /// <summary>
        /// Called when the game starts loading
        /// </summary>
        public virtual void OnGameStartLoading()
        {
            // Default implementation does nothing
        }
        
        /// <summary>
        /// Called when the game has finished loading
        /// </summary>
        public virtual void OnGameLoaded()
        {
            // Default implementation does nothing
        }
        
        /// <summary>
        /// Called each game frame
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds</param>
        public virtual void OnUpdate(float deltaTime)
        {
            // Default implementation does nothing
        }
        
        /// <summary>
        /// Called when the game is about to be unloaded
        /// </summary>
        public virtual void OnGameUnloading()
        {
            // Default implementation does nothing
        }
        
        /// <summary>
        /// Register an event handler
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="handler">Handler function</param>
        protected void RegisterEventHandler(string eventName, Action<object[]> handler)
        {
            Framework.EventManager.RegisterHandler(eventName, handler);
        }
        
        /// <summary>
        /// Trigger an event with the specified arguments
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="args">Event arguments</param>
        protected void TriggerEvent(string eventName, params object[] args)
        {
            Framework.EventManager.TriggerEvent(eventName, args);
        }
        
        /// <summary>
        /// Creates a typed config manager for this mod
        /// </summary>
        /// <typeparam name="T">Config class type</typeparam>
        /// <param name="configName">Optional config name</param>
        /// <returns>Config manager instance</returns>
        protected ConfigManager<T> CreateConfig<T>(string configName = "config") where T : class, new()
        {
            return new ConfigManager<T>(Id, configName);
        }
        
        /// <summary>
        /// Applies all Harmony patches defined in the mod's assembly
        /// </summary>
        protected void ApplyHarmonyPatches()
        {
            Framework.PatchManager.ApplyPatches(Id, GetType().Assembly);
        }
        
        /// <summary>
        /// Creates a manual Harmony patch
        /// </summary>
        /// <param name="originalMethod">Method to patch</param>
        /// <param name="prefix">Optional prefix method</param>
        /// <param name="postfix">Optional postfix method</param>
        /// <param name="transpiler">Optional transpiler method</param>
        /// <param name="finalizer">Optional finalizer method</param>
        protected void CreatePatch(
            MethodInfo originalMethod,
            MethodInfo prefix = null,
            MethodInfo postfix = null,
            MethodInfo transpiler = null,
            MethodInfo finalizer = null)
        {
            Framework.PatchManager.CreatePatch(Id, originalMethod, prefix, postfix, transpiler, finalizer);
        }
    }
}