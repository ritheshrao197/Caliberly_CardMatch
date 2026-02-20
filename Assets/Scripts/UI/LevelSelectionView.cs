using System;
using MemoryGame.Events;
using MemoryGame.UI.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    /// <summary>
    /// View component for the level selection screen.
    /// Dynamically creates level buttons based on available levels and handles level selection.
    /// </summary>
    public class LevelSelectionView : UIPanel
    {
        /// <summary>
        /// Container for the level buttons
        /// </summary>
        [Header("UI")]
        public Transform content;
        
        /// <summary>
        /// Prefab for creating level buttons
        /// </summary>
        public Button levelButtonPrefab;
        
        /// <summary>
        /// Button to close the level selection screen
        /// </summary>
        public Button closeButton;

        /// <summary>
        /// Reference to the level configuration data
        /// </summary>
        [Header("Refs")]
        public LevelConfig levelConfig;

        /// <summary>
        /// Initializes the component, builds the level buttons, and sets up the close button
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Build();
            if (closeButton) 
                closeButton.onClick.AddListener(() => EventBus.Instance.Publish(new OnHideLevelSelectEvent()));
        }

        public override void Show()
        {
            Rebuild();
            base.Show();
        }
        
        /// <summary>
        /// Cleans up button click listeners when the component is destroyed
        /// </summary>
        void OnDestroy() { 
            if (closeButton) 
                closeButton.onClick.RemoveAllListeners(); 
        }

        /// <summary>
        /// Rebuilds the level selection UI by destroying existing buttons and creating new ones
        /// </summary>
        public void Rebuild()
        {
            if (!content)
                return;

            for (int i = content.childCount - 1; i >= 0; i--) 
                Destroy(content.GetChild(i).gameObject);
            Build();
        }

        /// <summary>
        /// Builds the level selection UI by creating buttons for each level
        /// </summary>
        void Build()
        {
            if (!content || !levelButtonPrefab || !levelConfig) 
                return;
                
            int unlocked = UIQueries.GetUnlockedLevelCount(levelConfig.levels.Count);

            for (int i = 0; i < levelConfig.levels.Count; i++)
            {
                var btn = Instantiate(levelButtonPrefab, content);
                var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (txt) 
                    txt.text = (i + 1).ToString();

                bool isUnlocked = i < unlocked;
                btn.interactable = isUnlocked;

                int levelIndex = i;
                btn.onClick.AddListener(() =>
                {
                    // EventBus.Instance.Publish(new OnHideLevelSelectEvent());
                    EventBus.Instance.Publish(new StartLevelEvent(levelIndex));
                });
            }
        }
    }
}
