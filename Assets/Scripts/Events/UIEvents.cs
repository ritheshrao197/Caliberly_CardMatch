using MemoryGame.Events;

namespace MemoryGame.UI.Events
{
    /// <summary>Start game from home using current unlocked level.</summary>
    public struct StartFromHomeEvent : IEvent { }

    /// <summary>Start a specific level from level selection.</summary>
    public struct StartLevelEvent : IEvent
    {
        public int LevelIndex;

        public StartLevelEvent(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }

    /// <summary>Open level-selection popup.</summary>
    public struct ShowLevelSelectEvent : IEvent { }

    /// <summary>Reset saved progression.</summary>
    public struct ResetProgressEvent : IEvent { }

    /// <summary>Toggle SFX state from UI.</summary>
    public struct OnClickToggleSfxEvent : IEvent { }

    /// <summary>Toggle music state from UI.</summary>
    public struct OnClickToggleMusicEvent : IEvent { }

    /// <summary>Broadcast current SFX enabled state.</summary>
    public struct ToggleSfxEvent : IEvent
    {
        public bool Enabled;

        public ToggleSfxEvent(bool enabled)
        {
            Enabled = enabled;
        }
    }

    /// <summary>Broadcast current music enabled state.</summary>
    public struct ToggleMusicEvent : IEvent
    {
        public bool Enabled;

        public ToggleMusicEvent(bool enabled)
        {
            Enabled = enabled;
        }
    }

    /// <summary>Request app quit.</summary>
    public struct QuitEvent : IEvent { }

    /// <summary>Restart current level.</summary>
    public struct OnRestartEvent : IEvent { }

    /// <summary>Resume gameplay from pause.</summary>
    public struct OnResumeEvent : IEvent { }

    /// <summary>Pause gameplay.</summary>
    public struct OnPauseEvent : IEvent { }

    /// <summary>Switch UI to HUD state.</summary>
    public struct OnShowHUDEvent : IEvent { }

    /// <summary>Switch UI to home state.</summary>
    public struct OnGoHomeEvent : IEvent { }

    /// <summary>Close level-selection popup.</summary>
    public struct OnHideLevelSelectEvent : IEvent { }
}
