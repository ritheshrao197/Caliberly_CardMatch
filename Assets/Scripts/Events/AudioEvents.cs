using System;

namespace MemoryGame.Audio.Events
{
    /// <summary>
    /// Events related to audio settings changes in the memory game.
    /// Provides events for when SFX or music settings are toggled.
    /// </summary>
    public static class AudioEvents
    {
        /// <summary>
        /// Event that fires when the SFX enabled state changes.
        /// Parameter indicates whether SFX are now enabled (true) or disabled (false).
        /// </summary>
        public static event Action<bool> OnSfxEnabledChanged;
        
        /// <summary>
        /// Event that fires when the music enabled state changes.
        /// Parameter indicates whether music is now enabled (true) or disabled (false).
        /// </summary>
        public static event Action<bool> OnMusicEnabledChanged;

        /// <summary>
        /// Raises the SFX enabled changed event with the new state.
        /// </summary>
        /// <param name="enabled">True if SFX are now enabled, false if disabled</param>
        public static void RaiseSfxEnabled(bool enabled)   => OnSfxEnabledChanged?.Invoke(enabled);
        
        /// <summary>
        /// Raises the music enabled changed event with the new state.
        /// </summary>
        /// <param name="enabled">True if music is now enabled, false if disabled</param>
        public static void RaiseMusicEnabled(bool enabled) => OnMusicEnabledChanged?.Invoke(enabled);
    }
}