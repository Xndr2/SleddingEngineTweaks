using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SleddingGameModFramework.Utils;

namespace SleddingGameModFramework.Patches
{
    /// <summary>
    /// Manages Harmony patches for the game
    /// </summary>

    public class PatchManager
    {
        private readonly Dictionary<string, Harmony> _harmonyInstances = new();
        private bool _initialized = false;

        /// <summary>
        /// Initializes the patch manager
        /// </summary>
        public void Initialize()
        {
            if (_initialized) return;
            
            Logger.Info("Initializing Patch Manager");
            
            _initialized = true;
        }

        /// <summary>
        /// Creates a new Harmony instance for the specified mod
        /// </summary>
        /// <param name="modId">Unique ID of the mod</param>
        /// <returns>Harmony instance</returns>
        public Harmony GetHarmonyInstance(string modId)
        {
            if (string.IsNullOrEmpty(modId))
                throw new ArgumentNullException(nameof(modId));
            
            if (_harmonyInstances.TryGetValue(modId, out var instance))
                return instance;
            
            instance = new Harmony($"sleddingame.mod.{modId}");
            _harmonyInstances[modId] = instance;
            
            Logger.Debug($"Created Harmony instance for mod {modId}");
            return instance;
        }
        
        /// <summary>
        /// Applies all patches defined in the specified assembly
        /// </summary>
        /// <param name="modId">ID of the mod</param>
        /// <param name="assembly">Assembly containing the patches</param>
        public void ApplyPatches(string modId, Assembly assembly)
        {
            if (!_initialized)
                throw new InvalidOperationException("PatchManager is not initialized");
            
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            
            try
            {
                Logger.Info($"Applying patches for mod {modId} from assembly {assembly.GetName().Name}");
                Harmony harmony = GetHarmonyInstance(modId);
                harmony.PatchAll(assembly);
                Logger.Info($"Successfully applied patches for mod {modId}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to apply patches for mod {modId}: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Manually creates and applies a patch
        /// </summary>
        /// <param name="modId">ID of the mod</param>
        /// <param name="originalMethod">Method to patch</param>
        /// <param name="prefix">Optional prefix method</param>
        /// <param name="postfix">Optional postfix method</param>
        /// <param name="transpiler">Optional transpiler method</param>
        /// <param name="finalizer">Optional finalizer method</param>
        public void CreatePatch(
            string modId,
            MethodInfo originalMethod,
            MethodInfo prefix = null,
            MethodInfo postfix = null,
            MethodInfo transpiler = null,
            MethodInfo finalizer = null)
        {
            if (!_initialized)
                throw new InvalidOperationException("PatchManager is not initialized");
            
            if (originalMethod == null)
                throw new ArgumentNullException(nameof(originalMethod));
            
            if (prefix == null && postfix == null && transpiler == null && finalizer == null)
                throw new ArgumentException("At least one patch method must be provided");
            
            try
            {
                Logger.Debug($"Creating manual patch for method {originalMethod.DeclaringType?.FullName}.{originalMethod.Name}");
                Harmony harmony = GetHarmonyInstance(modId);
                
                var patcher = harmony.CreateProcessor(originalMethod);
                
                if (prefix != null)
                    patcher.AddPrefix(prefix);
                
                if (postfix != null)
                    patcher.AddPostfix(postfix);
                
                if (transpiler != null)
                    patcher.AddTranspiler(transpiler);
                
                if (finalizer != null)
                    patcher.AddFinalizer(finalizer);
                
                patcher.Patch();
                
                Logger.Debug($"Successfully applied manual patch for {originalMethod.DeclaringType?.FullName}.{originalMethod.Name}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to apply manual patch: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Removes all patches created by the specified mod
        /// </summary>
        /// <param name="modId">ID of the mod</param>
        public void RemovePatches(string modId)
        {
            if (!_initialized)
                return;
            
            if (string.IsNullOrEmpty(modId))
                throw new ArgumentNullException(nameof(modId));
            
            if (_harmonyInstances.TryGetValue(modId, out var harmony))
            {
                try
                {
                    Logger.Info($"Removing all patches for mod {modId}");
                    harmony.UnpatchAll($"sleddingame.mod.{modId}");
                    _harmonyInstances.Remove(modId);
                    Logger.Info($"Successfully removed patches for mod {modId}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to remove patches for mod {modId}: {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Adds the PatchManager to the ModFramework
        /// </summary>
        public void RegisterWithFramework()
        {
            // This will be implemented when we update the ModFramework class
        }
    }
}

