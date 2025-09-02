using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace CodeTycoon.Core
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;
        
        [Header("Audio Settings")]
        [SerializeField] private bool enableAudio = true;
        [SerializeField] private float masterVolume = 1.0f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 0.8f;
        
        [Header("Background Music")]
        [SerializeField] private AudioClip[] backgroundTracks;
        [SerializeField] private bool shuffleMusic = true;
        [SerializeField] private bool loopMusic = true;
        [SerializeField] private float fadeTime = 2.0f;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioClip notificationSound;
        [SerializeField] private AudioClip unlockSound;
        [SerializeField] private AudioClip cashSound;
        
        private Dictionary<string, AudioClip> soundEffects;
        private Coroutine musicFadeCoroutine;
        private int currentTrackIndex = 0;
        
        public void Initialize()
        {
            Debug.Log("[AudioManager] Initializing Audio Manager...");
            
            SetupAudioSources();
            InitializeSoundEffects();
            LoadAudioSettings();
            
            if (enableAudio && backgroundTracks != null && backgroundTracks.Length > 0)
            {
                PlayBackgroundMusic();
            }
            
            Debug.Log("[AudioManager] Audio Manager initialized.");
        }
        
        private void SetupAudioSources()
        {
            // Create audio sources if they don't exist
            if (musicSource == null)
            {
                GameObject musicGO = new GameObject("MusicSource");
                musicGO.transform.SetParent(transform);
                musicSource = musicGO.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxGO = new GameObject("SFXSource");
                sfxGO.transform.SetParent(transform);
                sfxSource = sfxGO.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            if (uiSource == null)
            {
                GameObject uiGO = new GameObject("UISource");
                uiGO.transform.SetParent(transform);
                uiSource = uiGO.AddComponent<AudioSource>();
                uiSource.loop = false;
                uiSource.playOnAwake = false;
            }
        }
        
        private void InitializeSoundEffects()
        {
            soundEffects = new Dictionary<string, AudioClip>();
            
            // Register sound effects
            if (clickSound != null) soundEffects["click"] = clickSound;
            if (successSound != null) soundEffects["success"] = successSound;
            if (errorSound != null) soundEffects["error"] = errorSound;
            if (notificationSound != null) soundEffects["notification"] = notificationSound;
            if (unlockSound != null) soundEffects["unlock"] = unlockSound;
            if (cashSound != null) soundEffects["cash"] = cashSound;
        }
        
        private void LoadAudioSettings()
        {
            if (GameManager.Instance?.GetGameData()?.settings != null)
            {
                GameSettings settings = GameManager.Instance.GetGameData().settings;
                
                enableAudio = settings.audioEnabled;
                masterVolume = settings.masterVolume;
                musicVolume = settings.musicVolume;
                sfxVolume = settings.sfxVolume;
                
                ApplyAudioSettings();
            }
        }
        
        private void ApplyAudioSettings()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
            
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume * masterVolume;
            }
            
            if (uiSource != null)
            {
                uiSource.volume = sfxVolume * masterVolume;
            }
            
            AudioListener.volume = enableAudio ? masterVolume : 0f;
        }
        
        public void PlayBackgroundMusic()
        {
            if (!enableAudio || backgroundTracks == null || backgroundTracks.Length == 0)
                return;
                
            AudioClip nextTrack = backgroundTracks[currentTrackIndex];
            
            if (musicSource.isPlaying)
            {
                StartCoroutine(CrossFadeMusic(nextTrack));
            }
            else
            {
                musicSource.clip = nextTrack;
                musicSource.Play();
            }
            
            if (shuffleMusic)
            {
                currentTrackIndex = Random.Range(0, backgroundTracks.Length);
            }
            else
            {
                currentTrackIndex = (currentTrackIndex + 1) % backgroundTracks.Length;
            }
        }
        
        private IEnumerator CrossFadeMusic(AudioClip newTrack)
        {
            float startVolume = musicSource.volume;
            
            // Fade out current track
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
                yield return null;
            }
            
            // Switch to new track
            musicSource.clip = newTrack;
            musicSource.Play();
            
            // Fade in new track
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
                yield return null;
            }
            
            musicSource.volume = startVolume;
        }
        
        public void PlaySoundEffect(string effectName)
        {
            if (!enableAudio || soundEffects == null || !soundEffects.ContainsKey(effectName))
                return;
                
            AudioClip clip = soundEffects[effectName];
            if (clip == null) return;
            
            if (IsUISound(effectName))
            {
                if (uiSource != null)
                    uiSource.PlayOneShot(clip);
            }
            else
            {
                if (sfxSource != null)
                    sfxSource.PlayOneShot(clip);
            }
        }
        
        public void PlaySoundEffect(AudioClip clip, bool isUISound = false)
        {
            if (!enableAudio || clip == null)
                return;
                
            if (isUISound)
            {
                uiSource.PlayOneShot(clip);
            }
            else
            {
                sfxSource.PlayOneShot(clip);
            }
        }
        
        private bool IsUISound(string effectName)
        {
            return effectName == "click" || effectName == "unlock" || effectName == "notification";
        }
        
        public void PlayClickSound()
        {
            PlaySoundEffect("click");
        }
        
        public void PlaySuccessSound()
        {
            PlaySoundEffect("success");
        }
        
        public void PlayErrorSound()
        {
            PlaySoundEffect("error");
        }
        
        public void PlayNotificationSound()
        {
            PlaySoundEffect("notification");
        }
        
        public void PlayUnlockSound()
        {
            PlaySoundEffect("unlock");
        }
        
        public void PlayCashSound()
        {
            PlaySoundEffect("cash");
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyAudioSettings();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyAudioSettings();
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyAudioSettings();
        }
        
        public void SetAudioEnabled(bool enabled)
        {
            enableAudio = enabled;
            ApplyAudioSettings();
            
            if (!enabled && musicSource.isPlaying)
            {
                musicSource.Pause();
            }
            else if (enabled && !musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.UnPause();
            }
        }
        
        public void PauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }
        
        public void ResumeMusic()
        {
            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.UnPause();
            }
        }
        
        public void StopMusic()
        {
            musicSource.Stop();
        }
        
        private void Update()
        {
            // Check if music finished and needs to loop/change
            if (enableAudio && backgroundTracks != null && backgroundTracks.Length > 0)
            {
                if (!musicSource.isPlaying && musicSource.clip != null)
                {
                    if (loopMusic)
                    {
                        PlayBackgroundMusic();
                    }
                }
            }
        }
        
        private void OnDestroy()
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }
        }
    }
}