using System;
using MemoryGame.Controller;
using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    /// <summary>
    /// View component for the Home screen of the Memory Game.
    /// Handles binding UI button clicks to game events and updating audio icons.
    /// </summary>
    public class HomeView : MonoBehaviour
    {
        /// <summary>
        /// Button to start the game from the home screen
        /// </summary>
        [Header("Buttons")]
        public Button startButton;
        
        /// <summary>
        /// Button to reset player progress
        /// </summary>
        public Button resetProgressButton;
        
        /// <summary>
        /// Button to show the level selection screen
        /// </summary>
        public Button levelSelectButton;
        
        /// <summary>
        /// Button to toggle sound effects on/off
        /// </summary>
        public Button sfxButton;
        
        /// <summary>
        /// Button to exit the application
        /// </summary>
        public Button exitButton;
        
        /// <summary>
        /// Button to toggle music on/off
        /// </summary>
        public Button musicButton;
        
        /// <summary>
        /// Image to display the sound effects status (on/off)
        /// </summary>
        [Header("Icons")]
        public Image sfxIcon;
        
        /// <summary>
        /// Image to display the music status (on/off)
        /// </summary>
        public Image musicIcon;
        
        /// <summary>
        /// Sprite to show when sound effects are enabled
        /// </summary>
        [Header("Sprites")]
        public Sprite onSpriteSfx;
        
        /// <summary>
        /// Sprite to show when sound effects are disabled
        /// </summary>
        public Sprite offSpriteSfx;
        
        /// <summary>
        /// Sprite to show when music is enabled
        /// </summary>
        public Sprite onSpriteMusic;
        
        /// <summary>
        /// Sprite to show when music is disabled
        /// </summary>
        public Sprite offSpriteMusic;

        /// <summary>
        /// Initializes the component and binds button click events to UI events
        /// </summary>
        void Awake()
        {
            if (startButton) 
                startButton.onClick.AddListener(() => EventBus.Instance.Publish(new StartFromHomeEvent()));
            if (resetProgressButton) 
                resetProgressButton.onClick.AddListener(() => EventBus.Instance.Publish(new ResetProgressEvent()));
            if (levelSelectButton) 
                levelSelectButton.onClick.AddListener(() => EventBus.Instance.Publish(new ShowLevelSelectEvent()));
            if (sfxButton) 
                sfxButton.onClick.AddListener(() => EventBus.Instance.Publish(new OnClickToggleSfxEvent()));
            if (musicButton) 
                musicButton.onClick.AddListener(() => EventBus.Instance.Publish(new OnClickToggleMusicEvent()));
            if (exitButton) 
                exitButton.onClick.AddListener(() => EventBus.Instance.Publish(new QuitEvent()));
        }
        
        /// <summary>
        /// Registers audio event handlers when the component is enabled
        /// </summary>
        void OnEnable()
        {
             EventBus.Instance.Subscribe<ToggleSfxEvent>(OnToggleSfx);
            EventBus.Instance.Subscribe<ToggleMusicEvent>(OnToggleMusic);
           
        }
        
        /// <summary>
        /// Unregisters audio event handlers when the component is disabled
        /// </summary>
        void OnDisable()
        {
            EventBus.Instance.Unsubscribe<ToggleSfxEvent>(OnToggleSfx);
            EventBus.Instance.Unsubscribe<ToggleMusicEvent>(OnToggleMusic);
        }

        private void OnToggleSfx(ToggleSfxEvent evt)
        {
            if (sfxIcon) 
                sfxIcon.sprite = evt.Enabled ? onSpriteSfx : offSpriteSfx;
        }

        private void OnToggleMusic(ToggleMusicEvent evt)
        {
            if (musicIcon) 
                musicIcon.sprite = evt.Enabled ? onSpriteMusic : offSpriteMusic;
        }

        /// <summary>
        /// Cleans up button click listeners when the component is destroyed
        /// </summary>
        void OnDestroy()
        {
            if (startButton) 
                startButton.onClick.RemoveAllListeners();
            if (resetProgressButton) 
                resetProgressButton.onClick.RemoveAllListeners();
            if (levelSelectButton) 
                levelSelectButton.onClick.RemoveAllListeners();
            if (sfxButton) 
                sfxButton.onClick.RemoveAllListeners();
            if (musicButton) 
                musicButton.onClick.RemoveAllListeners();
            if (exitButton) 
                exitButton.onClick.RemoveAllListeners();
        }
        
        /// <summary>
        /// Updates the sound effects icon when the SFX enabled state changes
        /// </summary>
        /// <param name="enabled">True if SFX are enabled, false if disabled</param>
        void OnSfxChanged(bool enabled)
        {
            if (sfxIcon) 
                sfxIcon.sprite = enabled ? onSpriteSfx : offSpriteSfx;
        }

        /// <summary>
        /// Updates the music icon when the music enabled state changes
        /// </summary>
        /// <param name="enabled">True if music is enabled, false if disabled</param>
        void OnMusicChanged(bool enabled)
        {
            if (musicIcon) 
                musicIcon.sprite = enabled ? onSpriteMusic : offSpriteMusic;
        }
    }
}