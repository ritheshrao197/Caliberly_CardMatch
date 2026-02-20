using UnityEngine;
using MemoryGame.Constants;
using MemoryGame.Events;
using MemoryGame.UI.Events;

namespace MemoryGame.Controller
{
    public class AudioManager : MonoBehaviour
    {
        // ---------- Clips ----------
        [Header("Card SFX")]
        [SerializeField] private AudioClip flip;
        [SerializeField] private AudioClip match;
        [SerializeField] private AudioClip mismatch;

        [Header("UI SFX")]
        [SerializeField] private AudioClip click;
        [SerializeField] private AudioClip popupOpen;
        [SerializeField] private AudioClip popupClose;

        [Header("Level SFX")]
        [SerializeField] private AudioClip levelStart;
        [SerializeField] private AudioClip levelWin;
        [SerializeField] private AudioClip levelFail;

        [Header("Music")]
        [SerializeField] private AudioClip bgMusic;
        [SerializeField] private bool playMusicOnAwake = true;

        [Header("Volumes")]
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = AudioConstants.DefaultSfxVolume;
        [Range(0f, 1f)] [SerializeField] private float musicVolume = AudioConstants.DefaultMusicVolume;

        private AudioSource _sfx;
        private AudioSource _music;
        private EventBus _bus;

        private bool _sfxEnabled;
        private bool _musicEnabled;

        private const string KEY_SFX = AudioConstants.SfxEnabledKey;
        private const string KEY_MUSIC = AudioConstants.MusicEnabledKey;

        #region Unity Lifecycle

        private void Awake()
        {
            _bus = EventBus.Instance; // cache once

            InitializeAudioSources();
            LoadPreferences();
            InitializeMusic();
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region Initialization

        private void InitializeAudioSources()
        {
            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;

            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;
            _music.clip = bgMusic;
            _music.volume = musicVolume;
        }

        private void LoadPreferences()
        {
            _sfxEnabled =true;// PlayerPrefs.GetInt(KEY_SFX, 1) == 1;
            _musicEnabled = true;//PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
        }

        private void InitializeMusic()
        {
            if (playMusicOnAwake && _musicEnabled && _music.clip != null)
                _music.Play();
        }

        #endregion

        #region Event Subscription

        private void SubscribeEvents()
        {
            // Gameplay
            _bus.Subscribe<CardSelectedEvent>(OnCardSelected);
            _bus.Subscribe<PairMatchedEvent>(OnPairMatched);
            _bus.Subscribe<PairMismatchedEvent>(OnPairMismatched);
            _bus.Subscribe<GameWonEvent>(OnGameWon);
            _bus.Subscribe<GameLostEvent>(OnGameLost);
            _bus.Subscribe<LevelStartedEvent>(OnLevelStarted);

            // UI
            _bus.Subscribe<OnPauseEvent>(OnPause);
            _bus.Subscribe<ShowLevelSelectEvent>(OnShowLevelSelect);

            _bus.Subscribe<OnResumeEvent>(OnResume);
            _bus.Subscribe<OnHideLevelSelectEvent>(OnHideLevelSelect);

            _bus.Subscribe<OnGoHomeEvent>(OnGoHome);
            _bus.Subscribe<OnRestartEvent>(OnRestart);
            _bus.Subscribe<StartFromHomeEvent>(OnStartFromHome);
            _bus.Subscribe<StartLevelEvent>(OnStartLevel);

            // Settings
            _bus.Subscribe<OnClickToggleSfxEvent>(OnClickToggleSfx);
            _bus.Subscribe<OnClickToggleMusicEvent>(OnClickToggleMusic);
        }

        private void UnsubscribeEvents()
        {
            _bus.Unsubscribe<CardSelectedEvent>(OnCardSelected);
            _bus.Unsubscribe<PairMatchedEvent>(OnPairMatched);
            _bus.Unsubscribe<PairMismatchedEvent>(OnPairMismatched);
            _bus.Unsubscribe<GameWonEvent>(OnGameWon);
            _bus.Unsubscribe<GameLostEvent>(OnGameLost);
            _bus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
            _bus.Unsubscribe<OnPauseEvent>(OnPause);
            _bus.Unsubscribe<ShowLevelSelectEvent>(OnShowLevelSelect);
            _bus.Unsubscribe<OnResumeEvent>(OnResume);
            _bus.Unsubscribe<OnHideLevelSelectEvent>(OnHideLevelSelect);
            _bus.Unsubscribe<OnGoHomeEvent>(OnGoHome);
            _bus.Unsubscribe<OnRestartEvent>(OnRestart);
            _bus.Unsubscribe<StartFromHomeEvent>(OnStartFromHome);
            _bus.Unsubscribe<StartLevelEvent>(OnStartLevel);
            _bus.Unsubscribe<OnClickToggleSfxEvent>(OnClickToggleSfx);
            _bus.Unsubscribe<OnClickToggleMusicEvent>(OnClickToggleMusic);
        }

        #endregion

        #region Gameplay Handlers

        private void OnLevelStarted(LevelStartedEvent e)
        {
            PlaySfx(levelStart);

            if (_musicEnabled && _music.clip != null && !_music.isPlaying)
                _music.Play();
        }

        private void OnCardSelected(CardSelectedEvent e) => PlaySfx(flip);
        private void OnPairMatched(PairMatchedEvent e) => PlaySfx(match);
        private void OnPairMismatched(PairMismatchedEvent e) => PlaySfx(mismatch);
        private void OnGameWon(GameWonEvent e) => PlaySfx(levelWin);
        private void OnGameLost(GameLostEvent e) => PlaySfx(levelFail);
        private void OnPause(OnPauseEvent e) => PlaySfx(popupOpen);
        private void OnShowLevelSelect(ShowLevelSelectEvent e) => PlaySfx(popupOpen);
        private void OnResume(OnResumeEvent e) => PlaySfx(popupClose);
        private void OnHideLevelSelect(OnHideLevelSelectEvent e) => PlaySfx(popupClose);
        private void OnGoHome(OnGoHomeEvent e) => PlaySfx(click);
        private void OnRestart(OnRestartEvent e) => PlaySfx(click);
        private void OnStartFromHome(StartFromHomeEvent e) => PlaySfx(click);
        private void OnStartLevel(StartLevelEvent e) => PlaySfx(click);
        private void OnClickToggleSfx(OnClickToggleSfxEvent e) => ToggleSfx();
        private void OnClickToggleMusic(OnClickToggleMusicEvent e) => ToggleMusic();

        #endregion

        #region Core Audio Logic

        private void PlaySfx(AudioClip clip)
        {
            if (!_sfxEnabled || clip == null)
                return;

            _sfx.PlayOneShot(clip, sfxVolume);
        }

        private void ToggleSfx()
        {
            _sfxEnabled = !_sfxEnabled;
            PlayerPrefs.SetInt(KEY_SFX, _sfxEnabled ? 1 : 0);
            EventBus.Instance.Publish(new ToggleSfxEvent(_sfxEnabled));
        }

        private void ToggleMusic()
        {
            _musicEnabled = !_musicEnabled;
            PlayerPrefs.SetInt(KEY_MUSIC, _musicEnabled ? 1 : 0);

            if (_musicEnabled)
            {
                if (_music.clip != null && !_music.isPlaying)
                    _music.Play();
            }
            else
            {
                if (_music.isPlaying)
                    _music.Stop();
            }
            EventBus.Instance.Publish(new ToggleMusicEvent(_musicEnabled));
        }

        #endregion
    }
}
