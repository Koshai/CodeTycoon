using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace CodeTycoon.Core
{
    public class UIManager : MonoBehaviour
    {
        [Header("Main UI References")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private CanvasGroup mainCanvasGroup;
        
        [Header("Dashboard Elements")]
        [SerializeField] private TextMeshProUGUI knowledgePointsText;
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI companyValueText;
        [SerializeField] private TextMeshProUGUI companyStageText;
        [SerializeField] private Button earnKnowledgeButton;
        
        [Header("Learning UI")]
        [SerializeField] private Transform conceptsContainer;
        [SerializeField] private GameObject conceptButtonPrefab;
        
        [Header("Project UI")]
        [SerializeField] private Transform projectsContainer;
        [SerializeField] private GameObject projectCardPrefab;
        
        [Header("Notification System")]
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private GameObject notificationPrefab;
        
        [Header("Screens")]
        [SerializeField] private GameObject mainDashboard;
        [SerializeField] private GameObject learningScreen;
        [SerializeField] private GameObject projectsScreen;
        [SerializeField] private GameObject settingsScreen;
        
        // Current screen tracking
        private GameObject currentScreen;
        
        // Update coroutines
        private Coroutine uiUpdateCoroutine;
        
        public void Initialize()
        {
            Debug.Log("[UIManager] Initializing UI Manager...");
            
            SetupUI();
            ShowScreen(mainDashboard);
            StartUIUpdates();
            
            Debug.Log("[UIManager] UI Manager initialized.");
        }
        
        private void SetupUI()
        {
            // Setup main canvas if not assigned
            if (mainCanvas == null)
            {
                mainCanvas = GetComponentInChildren<Canvas>();
            }
            
            if (mainCanvasGroup == null)
            {
                mainCanvasGroup = mainCanvas.GetComponent<CanvasGroup>();
                if (mainCanvasGroup == null)
                {
                    mainCanvasGroup = mainCanvas.gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            // Earn knowledge button is handled by DashboardUI component
            
            // Initialize all screens as inactive except dashboard
            if (mainDashboard != null) mainDashboard.SetActive(true);
            if (learningScreen != null) learningScreen.SetActive(false);
            if (projectsScreen != null) projectsScreen.SetActive(false);
            if (settingsScreen != null) settingsScreen.SetActive(false);
        }
        
        private void StartUIUpdates()
        {
            if (uiUpdateCoroutine != null)
            {
                StopCoroutine(uiUpdateCoroutine);
            }
            
            uiUpdateCoroutine = StartCoroutine(UpdateUICoroutine());
        }
        
        private IEnumerator UpdateUICoroutine()
        {
            while (true)
            {
                UpdateDashboard();
                yield return new WaitForSeconds(0.1f); // Update 10 times per second
            }
        }
        
        private void UpdateDashboard()
        {
            if (GameManager.Instance == null) return;
            
            GameData gameData = GameManager.Instance.GetGameData();
            if (gameData == null) return;
            
            // Update resource displays
            if (knowledgePointsText != null)
            {
                knowledgePointsText.text = $"Knowledge: {FormatNumber(gameData.knowledgePoints)} KP";
            }
            
            if (cashText != null)
            {
                cashText.text = $"Cash: ${FormatNumber(gameData.cash)}";
            }
            
            if (companyValueText != null)
            {
                companyValueText.text = $"Value: ${FormatNumber(gameData.companyValuation)}";
            }
            
            if (companyStageText != null)
            {
                companyStageText.text = $"Stage: {gameData.GetCompanyStage()}";
            }
        }
        
        private string FormatNumber(double number)
        {
            if (number >= 1000000000)
                return (number / 1000000000).ToString("F1") + "B";
            if (number >= 1000000)
                return (number / 1000000).ToString("F1") + "M";
            if (number >= 1000)
                return (number / 1000).ToString("F1") + "K";
            
            return number.ToString("F0");
        }
        
        // Knowledge point earning is handled by DashboardUI component
        
        public void ShowScreen(GameObject screen)
        {
            // Hide current screen
            if (currentScreen != null)
            {
                currentScreen.SetActive(false);
            }
            
            // Show new screen
            if (screen != null)
            {
                screen.SetActive(true);
                currentScreen = screen;
            }
        }
        
        public void ShowDashboard()
        {
            ShowScreen(mainDashboard);
        }
        
        public void ShowLearningScreen()
        {
            ShowScreen(learningScreen);
            UpdateLearningScreen();
        }
        
        public void ShowProjectsScreen()
        {
            ShowScreen(projectsScreen);
            UpdateProjectsScreen();
        }
        
        public void ShowSettingsScreen()
        {
            ShowScreen(settingsScreen);
        }
        
        private void UpdateLearningScreen()
        {
            // TODO: Update learning screen with available concepts
            Debug.Log("[UIManager] Updating learning screen...");
        }
        
        private void UpdateProjectsScreen()
        {
            // TODO: Update projects screen with available projects
            Debug.Log("[UIManager] Updating projects screen...");
        }
        
        public void ShowNotification(string message, NotificationType type = NotificationType.Info)
        {
            Debug.Log($"[UIManager] Notification: {message}");
            
            // TODO: Implement notification UI system
        }
        
        public void SetUIInteractive(bool interactive)
        {
            if (mainCanvasGroup != null)
            {
                mainCanvasGroup.interactable = interactive;
            }
        }
        
        private void OnDestroy()
        {
            if (uiUpdateCoroutine != null)
            {
                StopCoroutine(uiUpdateCoroutine);
            }
        }
    }
    
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}