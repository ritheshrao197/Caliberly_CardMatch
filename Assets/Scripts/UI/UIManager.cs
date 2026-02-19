using System;
using MemoryGame;
using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;

namespace MemoryGame.Views
{
    /// <summary>
    /// Manages the overall UI flow of the game, controlling which panels and popups are visible.
    /// Handles transitions between home screen, game HUD, and various popup dialogs.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// Main canvas panel for the home screen
        /// </summary>
        [Header("Panels (MainCanvas)")]
        public GameObject homePanel;

        /// <summary>
        /// Main canvas panel for the game HUD
        /// </summary>
        public GameObject hudPanel;

        /// <summary>
        /// Root canvas for popup dialogs
        /// </summary>
        [Header("PopupCanvas (overlay)")]
        public GameObject popupCanvasRoot;

        /// <summary>
        /// Popup dialog for level results (win/lose)
        /// </summary>
        public GameObject resultPopup;

        /// <summary>
        /// Popup dialog for pause menu
        /// </summary>
        public GameObject pausePopup;

        /// <summary>
        /// Panel for level selection
        /// </summary>
        public GameObject levelSelectPanel;

        /// <summary>
        /// Dim background overlay for popups
        /// </summary>
        public GameObject dimBackground;

        /// <summary>
        /// Registers UI event handlers when the component is enabled
        /// </summary>
        void OnEnable()
        {
            Debug.Log("[UIFlow] UIManager.OnEnable - Registering event handlers");
            EventBus.Instance.Subscribe<OnShowLevelSelectEvent>(ShowLevelSelect);
            EventBus.Instance.Subscribe<StartFromHomeEvent>(HideHome);
            EventBus.Instance.Subscribe<OnHideLevelSelectEvent>(HideLevelSelect);
            EventBus.Instance.Subscribe<OnGoHomeEvent>(ShowHome);
            EventBus.Instance.Subscribe<OnShowHUDEvent>(ShowGameHUD);
            EventBus.Instance.Subscribe<OnPauseEvent>(ShowPause);
            EventBus.Instance.Subscribe<OnResumeEvent>(HidePause);
            EventBus.Instance.Subscribe<OnRestartEvent>(Restart);
        }




        /// <summary>
        /// Unregisters UI event handlers when the component is disabled
        /// </summary>
        void OnDisable()
        {
            Debug.Log("[UIFlow] UIManager.OnDisable - Unregistering event handlers"); EventBus.Instance.Unsubscribe<OnShowLevelSelectEvent>(ShowLevelSelect);
            EventBus.Instance.Unsubscribe<OnHideLevelSelectEvent>(HideLevelSelect);
            EventBus.Instance.Unsubscribe<OnGoHomeEvent>(ShowHome);
            EventBus.Instance.Unsubscribe<OnShowHUDEvent>(ShowGameHUD);
            EventBus.Instance.Unsubscribe<OnPauseEvent>(ShowPause);
            EventBus.Instance.Unsubscribe<OnResumeEvent>(HidePause);
            EventBus.Instance.Unsubscribe<OnRestartEvent>(Restart);
        }

        /// <summary>
        /// Initializes the UI by showing the home screen and hiding all popups
        /// </summary>
        void Awake()
        {
            Debug.Log("[UIFlow] UIManager.Awake - Initializing UI");
            ShowOnly(home: true, hud: false);
            HideAllPopups();
        }
        /// <summary>
        /// Handles the event to hide the home screen and show the game HUD 
        /// </summary>
        private void HideHome(StartFromHomeEvent @event)
        {
            Debug.Log("[UIFlow] HideHome - Hiding home screen and showing game HUD");
            ShowOnly(home: false, hud: true);
        }

        /// <summary>
        /// Shows the home screen and hides the game HUD
        /// </summary>
        public void ShowHome(OnGoHomeEvent e)
        {
            Debug.Log("[UIFlow] ShowHome - Displaying home screen");
            HideAllPopups();
            ShowOnly(true, false);
        }

        /// <summary>
        /// Shows the game HUD and hides the home screen
        /// </summary>
        public void ShowGameHUD(OnShowHUDEvent e)
        {
            Debug.Log("[UIFlow] ShowGameHUD - Displaying game HUD");
            HideAllPopups();
            ShowOnly(false, true);
        }

        /// <summary>
        /// Shows the level selection screen
        /// </summary>
        public void ShowLevelSelect(OnShowLevelSelectEvent e)
        {
            Debug.Log("[UIFlow] ShowLevelSelect - Displaying level selection screen");
            ShowOnly(false, false);
            SetDim(true);
            if (levelSelectPanel)
                levelSelectPanel.SetActive(true);
            EnsurePopupCanvasActive();
        }

        /// <summary>
        /// Hides the level selection screen and returns to the home screen
        /// </summary>
        public void HideLevelSelect(OnHideLevelSelectEvent e)
        {
            Debug.Log("[UIFlow] HideLevelSelect - Hiding level selection screen");
            ShowHome(new OnGoHomeEvent());
        }
        private void Restart(OnRestartEvent @event)
        {
            Debug.Log("[UIFlow] Restart - Restarting game, hiding all popups");
            HideAllPopups();
        }
        /// <summary>
        /// Shows the result popup for level completion or failure
        /// </summary>
        /// <param name="win">True if the player won, false if they lost</param>
        /// <param name="levelIndex">The zero-based level index</param>
        /// <param name="reason">Reason for losing (if applicable)</param>
        /// <param name="onNext">Action to perform when next/retry button is clicked</param>
        /// <param name="onHome">Action to perform when home button is clicked</param>
        public void ShowResult(bool win, int levelIndex, string reason, Action onNext, Action onHome)
        {
            Debug.Log($"[UIFlow] ShowResult called with win={win}, levelIndex={levelIndex}, reason={reason}");
            ShowOnly(false, false);
            SetDim(true);
            if (resultPopup != null)
            {
                var rp = resultPopup.GetComponent<ResultPopup>();
                if (rp != null)
                {
                    // Wrap callbacks so we hide the popup and show HUD before starting the next/home action
                    void NextWrapped()
                    {
                        HideAllPopups();
                        onNext?.Invoke();
                        ShowGameHUD(new OnShowHUDEvent());
                    }
                    void HomeWrapped()
                    {
                        HideAllPopups();
                        ShowHome(new OnGoHomeEvent());
                        onHome?.Invoke();
                    }
                    rp.Bind(win, levelIndex, reason, NextWrapped, HomeWrapped);
                    resultPopup.SetActive(true);
                }
            }
            EnsurePopupCanvasActive();
        }

        /// <summary>
        /// Shows the pause popup
        /// </summary>
        public void ShowPause(OnPauseEvent e)
        {
            Debug.Log("[UIFlow] ShowPause - Displaying pause menu");
            InputLock.Lock();
            SetDim(true);
            if (pausePopup != null)
                pausePopup.SetActive(true);
            EnsurePopupCanvasActive();
        }

        /// <summary>
        /// Hides the pause popup
        /// </summary>
        public void HidePause(OnResumeEvent e)
        {
            Debug.Log("[UIFlow] HidePause - Hiding pause menu");
            if (pausePopup != null)
                pausePopup.SetActive(false);
            SetDim(false);
            EnsurePopupCanvasActive();
            InputLock.Unlock();
        }

        /// <summary>
        /// Hides all popup dialogs
        /// </summary>
        void HideAllPopups()
        {
            Debug.Log("[UIFlow] HideAllPopups - Hiding all popups");
            if (resultPopup)
                resultPopup.SetActive(false);
            if (pausePopup)
                pausePopup.SetActive(false);
            if (levelSelectPanel)
                levelSelectPanel.SetActive(false);
            SetDim(false);
            EnsurePopupCanvasActive();
        }

        /// <summary>
        /// Shows only the specified panels
        /// </summary>
        /// <param name="home">True to show the home panel</param>
        /// <param name="hud">True to show the HUD panel</param>
        void ShowOnly(bool home, bool hud)
        {
            Debug.Log($"[UIFlow] ShowOnly called with home={home}, hud={hud}");
            if (homePanel)
                homePanel.SetActive(home);
            if (hudPanel)
                hudPanel.SetActive(hud);
        }

        /// <summary>
        /// Sets the dim background visibility
        /// </summary>
        /// <param name="v">True to show the dim background, false to hide it</param>
        void SetDim(bool v)
        {
            if (dimBackground)
                dimBackground.SetActive(v);
        }

        /// <summary>
        /// Ensures the popup canvas is active when any popup is visible
        /// </summary>
        void EnsurePopupCanvasActive()
        {
            if (!popupCanvasRoot)
                return;
            bool any = (resultPopup?.activeSelf ?? false) || (pausePopup?.activeSelf ?? false) ||
                       (levelSelectPanel?.activeSelf ?? false) || (dimBackground?.activeSelf ?? false);
            popupCanvasRoot.SetActive(any);
        }
    }


}