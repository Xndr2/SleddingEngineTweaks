using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SleddingGameModFramework.Utils;

namespace SleddingGameModFramework.Patches
{
    /// <summary>
    /// Manages Harmony patches for the mod framework
    /// </summary>
    public class PatchManager
    {
        private readonly Dictionary<string, Harmony> _harmonyInstances = new();
        
        /// <summary>
        /// Creates a new Harmony instance for the specified mod
        /// </summary>
        /// <param name="modId">Unique ID of the mod</param>
        /// <returns>The Harmony instance</returns>
        public Harmony GetHarmonyInstance(string modId)
        {
            if (string.IsNullOrEmpty(modId))
                throw new ArgumentNullException(nameof(modId));
            
            if (!_harmonyInstances.TryGetValue(modId, out var harmony))
            {
                // Create a new Harmony instance for this mod
                string harmonyId = $"com.sleddingmodframework.{modId}";
                harmony = new Harmony(harmonyId);
                _harmonyInstances[modId] = harmony;
                
                Logger.Debug($"Created Harmony instance for mod: {modId}");
            }
            
            return harmony;
        }
        
        /// <summary>
        /// Applies all patches in the specified assembly
        /// </summary>
        /// <param name="modId">ID of the mod</param>
        /// <param name="assembly">Assembly containing patches</param>
        public void ApplyPatches(string modId, Assembly assembly)
        {
            if (string.IsNullOrEmpty(modId))
                throw new ArgumentNullException(nameof(modId));
            
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            
            try
            {
                Logger.Info($"Applying patches for mod: {modId}");
                
                Harmony harmony = GetHarmonyInstance(modId);
                harmony.PatchAll(assembly);
                
                Logger.Info($"Successfully applied patches for mod: {modId}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error applying patches for mod {modId}: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Applies a single patch manually
        /// </summary>
        /// <param name="modId">ID of the mod</param>
        /// <param name="originalMethod">Method to patch</param>
        /// <param name="prefix">Optional prefix method</param>
        /// <param name="postfix">Optional postfix method</param>
        /// <param name="transpiler">Optional transpiler method</param>
        /// <param name="finalizer">Optional finalizer method</param>
        /// <returns>The patch info</returns>
        public MethodInfo ApplyPatch(
            string modId,
            MethodInfo originalMethod, 
            MethodInfo prefix = null, 
            MethodInfo postfix = null, 
            MethodInfo transpiler = null,
            MethodInfo finalizer = null)
        {
            if (string.IsNullOrEmpty(modId))
                throw new ArgumentNullException(nameof(modId));
            
            if (originalMethod == null)
                throw new ArgumentNullException(nameof(originalMethod));
            
            if (prefix == null && postfix == null && transpiler == null && finalizer == null)
                throw new ArgumentException("At least one patch method must be provided");
            
            try
            {
                Logger.Info($"Applying manual patch to {originalMethod.DeclaringType?.FullName}.{originalMethod.Name} for mod: {modId}");
                
                Harmony harmony = GetHarmonyInstance(modId);
                HarmonyMethod prefixPatch = prefix != null ? new HarmonyMethod(prefix) : null;
                HarmonyMethod postfixPatch = postfix != null ? new HarmonyMethod(postfix) : null;
                HarmonyMethod transpilerPatch = transpiler != null ? new HarmonyMethod(transpiler) : null;
                HarmonyMethod finalizerPatch = finalizer != null ? new HarmonyMethod(finalizer) : null;
                
                return harmony.Patch(
                    originalMethod,
                    prefixPatch,
                    postfixPatch,
                    transpilerPatch,
                    finalizerPatch);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error applying patch for mod {modId}: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Removes all patches applied by the specified mod
        /// </summary>
        /// <param name="modId">ID of the mod</param>
        public void RemovePatches(string modId)
        {
            if (string.IsNullOrEmpty(modId))
                return;
            
            if (_harmonyInstances.TryGetValue(modId, out var harmony))
            {
                try
                {
                    Logger.Info($"Removing all patches for mod: {modId}");
                    harmony.UnpatchAll(harmony.Id);
                    _harmonyInstances.Remove(modId);
                    Logger.Info($"Successfully removed patches for mod: {modId}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error removing patches for mod {modId}: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Removes all patches from all mods
        /// </summary>
        public void RemoveAllPatches()
        {
            Logger.Info("Removing all patches from all mods");
            
            foreach (var harmony in _harmonyInstances.Values)
            {
                try
                {
                    harmony.UnpatchAll(harmony.Id);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error removing patches for instance {harmony.Id}: {ex.Message}");
                }
            }
            
            _harmonyInstances.Clear();
            Logger.Info("All patches removed");
        }
    }
}