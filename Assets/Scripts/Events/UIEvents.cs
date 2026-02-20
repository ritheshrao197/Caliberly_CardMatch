using MemoryGame.Events;

namespace MemoryGame.UI.Events
{
    public struct StartFromHomeEvent : IEvent{}

    public struct StartLevelEvent : IEvent
    {
        public int LevelIndex;

        public StartLevelEvent(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
    public struct ShowLevelSelectEvent : IEvent{}
    public struct ResetProgressEvent : IEvent{}
        public struct OnClickToggleSfxEvent : IEvent{}
        public struct OnClickToggleMusicEvent : IEvent{}

    public struct ToggleSfxEvent : IEvent
    {
        public bool Enabled;

        public ToggleSfxEvent(bool enabled)
        {
            Enabled = enabled;
        }
    }

    public struct ToggleMusicEvent : IEvent
    {
        public bool Enabled;

        public ToggleMusicEvent(bool enabled)
        {
            Enabled = enabled;
        }
    }
    public struct QuitEvent : IEvent{}
     public struct OnRestartEvent : IEvent{}
    public struct OnResumeEvent : IEvent{}
    public struct OnPauseEvent : IEvent{}
    public struct OnShowHUDEvent : IEvent{}
    public struct OnGoHomeEvent : IEvent{}
    public struct OnHideLevelSelectEvent : IEvent{}
}
