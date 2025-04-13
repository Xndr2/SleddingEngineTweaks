using System;
using HarmonyLib;
using SleddingGameModFramework.Patches;
using SleddingGameModFramework.Utils;

namespace SleddingGame.TestMod
{
    /// <summary>
    /// Example patch class for the test mod
    /// This won't do anything until the game releases and we can identify classes to patch
    /// </summary>
    [Patch("SampleGame.PlayerController", "Move")]
    public static class TestPatches
    {
        /// <summary>
        /// Example prefix patch for a movement method
        /// </summary>
        [HarmonyPrefix]
        public static bool MovePrefix(ref float ___speed)
        {
            // This is just an example and would need to be updated for the actual game
            Logger.Info("Move prefix called");
            
            // Modify player speed
            ___speed *= 1.5f;
            
            // Return true to allow the original method to run
            return true;
        }
        
        /// <summary>
        /// Example postfix patch for a movement method
        /// </summary>
        [HarmonyPostfix]
        public static void MovePostfix()
        {
            // This is just an example and would need to be updated for the actual game
            Logger.Info("Move postfix called");
        }
    }
}