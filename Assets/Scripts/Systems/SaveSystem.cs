using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace CodeTycoon.Core
{
    public static class SaveSystem
    {
        private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/saves/";
        private static readonly string SAVE_FILE = "gamedata.json";
        private static readonly string BACKUP_FILE = "gamedata_backup.json";
        private static readonly string SETTINGS_FILE = "settings.json";
        
        static SaveSystem()
        {
            // Ensure save directory exists
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
                Debug.Log($"[SaveSystem] Created save directory: {SAVE_FOLDER}");
            }
        }
        
        public static void SaveGame(GameData gameData)
        {
            try
            {
                gameData.OnBeforeSave();
                
                string savePath = SAVE_FOLDER + SAVE_FILE;
                string backupPath = SAVE_FOLDER + BACKUP_FILE;
                
                // Create backup of existing save
                if (File.Exists(savePath))
                {
                    File.Copy(savePath, backupPath, true);
                }
                
                // Serialize game data to JSON
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Ignore
                };
                
                string jsonData = JsonConvert.SerializeObject(gameData, settings);
                
                // Write to file
                File.WriteAllText(savePath, jsonData);
                
                Debug.Log($"[SaveSystem] Game saved successfully to: {savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to save game: {e.Message}");
            }
        }
        
        public static GameData LoadGame()
        {
            try
            {
                string savePath = SAVE_FOLDER + SAVE_FILE;
                
                if (!File.Exists(savePath))
                {
                    Debug.Log("[SaveSystem] No save file found.");
                    return null;
                }
                
                string jsonData = File.ReadAllText(savePath);
                
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Ignore
                };
                
                GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonData, settings);
                
                // Validate loaded data
                if (ValidateGameData(gameData))
                {
                    Debug.Log($"[SaveSystem] Game loaded successfully from: {savePath}");
                    return gameData;
                }
                else
                {
                    Debug.LogWarning("[SaveSystem] Loaded game data is invalid, attempting to load backup.");
                    return LoadBackup();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to load game: {e.Message}");
                Debug.Log("[SaveSystem] Attempting to load backup...");
                return LoadBackup();
            }
        }
        
        private static GameData LoadBackup()
        {
            try
            {
                string backupPath = SAVE_FOLDER + BACKUP_FILE;
                
                if (!File.Exists(backupPath))
                {
                    Debug.LogWarning("[SaveSystem] No backup file found.");
                    return null;
                }
                
                string jsonData = File.ReadAllText(backupPath);
                
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Ignore
                };
                
                GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonData, settings);
                
                if (ValidateGameData(gameData))
                {
                    Debug.Log($"[SaveSystem] Backup loaded successfully from: {backupPath}");
                    return gameData;
                }
                else
                {
                    Debug.LogError("[SaveSystem] Backup data is also invalid.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to load backup: {e.Message}");
                return null;
            }
        }
        
        private static bool ValidateGameData(GameData gameData)
        {
            if (gameData == null) return false;
            
            // Basic validation checks
            if (gameData.companyData == null) return false;
            if (gameData.learningProgress == null) return false;
            if (gameData.settings == null) return false;
            
            // Ensure non-negative values
            if (gameData.knowledgePoints < 0) gameData.knowledgePoints = 0;
            if (gameData.cash < 0) gameData.cash = 0;
            if (gameData.companyValuation < 0) gameData.companyValuation = 0;
            
            // Ensure lists are not null
            if (gameData.activeProjects == null) gameData.activeProjects = new System.Collections.Generic.List<ProjectData>();
            if (gameData.completedProjects == null) gameData.completedProjects = new System.Collections.Generic.List<ProjectData>();
            
            return true;
        }
        
        public static void SaveSettings(GameSettings settings)
        {
            try
            {
                string settingsPath = SAVE_FOLDER + SETTINGS_FILE;
                
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };
                
                string jsonData = JsonConvert.SerializeObject(settings, jsonSettings);
                File.WriteAllText(settingsPath, jsonData);
                
                Debug.Log($"[SaveSystem] Settings saved to: {settingsPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to save settings: {e.Message}");
            }
        }
        
        public static GameSettings LoadSettings()
        {
            try
            {
                string settingsPath = SAVE_FOLDER + SETTINGS_FILE;
                
                if (!File.Exists(settingsPath))
                {
                    Debug.Log("[SaveSystem] No settings file found, creating default settings.");
                    return new GameSettings();
                }
                
                string jsonData = File.ReadAllText(settingsPath);
                
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                
                GameSettings gameSettings = JsonConvert.DeserializeObject<GameSettings>(jsonData, settings);
                
                Debug.Log($"[SaveSystem] Settings loaded from: {settingsPath}");
                return gameSettings ?? new GameSettings();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to load settings: {e.Message}");
                return new GameSettings();
            }
        }
        
        public static bool SaveExists()
        {
            return File.Exists(SAVE_FOLDER + SAVE_FILE);
        }
        
        public static void DeleteSave()
        {
            try
            {
                string savePath = SAVE_FOLDER + SAVE_FILE;
                string backupPath = SAVE_FOLDER + BACKUP_FILE;
                
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                    Debug.Log("[SaveSystem] Save file deleted.");
                }
                
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                    Debug.Log("[SaveSystem] Backup file deleted.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to delete save files: {e.Message}");
            }
        }
        
        public static string GetSaveDirectory()
        {
            return SAVE_FOLDER;
        }
        
        public static long GetSaveFileSize()
        {
            try
            {
                string savePath = SAVE_FOLDER + SAVE_FILE;
                if (File.Exists(savePath))
                {
                    FileInfo fileInfo = new FileInfo(savePath);
                    return fileInfo.Length;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to get save file size: {e.Message}");
            }
            
            return 0;
        }
    }
}