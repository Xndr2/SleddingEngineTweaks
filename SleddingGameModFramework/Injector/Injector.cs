using System;
using System.IO;
using System.Reflection;
using SleddingGameModFramework.Utils;

namespace SleddingGameModFramework.Injector
{
    /// <summary>
    /// Handles injecting the mod framework into the game
    /// </summary>
    public static class Injector
    {
        /// <summary>
        /// Entry point for the injector
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            try
            {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Console.WriteLine($"Initializing SleddingGameModFramework injector from {baseDir}");
                
                // Initialize the framework
                ModFramework.Instance.Initialize(baseDir);
                
                // Load and initialize mods
                ModFramework.Instance.LoadMods();
                
                Console.WriteLine("Injection successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during injection: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            // Prevent console from closing immediately if launched directly
            if (Array.IndexOf(args, "--wait") >= 0)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}