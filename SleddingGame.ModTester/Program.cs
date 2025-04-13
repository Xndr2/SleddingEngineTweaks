using System;
using System.IO;
using System.Threading;
using SleddingGameModFramework;
using SleddingGameModFramework.Utils;

namespace SleddingGame.ModTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sledding Game Mod Framework Tester");
            Console.WriteLine("==================================");
            
            try
            {
                // Get the executable directory
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                
                // Create mods directory if it doesn't exist
                string modsDir = Path.Combine(baseDir, "Mods");
                Directory.CreateDirectory(modsDir);
                
                // Create test mod directory
                string testModDir = Path.Combine(modsDir, "test_mod");
                Directory.CreateDirectory(testModDir);
                
                // Copy test mod files to mods directory
                CopyTestModFiles(testModDir);
                
                Console.WriteLine("Initializing mod framework...");
                ModFramework.Instance.Initialize(baseDir);
                
                Console.WriteLine("Loading mods...");
                ModFramework.Instance.LoadMods();
                
                Console.WriteLine("Simulating game events...");
                ModFramework.Instance.SimulateGameEvent("GameStartLoading");
                ModFramework.Instance.SimulateGameEvent("GameLoaded");
                
                // Simulate update loop
                Console.WriteLine("Press any key to stop simulation...");
                var simulationThread = new Thread(SimulateGameLoop);
                simulationThread.Start();
                
                Console.ReadKey();
                _isRunning = false;
                simulationThread.Join();
                
                Console.WriteLine("Shutting down...");
                ModFramework.Instance.Shutdown();
                
                Console.WriteLine("Test completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        private static bool _isRunning = true;
        
        private static void SimulateGameLoop()
        {
            float deltaTime = 0.016f; // ~60 FPS
            DateTime lastUpdate = DateTime.Now;
            
            while (_isRunning)
            {
                // Calculate actual delta time
                DateTime now = DateTime.Now;
                deltaTime = (float)(now - lastUpdate).TotalSeconds;
                lastUpdate = now;
                
                // Simulate game update
                ModFramework.Instance.SimulateGameEvent("GameUpdate", deltaTime);
                
                // Sleep to avoid consuming too much CPU
                Thread.Sleep(16); // ~60 updates per second
            }
        }
        
        private static void CopyTestModFiles(string targetDir)
        {
            // In a real implementation, you'd need to find and copy the test mod DLL and JSON
            // For simplicity, we'll assume they're in a specific location
            
            string testModDll = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SleddingGame.TestMod.dll");
            string testModJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modinfo.json");
            
            if (File.Exists(testModDll))
            {
                File.Copy(testModDll, Path.Combine(targetDir, "SleddingGame.TestMod.dll"), true);
                Console.WriteLine("Copied test mod DLL");
            }
            else
            {
                Console.WriteLine($"Warning: Test mod DLL not found at {testModDll}");
            }
            
            if (File.Exists(testModJson))
            {
                File.Copy(testModJson, Path.Combine(targetDir, "modinfo.json"), true);
                Console.WriteLine("Copied test mod info");
            }
            else
            {
                Console.WriteLine($"Warning: Test mod info not found at {testModJson}");
            }
        }
    }
}