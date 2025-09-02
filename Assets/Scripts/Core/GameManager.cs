using UnityEngine;
using System.Collections;

namespace CodeTycoon.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        [SerializeField] private GameData gameData;
        [SerializeField] private bool debugMode = false;
        
        [Header("Managers")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private CompanyManager companyManager;
        [SerializeField] private LearningSystem learningSystem;
        [SerializeField] private AudioManager audioManager;
        
        // Singleton pattern
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        // Events
        public delegate void GameStateChanged(GameState newState);
        public static event GameStateChanged OnGameStateChanged;
        
        private GameState currentState = GameState.Loading;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            Debug.Log("[GameManager] Initializing game systems...");
            
            StartCoroutine(InitializeGameSystems());
        }
        
        private IEnumerator InitializeGameSystems()
        {
            // Initialize core data
            if (gameData == null)
            {
                gameData = new GameData();
            }
            
            // Initialize managers in order
            yield return StartCoroutine(InitializeManager("AudioManager", () => audioManager?.Initialize()));
            yield return StartCoroutine(InitializeManager("CompanyManager", () => companyManager?.Initialize()));
            yield return StartCoroutine(InitializeManager("LearningSystem", () => learningSystem?.Initialize()));
            yield return StartCoroutine(InitializeManager("UIManager", () => uiManager?.Initialize()));
            
            // Load save data
            LoadGame();
            
            // Game is ready
            SetGameState(GameState.Playing);
            
            Debug.Log("[GameManager] Game initialization complete!");
        }
        
        private IEnumerator InitializeManager(string managerName, System.Action initAction)
        {
            Debug.Log($"[GameManager] Initializing {managerName}...");
            
            try
            {
                initAction?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameManager] Failed to initialize {managerName}: {e.Message}");
            }
            
            yield return null; // Wait one frame between initializations
        }
        
        public void SetGameState(GameState newState)
        {
            if (currentState != newState)
            {
                GameState oldState = currentState;
                currentState = newState;
                
                Debug.Log($"[GameManager] Game state changed: {oldState} -> {newState}");
                OnGameStateChanged?.Invoke(newState);
            }
        }
        
        public GameState GetGameState()
        {
            return currentState;
        }
        
        public GameData GetGameData()
        {
            return gameData;
        }
        
        public void SaveGame()
        {
            Debug.Log("[GameManager] Saving game...");
            SaveSystem.SaveGame(gameData);
        }
        
        public void LoadGame()
        {
            Debug.Log("[GameManager] Loading game...");
            GameData loadedData = SaveSystem.LoadGame();
            
            if (loadedData != null)
            {
                gameData = loadedData;
                Debug.Log("[GameManager] Game loaded successfully!");
            }
            else
            {
                Debug.Log("[GameManager] No save file found, starting new game.");
                StartNewGame();
            }
        }
        
        public void StartNewGame()
        {
            Debug.Log("[GameManager] Starting new game...");
            gameData = new GameData();
            gameData.Initialize();
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGame();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveGame();
            }
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                SaveGame();
            }
        }
    }
    
    public enum GameState
    {
        Loading,
        MainMenu,
        Playing,
        Paused,
        Settings
    }
}