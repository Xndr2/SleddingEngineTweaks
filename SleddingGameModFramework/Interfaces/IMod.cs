namespace SleddingGameModFramework.Interfaces
{
    public interface IMod
    {
        /// <summary>
        /// Called when the mod is loaded
        /// </summary>
        void OnLoad();

        /// <summary>
        /// Called when the mod is unloaded
        /// </summary>
        void OnUnload();

        /// <summary>
        /// Called every frame
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// Called when the game is paused
        /// </summary>
        void OnPause();

        /// <summary>
        /// Called when the game is resumed
        /// </summary>
        void OnResume();
    }
} 