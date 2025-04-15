using UnityEngine;

namespace SleddingGameModFramework.UI
{
    public interface IModUI
    {
        /// <summary>
        /// Called when the UI is created
        /// </summary>
        void OnCreate();

        /// <summary>
        /// Called when the UI is destroyed
        /// </summary>
        void OnDestroy();

        /// <summary>
        /// Called every frame to update the UI
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// Called when the UI should be shown
        /// </summary>
        void Show();

        /// <summary>
        /// Called when the UI should be hidden
        /// </summary>
        void Hide();

        /// <summary>
        /// The root GameObject of the UI
        /// </summary>
        GameObject Root { get; }

        /// <summary>
        /// Whether the UI is currently visible
        /// </summary>
        bool IsVisible { get; }
    }
} 