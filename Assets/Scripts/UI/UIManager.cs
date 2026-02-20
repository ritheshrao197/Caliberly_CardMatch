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

            bus.Subscribe<ShowLevelSelectEvent>(_ => ShowLevelSelect());
            bus.Subscribe<StartFromHomeEvent>(_ => SetMainState(MainState.HUD));
            bus.Subscribe<OnHideLevelSelectEvent>(_ => SetMainState(MainState.Home));
            bus.Subscribe<OnGoHomeEvent>(_ => SetMainState(MainState.Home));
            bus.Subscribe<OnShowHUDEvent>(_ => SetMainState(MainState.HUD));
            bus.Subscribe<OnPauseEvent>(_ => ShowPause());
            bus.Subscribe<OnResumeEvent>(_ => HidePause());
            bus.Subscribe<OnRestartEvent>(_ => HideAllPopups());
            bus.Subscribe<StartLevelEvent>(_ => SetMainState(MainState.HUD));
            bus.Subscribe<ShowResultEvent>(e => ShowResult(
                e.Win,
                e.LevelIndex,
                e.Reason,
                e.OnNext,
                e.OnHome));
        }

        private void OnDisable()
        {
            var bus = EventBus.Instance;

            bus.Unsubscribe<ShowLevelSelectEvent>(_ => ShowLevelSelect());
            bus.Unsubscribe<StartFromHomeEvent>(_ => SetMainState(MainState.HUD));
            bus.Unsubscribe<OnHideLevelSelectEvent>(_ => SetMainState(MainState.Home));
            bus.Unsubscribe<OnGoHomeEvent>(_ => SetMainState(MainState.Home));
            bus.Unsubscribe<OnShowHUDEvent>(_ => SetMainState(MainState.HUD));
            bus.Unsubscribe<OnPauseEvent>(_ => ShowPause());
            bus.Unsubscribe<OnResumeEvent>(_ => HidePause());
            bus.Unsubscribe<OnRestartEvent>(_ => HideAllPopups());
            bus.Unsubscribe<StartLevelEvent>(_ => SetMainState(MainState.HUD));
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
