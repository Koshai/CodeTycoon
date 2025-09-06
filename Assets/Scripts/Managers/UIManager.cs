using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
            SetupBackButton(learningScreen);
        }
        
        public void ShowProjectsScreen()
        {
            ShowScreen(projectsScreen);
            UpdateProjectsScreen();
            SetupBackButton(projectsScreen);
        }
        
        public void ShowSettingsScreen()
        {
            ShowScreen(settingsScreen);
            SetupBackButton(settingsScreen);
        }
        
        private void SetupBackButton(GameObject screen)
        {
            if (screen == null) return;
            
            // Find back button in the screen
            Transform backButtonTransform = screen.transform.Find("BackButton");
            if (backButtonTransform != null)
            {
                UnityEngine.UI.Button backButton = backButtonTransform.GetComponent<UnityEngine.UI.Button>();
                if (backButton != null)
                {
                    // Clear existing listeners and add back functionality
                    backButton.onClick.RemoveAllListeners();
                    backButton.onClick.AddListener(() => ShowDashboard());
                }
            }
            else
            {
                Debug.LogWarning($"[UIManager] No BackButton found in {screen.name}");
            }
        }
        
        private void UpdateLearningScreen()
        {
            Debug.Log("[UIManager] Updating learning screen...");
            
            // Find the concept scroll view content area
            if (learningScreen != null)
            {
                Transform scrollView = learningScreen.transform.Find("ConceptScrollView");
                if (scrollView != null)
                {
                    Transform content = scrollView.Find("Viewport/Content");
                    if (content == null)
                    {
                        content = scrollView.Find("Content"); // Fallback
                    }
                    
                    if (content != null)
                    {
                        PopulateConceptButtons(content);
                    }
                    else
                    {
                        Debug.LogWarning("[UIManager] Could not find Content area in ConceptScrollView");
                    }
                }
            }
        }
        
        private void PopulateConceptButtons(Transform contentArea)
        {
            // Clear existing buttons
            foreach (Transform child in contentArea)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
            
            // Get learning system
            LearningSystem learningSystem = FindFirstObjectByType<LearningSystem>();
            if (learningSystem == null) 
            {
                Debug.LogError("[UIManager] LearningSystem not found!");
                return;
            }
            
            var gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) 
            {
                Debug.LogError("[UIManager] GameData not found!");
                return;
            }
            
            // Get all concepts from learning system
            var allConceptIds = learningSystem.GetAllConceptIds();
            
            // Define the order we want to display them
            var desiredOrder = new[] { "Variables", "Functions", "Loops", "Debugging", "OOP", "HTML", "CSS", "JavaScript" };
            
            // Show concepts in desired order first, then any others
            var conceptsToShow = new List<string>();
            
            // Add concepts in preferred order if they exist
            foreach (string conceptId in desiredOrder)
            {
                if (allConceptIds.Contains(conceptId))
                {
                    conceptsToShow.Add(conceptId);
                }
            }
            
            // Add any remaining concepts not in our ordered list
            foreach (string conceptId in allConceptIds)
            {
                if (!conceptsToShow.Contains(conceptId))
                {
                    conceptsToShow.Add(conceptId);
                }
            }
            
            Debug.Log($"[UIManager] Creating {conceptsToShow.Count} concept buttons from learning system...");
            Debug.Log($"[UIManager] Concepts to show: {string.Join(", ", conceptsToShow)}");
            
            foreach (string conceptId in conceptsToShow)
            {
                CreateConceptButton(conceptId, learningSystem, contentArea);
            }
            
            Debug.Log($"[UIManager] Created concept buttons. Content area has {contentArea.childCount} children.");
        }
        
        private void CreateConceptButton(string conceptId, LearningSystem learningSystem, Transform parent)
        {
            // Create simple concept button UI
            GameObject buttonGO = new GameObject($"Concept_{conceptId}");
            buttonGO.transform.SetParent(parent, false);
            
            // Add RectTransform - let Grid Layout control the sizing
            RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
            // Don't set sizeDelta - let GridLayoutGroup handle it
            
            // Add UI components
            UnityEngine.UI.Image bg = buttonGO.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0.2f, 0.2f, 0.5f, 0.8f);
            
            UnityEngine.UI.Button button = buttonGO.AddComponent<UnityEngine.UI.Button>();
            
            // Add text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform textRT = textGO.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;
            
            TMPro.TextMeshProUGUI text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = $"{conceptId}\nCost: {learningSystem.GetConceptCost(conceptId):F0} KP";
            text.color = Color.white;
            text.fontSize = 14;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            
            // Add click functionality
            button.onClick.AddListener(() => {
                if (learningSystem.CanUnlockConcept(conceptId))
                {
                    if (learningSystem.TryUnlockConcept(conceptId))
                    {
                        ShowNotification($"Unlocked: {conceptId}!", NotificationType.Success);
                        UpdateLearningScreen(); // Refresh the display
                    }
                    else
                    {
                        ShowNotification("Not enough Knowledge Points!", NotificationType.Error);
                    }
                }
                else
                {
                    var missing = learningSystem.GetMissingPrerequisites(conceptId);
                    if (missing.Count > 0)
                    {
                        ShowNotification($"Prerequisites needed: {string.Join(", ", missing)}", NotificationType.Info);
                    }
                    else
                    {
                        ShowNotification("Not enough Knowledge Points!", NotificationType.Error);
                    }
                }
            });
            
            // Color coding based on availability
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData != null)
            {
                if (gameData.learningProgress.IsConceptMastered(conceptId))
                {
                    bg.color = Color.yellow; // Mastered
                }
                else if (gameData.learningProgress.IsConceptUnlocked(conceptId))
                {
                    bg.color = Color.green; // Unlocked
                }
                else if (learningSystem.CanUnlockConcept(conceptId))
                {
                    bg.color = Color.blue; // Available
                }
                else
                {
                    bg.color = Color.gray; // Locked
                }
            }
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