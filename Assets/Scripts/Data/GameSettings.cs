using System;
using UnityEngine;

namespace CodeTycoon.Core
{
    [Serializable]
    public class GameSettings
    {
        [Header("Audio Settings")]
        public float masterVolume = 1.0f;
        public float musicVolume = 0.7f;
        public float sfxVolume = 0.8f;
        public bool audioEnabled = true;
        
        [Header("Gameplay Settings")]
        public bool autoSaveEnabled = true;
        public float autoSaveInterval = 300f; // 5 minutes
        public bool showHints = true;
        public bool skipTutorials = false;
        public float gameSpeed = 1.0f;
        
        [Header("UI Settings")]
        public bool showTooltips = true;
        public float tooltipDelay = 0.5f;
        public bool showNotifications = true;
        public bool compactMode = false;
        public string theme = "Default";
        
        [Header("Accessibility")]
        public bool colorBlindMode = false;
        public float textScale = 1.0f;
        public bool highContrast = false;
        public bool reducedMotion = false;
        
        [Header("Advanced Settings")]
        public bool debugMode = false;
        public bool showFPS = false;
        public QualityLevel graphicsQuality = QualityLevel.Medium;
        public bool vsyncEnabled = true;
        
        [Header("Learning Preferences")]
        public DifficultyMode difficultyMode = DifficultyMode.Normal;
        public bool showCodeHints = true;
        public bool enableAITutor = true;
        public string preferredLanguage = "C#";
        
        public GameSettings()
        {
            // Initialize with default values
            ResetToDefaults();
        }
        
        public void ResetToDefaults()
        {
            masterVolume = 1.0f;
            musicVolume = 0.7f;
            sfxVolume = 0.8f;
            audioEnabled = true;
            
            autoSaveEnabled = true;
            autoSaveInterval = 300f;
            showHints = true;
            skipTutorials = false;
            gameSpeed = 1.0f;
            
            showTooltips = true;
            tooltipDelay = 0.5f;
            showNotifications = true;
            compactMode = false;
            theme = "Default";
            
            colorBlindMode = false;
            textScale = 1.0f;
            highContrast = false;
            reducedMotion = false;
            
            debugMode = false;
            showFPS = false;
            graphicsQuality = QualityLevel.Medium;
            vsyncEnabled = true;
            
            difficultyMode = DifficultyMode.Normal;
            showCodeHints = true;
            enableAITutor = true;
            preferredLanguage = "C#";
        }
        
        public void ApplySettings()
        {
            // Apply audio settings
            AudioListener.volume = audioEnabled ? masterVolume : 0f;
            
            // Apply graphics settings
            QualitySettings.SetQualityLevel((int)graphicsQuality);
            QualitySettings.vSyncCount = vsyncEnabled ? 1 : 0;
            
            // Apply game speed
            Time.timeScale = gameSpeed;
            
            Debug.Log("[GameSettings] Settings applied successfully.");
        }
        
        public void SaveSettings()
        {
            // Save settings to PlayerPrefs
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetInt("AudioEnabled", audioEnabled ? 1 : 0);
            
            PlayerPrefs.SetInt("AutoSaveEnabled", autoSaveEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("AutoSaveInterval", autoSaveInterval);
            PlayerPrefs.SetInt("ShowHints", showHints ? 1 : 0);
            PlayerPrefs.SetInt("SkipTutorials", skipTutorials ? 1 : 0);
            PlayerPrefs.SetFloat("GameSpeed", gameSpeed);
            
            PlayerPrefs.SetInt("ShowTooltips", showTooltips ? 1 : 0);
            PlayerPrefs.SetFloat("TooltipDelay", tooltipDelay);
            PlayerPrefs.SetInt("ShowNotifications", showNotifications ? 1 : 0);
            PlayerPrefs.SetInt("CompactMode", compactMode ? 1 : 0);
            PlayerPrefs.SetString("Theme", theme);
            
            PlayerPrefs.SetInt("ColorBlindMode", colorBlindMode ? 1 : 0);
            PlayerPrefs.SetFloat("TextScale", textScale);
            PlayerPrefs.SetInt("HighContrast", highContrast ? 1 : 0);
            PlayerPrefs.SetInt("ReducedMotion", reducedMotion ? 1 : 0);
            
            PlayerPrefs.SetInt("DebugMode", debugMode ? 1 : 0);
            PlayerPrefs.SetInt("ShowFPS", showFPS ? 1 : 0);
            PlayerPrefs.SetInt("GraphicsQuality", (int)graphicsQuality);
            PlayerPrefs.SetInt("VSyncEnabled", vsyncEnabled ? 1 : 0);
            
            PlayerPrefs.SetInt("DifficultyMode", (int)difficultyMode);
            PlayerPrefs.SetInt("ShowCodeHints", showCodeHints ? 1 : 0);
            PlayerPrefs.SetInt("EnableAITutor", enableAITutor ? 1 : 0);
            PlayerPrefs.SetString("PreferredLanguage", preferredLanguage);
            
            PlayerPrefs.Save();
        }
        
        public void LoadSettings()
        {
            // Load settings from PlayerPrefs
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            audioEnabled = PlayerPrefs.GetInt("AudioEnabled", 1) == 1;
            
            autoSaveEnabled = PlayerPrefs.GetInt("AutoSaveEnabled", 1) == 1;
            autoSaveInterval = PlayerPrefs.GetFloat("AutoSaveInterval", 300f);
            showHints = PlayerPrefs.GetInt("ShowHints", 1) == 1;
            skipTutorials = PlayerPrefs.GetInt("SkipTutorials", 0) == 1;
            gameSpeed = PlayerPrefs.GetFloat("GameSpeed", 1.0f);
            
            showTooltips = PlayerPrefs.GetInt("ShowTooltips", 1) == 1;
            tooltipDelay = PlayerPrefs.GetFloat("TooltipDelay", 0.5f);
            showNotifications = PlayerPrefs.GetInt("ShowNotifications", 1) == 1;
            compactMode = PlayerPrefs.GetInt("CompactMode", 0) == 1;
            theme = PlayerPrefs.GetString("Theme", "Default");
            
            colorBlindMode = PlayerPrefs.GetInt("ColorBlindMode", 0) == 1;
            textScale = PlayerPrefs.GetFloat("TextScale", 1.0f);
            highContrast = PlayerPrefs.GetInt("HighContrast", 0) == 1;
            reducedMotion = PlayerPrefs.GetInt("ReducedMotion", 0) == 1;
            
            debugMode = PlayerPrefs.GetInt("DebugMode", 0) == 1;
            showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
            graphicsQuality = (QualityLevel)PlayerPrefs.GetInt("GraphicsQuality", 2);
            vsyncEnabled = PlayerPrefs.GetInt("VSyncEnabled", 1) == 1;
            
            difficultyMode = (DifficultyMode)PlayerPrefs.GetInt("DifficultyMode", 1);
            showCodeHints = PlayerPrefs.GetInt("ShowCodeHints", 1) == 1;
            enableAITutor = PlayerPrefs.GetInt("EnableAITutor", 1) == 1;
            preferredLanguage = PlayerPrefs.GetString("PreferredLanguage", "C#");
        }
    }
    
    public enum QualityLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Ultra = 3
    }
    
    public enum DifficultyMode
    {
        Easy,
        Normal,
        Hard,
        Expert
    }
}