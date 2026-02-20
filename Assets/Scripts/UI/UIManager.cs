using System;
using System.Collections.Generic;
using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;

namespace MemoryGame.Views
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private static readonly PanelType[] PopupPanels =
        {
            PanelType.ResultPopup,
            PanelType.PausePopup,
            PanelType.LevelSelect
        };

        [Header("Panels")]
        [SerializeField] private List<UIPanel> panels = new List<UIPanel>();


        [Header("Named Panels")]
        [SerializeField] private PanelType homePanelName = PanelType.Home;
        [SerializeField] private PanelType hudPanelName = PanelType.HUD;
        [SerializeField] private PanelType resultPopupName = PanelType.ResultPopup;
        [SerializeField] private PanelType pausePopupName = PanelType.PausePopup;
        [SerializeField] private PanelType levelSelectPanelName = PanelType.LevelSelect;

        private readonly Dictionary<PanelType, UIPanel> panelDictionary = new Dictionary<PanelType, UIPanel>();
        private EventBus _bus;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializePanels();
            SetMainState(MainState.Home);
        }

        private void OnEnable()
        {
            _bus ??= EventBus.Instance;
            var bus = _bus;

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
            if (_bus == null)
                return;

            var bus = _bus;

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

        private void InitializePanels()
        {
            panelDictionary.Clear();

            if (panels == null || panels.Count == 0)
            {
                panels = new List<UIPanel>(GetComponentsInChildren<UIPanel>(true));
            }

            foreach (var panel in panels)
            {
                if (panel == null)
                    continue;

                PanelType panelType = ResolvePanelType(panel);

                if (panelDictionary.ContainsKey(panelType))
                {
                    Debug.LogWarning($"Duplicate panel type '{panelType}' found. Skipping duplicate.");
                    continue;
                }

                panelDictionary.Add(panelType, panel);
                panel.Hide();
            }
        }

        public void OpenPanel(PanelType panelType)
        {
            SetPanelVisible(panelType, true);
        }

        public void ClosePanel(PanelType panelType)
        {
            SetPanelVisible(panelType, false);
        }

        public bool IsPanelOpen(PanelType panelType)
        {
            return panelDictionary.TryGetValue(panelType, out UIPanel panel) && panel.IsVisible;
        }

        private void SetMainState(MainState state)
        {
            if (state == MainState.Home)
            {
                OpenPanel(homePanelName);
                ClosePanel(hudPanelName);
            }
            else
            {
                OpenPanel(hudPanelName);
                ClosePanel(homePanelName);
            }

            CloseAllPopups();

        }

        // ------------------------
        // Popup Handling
        // ------------------------

        private void ShowLevelSelect()
        {
            SetMainState(MainState.Home);
            ShowPopup(levelSelectPanelName);
        }

        private void OnShowLevelSelect(ShowLevelSelectEvent e) => ShowLevelSelect();
        private void OnStartFromHome(StartFromHomeEvent e) => SetMainState(MainState.HUD);
        private void OnHideLevelSelect(OnHideLevelSelectEvent e) => SetMainState(MainState.Home);
        private void OnGoHome(OnGoHomeEvent e) => SetMainState(MainState.Home);
        private void OnShowHud(OnShowHUDEvent e) => SetMainState(MainState.HUD);
        private void OnPause(OnPauseEvent e) => ShowPause();
        private void OnResume(OnResumeEvent e) => HidePause();
        private void OnRestart(OnRestartEvent e) => CloseAllPopups();
        private void OnStartLevel(StartLevelEvent e) => SetMainState(MainState.HUD);
        private void OnShowResult(ShowResultEvent e) =>
            ShowResult(e.Win, e.LevelIndex, e.Reason, e.OnNext, e.OnHome);

        public void ShowResult(
          bool win, int levelIndex, string reason, Action onNext, Action onHome)
        {
            SetMainState(MainState.Home);
            if (!TryGetPanel(resultPopupName, out UIPanel panel))
            {
                return;
            }

            var rp = panel as ResultPopup;
            if (rp == null)
            {
                Debug.LogWarning($"Panel '{resultPopupName}' is not a ResultPopup.");
                return;
            }

            rp.Bind(
                win,
                levelIndex,
                reason,
                () =>
                {
                    CloseAllPopups();
                    onNext?.Invoke();
                    SetMainState(MainState.HUD);
                },
                () =>
                {
                    CloseAllPopups();
                    onHome?.Invoke();
                    SetMainState(MainState.Home);
                });

            ShowPopup(resultPopupName);
        }

        private void ShowPause()
        {
            InputLock.Lock();
            ShowPopup(pausePopupName);
        }

        private void HidePause()
        {
            SetPanelVisible(pausePopupName, false);
            InputLock.Unlock();
        }

        private void ShowPopup(PanelType panelType)
        {
            SetPanelVisible(panelType, true);
        }

        private void CloseAllPopups()
        {
            bool pauseWasOpen = IsPanelOpen(pausePopupName);

            for (int i = 0; i < PopupPanels.Length; i++)
            {
                SetPanelVisible(PopupPanels[i], false);
            }

            if (pauseWasOpen)
                InputLock.Unlock();

        }

       

        private bool TryGetPanel(PanelType panelType, out UIPanel panel)
        {
            if (panelDictionary.TryGetValue(panelType, out panel))
                return true;

            Debug.LogWarning($"Panel '{panelType}' not found.");
            return false;
        }

        private void SetPanelVisible(PanelType panelType, bool visible)
        {
            if (!TryGetPanel(panelType, out UIPanel panel))
                return;

            bool isVisible = panel.IsVisible;
            if (!visible && !isVisible)
                return;

            if (visible)
                panel.Show();
            else
                panel.Hide();
        }

        private static bool IsPopupPanel(PanelType panelType)
        {
            for (int i = 0; i < PopupPanels.Length; i++)
            {
                if (PopupPanels[i] == panelType)
                    return true;
            }

            return false;
        }

        private static PanelType ResolvePanelType(UIPanel panel)
        {
            if (panel is HomeView)
                return PanelType.Home;

            if (panel is HudView)
                return PanelType.HUD;

            if (panel is ResultPopup)
                return PanelType.ResultPopup;

            if (panel is PausePopup)
                return PanelType.PausePopup;

            if (panel is LevelSelectionView)
                return PanelType.LevelSelect;

            return panel.PanelType;
        }
    }
}
