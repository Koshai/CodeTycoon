using UnityEngine;
using System.IO;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Utility to reset save data for testing purposes
    /// </summary>
    public class SaveDataResetter : MonoBehaviour
    {
        [Header("Reset Options")]
        [SerializeField] private bool resetOnStart = false;
        [SerializeField] private bool showDebugButtons = true;
        
        private void Start()
        {
            if (resetOnStart)
            {
                ResetSaveData();
            }
        }
        
        private void OnGUI()
        {
            if (!showDebugButtons) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 200, 150));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Save Data Controls");
            
            if (GUILayout.Button("Reset Save Data"))
            {
                ResetSaveData();
            }
            
            if (GUILayout.Button("Delete All Saves"))
            {
                DeleteAllSaveFiles();
            }
            
            if (GUILayout.Button("Start New Game"))
            {
                StartNewGame();
            }
            
            string saveDir = SaveSystem.GetSaveDirectory();
            if (GUILayout.Button("Open Save Folder"))
            {
                if (Directory.Exists(saveDir))
                {
                    System.Diagnostics.Process.Start("explorer.exe", saveDir);
                }
                else
                {
                    Debug.Log($"Save directory doesn't exist: {saveDir}");
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        [ContextMenu("Reset Save Data")]
        public void ResetSaveData()
        {
            Debug.Log("[SaveDataResetter] Resetting save data...");
            
            // Delete save files
            SaveSystem.DeleteSave();
            
            // Reset current game state if GameManager exists
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
                Debug.Log("[SaveDataResetter] Started new game with fresh data");
            }
            
            Debug.Log("[SaveDataResetter] Save data reset complete!");
        }
        
        public void DeleteAllSaveFiles()
        {
            Debug.Log("[SaveDataResetter] Deleting all save files...");
            
            string saveDir = SaveSystem.GetSaveDirectory();
            
            if (Directory.Exists(saveDir))
            {
                try
                {
                    Directory.Delete(saveDir, true);
                    Debug.Log($"[SaveDataResetter] Deleted save directory: {saveDir}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SaveDataResetter] Failed to delete save directory: {e.Message}");
                }
            }
            else
            {
                Debug.Log("[SaveDataResetter] Save directory doesn't exist");
            }
        }
        
        public void StartNewGame()
        {
            Debug.Log("[SaveDataResetter] Starting completely new game...");
            
            if (GameManager.Instance != null)
            {
                // Force start new game
                GameManager.Instance.StartNewGame();
                
                // Immediately save to overwrite any existing save
                GameManager.Instance.SaveGame();
                
                Debug.Log("[SaveDataResetter] New game started and saved");
            }
        }
        
        public void ShowSaveFileInfo()
        {
            string saveDir = SaveSystem.GetSaveDirectory();
            bool saveExists = SaveSystem.SaveExists();
            long saveSize = SaveSystem.GetSaveFileSize();
            
            Debug.Log($"[SaveDataResetter] Save Info:");
            Debug.Log($"  Directory: {saveDir}");
            Debug.Log($"  Save Exists: {saveExists}");
            Debug.Log($"  Save Size: {saveSize} bytes");
            
            if (Directory.Exists(saveDir))
            {
                string[] files = Directory.GetFiles(saveDir);
                Debug.Log($"  Files in save directory: {files.Length}");
                foreach (string file in files)
                {
                    Debug.Log($"    - {Path.GetFileName(file)}");
                }
            }
        }
    }
}