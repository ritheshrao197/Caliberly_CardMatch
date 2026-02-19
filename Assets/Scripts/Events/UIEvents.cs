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
    public struct RestartEvent : IEvent{}
    public struct GoHomeEvent : IEvent{}
    public struct ShowHUDEvent : IEvent{}
    public struct ShowLevelSelectEvent : IEvent{}
    public struct HideLevelSelectEvent : IEvent{}
    public struct PauseEvent : IEvent{}
    public struct ResumeEvent : IEvent{}
    public struct ResetProgressEvent : IEvent{}
    public struct ToggleSfxEvent : IEvent{}
    public struct ToggleMusicEvent : IEvent{}
    public struct QuitEvent : IEvent{}
     public struct OnRestartEvent : IEvent{}
    public struct OnResumeEvent : IEvent{}
    public struct OnPauseEvent : IEvent{}
    public struct OnShowHUDEvent : IEvent{}
    public struct OnGoHomeEvent : IEvent{}
    public struct OnHideLevelSelectEvent : IEvent{}
    public struct OnShowLevelSelectEvent : IEvent{}
}
