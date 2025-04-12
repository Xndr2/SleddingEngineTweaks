namespace SleddingGameModFramework.API
{
    /// <summary>
    /// Interface for mods that want to hook into various game events
    /// </summary>
    public interface IModEvents
    {
        /// <summary>
        /// Called when the game starts loading
        /// </summary>
        void OnGameStartLoading();
        
        /// <summary>
        /// Called when the game has finished loading
        /// </summary>
        void OnGameLoaded();
        
        /// <summary>
        /// Called each game frame
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds</param>
        void OnUpdate(float deltaTime);
        
        /// <summary>
        /// Called when the game is about to be unloaded
        /// </summary>
        void OnGameUnloading();
    }
}