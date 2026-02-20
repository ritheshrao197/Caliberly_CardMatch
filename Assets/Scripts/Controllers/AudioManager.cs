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
            _bus.Subscribe<CardSelectedEvent>(_ => PlaySfx(flip));
            _bus.Subscribe<PairMatchedEvent>(_ => PlaySfx(match));
            _bus.Subscribe<PairMismatchedEvent>(_ => PlaySfx(mismatch));
            _bus.Subscribe<GameWonEvent>(_ => PlaySfx(levelWin));
            _bus.Subscribe<GameLostEvent>(_ => PlaySfx(levelFail));
            _bus.Subscribe<LevelStartedEvent>(OnLevelStarted);

            // UI
            _bus.Subscribe<OnPauseEvent>(_ => PlaySfx(popupOpen));
            _bus.Subscribe<ShowLevelSelectEvent>(_ => PlaySfx(popupOpen));

            _bus.Subscribe<OnResumeEvent>(_ => PlaySfx(popupClose));
            _bus.Subscribe<OnHideLevelSelectEvent>(_ => PlaySfx(popupClose));

            _bus.Subscribe<OnGoHomeEvent>(_ => PlaySfx(click));
            _bus.Subscribe<OnRestartEvent>(_ => PlaySfx(click));
            _bus.Subscribe<StartFromHomeEvent>(_ => PlaySfx(click));
            _bus.Subscribe<StartLevelEvent>(_ => PlaySfx(click));

            // Settings
            _bus.Subscribe<OnClickToggleSfxEvent>(_ => ToggleSfx());
            _bus.Subscribe<OnClickToggleMusicEvent>(_ => ToggleMusic());
        }

        private void UnsubscribeEvents()
        {
            _bus.Clear(); 
            // Since this AudioManager subscribes anonymously,
            // safest option is clearing if it owns global audio.
            // If not, switch to stored delegates instead.
        }

        #endregion

        #region Gameplay Handlers

        private void OnLevelStarted(LevelStartedEvent e)
        {
            PlaySfx(levelStart);

            if (_musicEnabled && _music.clip != null && !_music.isPlaying)
                _music.Play();
        }

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
            // PlayerPrefs.SetInt(KEY_SFX, _sfxEnabled ? 1 : 0);
            EventBus.Instance.Publish(new ToggleSfxEvent(_sfxEnabled));
        }

        private void ToggleMusic()
        {
            _musicEnabled = !_musicEnabled;
            // PlayerPrefs.SetInt(KEY_MUSIC, _musicEnabled ? 1 : 0);

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
