namespace MemoryGame.Constants
{
    /// <summary>
    /// Constants for audio-related values in the memory game.
    /// Centralizes string constants for audio configuration and player preferences.
    /// </summary>
    public static class AudioConstants
    {
        /// <summary>
        /// PlayerPrefs key for storing whether sound effects are enabled
        /// </summary>
        public const string SfxEnabledKey = "MG_SFX_ENABLED";
        
        /// <summary>
        /// PlayerPrefs key for storing whether music is enabled
        /// </summary>
        public const string MusicEnabledKey = "MG_MUSIC_ENABLED";
        
        /// <summary>
        /// Default value for music volume (0.0 to 1.0)
        /// </summary>
        public const float DefaultMusicVolume = 0.75f;
        
        /// <summary>
        /// Default value for SFX volume (0.0 to 1.0)
        /// </summary>
        public const float DefaultSfxVolume = 1.0f;
    }
}