using System;
using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;

namespace MemoryGame.Services
{
    /// <summary>
    /// Service to handle quitting the application.
    /// Listens for quit events and properly terminates the application based on the platform.
    /// </summary>
    public class QuitService : MonoBehaviour
    {
        /// <summary>
        /// Registers the quit event handler when the component is enabled
        /// </summary>
        void OnEnable()
        {
            EventBus.Instance.Subscribe<QuitEvent>(HandleQuit);
        }

        private void HandleQuit(QuitEvent @event)
        {
           
#if UNITY_EDITOR
            // In the Unity Editor, stop play mode
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID || UNITY_IOS
            // On mobile platforms, quit the application
            Application.Quit(); 
#else
            // On other platforms, quit the application
            Application.Quit();
#endif
        }

        /// <summary>
        /// Unregisters the quit event handler when the component is disabled
        /// </summary>
        void OnDisable()
        {
            EventBus.Instance.Unsubscribe<QuitEvent>(HandleQuit);
        }

       
       
    }
}