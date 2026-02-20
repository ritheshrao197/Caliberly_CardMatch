using System;
using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;

namespace MemoryGame.Views
{
    public class UIManager : MonoBehaviour
    {
        [Header("Main Panels")]
        [SerializeField] private GameObject homePanel;
        [SerializeField] private GameObject hudPanel;

        [Header("Popup Canvas")]
        [SerializeField] private GameObject popupCanvasRoot;
        [SerializeField] private GameObject resultPopup;
        [SerializeField] private GameObject pausePopup;
        [SerializeField] private GameObject levelSelectPanel;

        private void Awake()
        {
            SetMainState(MainState.Home);
            HideAllPopups();
        }

        private void OnEnable()
        {
            var bus = EventBus.Instance;

            bus.Subscribe<ShowLevelSelectEvent>(OnShowLevelSelect);
            bus.Subscribe<StartFromHomeEvent>(OnStartFromHome);
            bus.Subscribe<OnHideLevelSelectEvent>(OnHideLevelSelect);
            bus.Subscribe<OnGoHomeEvent>(OnGoHome);
            bus.Subscribe<OnShowHUDEvent>(OnShowHud);
            bus.Subscribe<OnPauseEvent>(OnPause);
            bus.Subscribe<OnResumeEvent>(OnResume);
            bus.Subscribe<OnRestartEvent>(OnRestart);
            bus.Subscribe<StartLevelEvent>(OnStartLevel);
            bus.Subscribe<ShowResultEvent>(OnShowResult);
        }

        private void OnDisable()
        {
            var bus = EventBus.Instance;

            bus.Unsubscribe<ShowLevelSelectEvent>(OnShowLevelSelect);
            bus.Unsubscribe<StartFromHomeEvent>(OnStartFromHome);
            bus.Unsubscribe<OnHideLevelSelectEvent>(OnHideLevelSelect);
            bus.Unsubscribe<OnGoHomeEvent>(OnGoHome);
            bus.Unsubscribe<OnShowHUDEvent>(OnShowHud);
            bus.Unsubscribe<OnPauseEvent>(OnPause);
            bus.Unsubscribe<OnResumeEvent>(OnResume);
            bus.Unsubscribe<OnRestartEvent>(OnRestart);
            bus.Unsubscribe<StartLevelEvent>(OnStartLevel);
            bus.Unsubscribe<ShowResultEvent>(OnShowResult);
        }

        // ------------------------
        // Main Panel State
        // ------------------------

        private enum MainState
        {
            Home,
            HUD
        }

        private void SetMainState(MainState state)
        {
            Debug.Log($"Switching main UI state to: {state}");
            homePanel?.SetActive(state == MainState.Home);
            hudPanel?.SetActive(state == MainState.HUD);
            HideAllPopups();


            if (state != MainState.HUD)
                HideAllPopups();
        }

        // ------------------------
        // Popup Handling
        // ------------------------

        private void ShowLevelSelect()
        {
            SetMainState(MainState.Home);
            ShowPopup(levelSelectPanel);
        }

        private void OnShowLevelSelect(ShowLevelSelectEvent e) => ShowLevelSelect();
        private void OnStartFromHome(StartFromHomeEvent e) => SetMainState(MainState.HUD);
        private void OnHideLevelSelect(OnHideLevelSelectEvent e) => SetMainState(MainState.Home);
        private void OnGoHome(OnGoHomeEvent e) => SetMainState(MainState.Home);
        private void OnShowHud(OnShowHUDEvent e) => SetMainState(MainState.HUD);
        private void OnPause(OnPauseEvent e) => ShowPause();
        private void OnResume(OnResumeEvent e) => HidePause();
        private void OnRestart(OnRestartEvent e) => HideAllPopups();
        private void OnStartLevel(StartLevelEvent e) => SetMainState(MainState.HUD);
        private void OnShowResult(ShowResultEvent e) =>
            ShowResult(e.Win, e.LevelIndex, e.Reason, e.OnNext, e.OnHome);

        public void ShowResult(
          bool win, int levelIndex, string reason, Action onNext, Action onHome)
        {
            SetMainState(MainState.Home);

            if (resultPopup == null)
                return;

            var rp = resultPopup.GetComponent<ResultPopup>();
            if (rp == null)
                return;

            rp.Bind(
                win,
                levelIndex,
                reason,
                () =>
                {
                    HideAllPopups();
                    onNext?.Invoke();
                    SetMainState(MainState.HUD);
                },
                () =>
                {
                    HideAllPopups();
                    onHome?.Invoke();
                    SetMainState(MainState.Home);
                });

            ShowPopup(resultPopup);
        }

        private void ShowPause()
        {
            InputLock.Lock();
            ShowPopup(pausePopup);
        }

        private void HidePause()
        {
            Debug.Log("Resuming game from pause");
            pausePopup?.SetActive(false);
            InputLock.Unlock();
            RefreshPopupCanvas();
        }

        private void ShowPopup(GameObject popup)
        {
            if (popup == null)
                return;

            popup.SetActive(true);
            RefreshPopupCanvas();
        }

        private void HideAllPopups()
        {
            resultPopup?.SetActive(false);
            pausePopup?.SetActive(false);
            levelSelectPanel?.SetActive(false);

            RefreshPopupCanvas();
        }

        private void RefreshPopupCanvas()
        {
            if (popupCanvasRoot == null)
                return;

            bool active =
                (resultPopup?.activeSelf ?? false) ||
                (pausePopup?.activeSelf ?? false) ||
                (levelSelectPanel?.activeSelf ?? false) ;

            popupCanvasRoot.SetActive(active);
        }
    }
}
