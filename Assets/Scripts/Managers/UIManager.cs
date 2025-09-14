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
            // Create enhanced concept button UI
            GameObject buttonGO = new GameObject($"Concept_{conceptId}");
            buttonGO.transform.SetParent(parent, false);
            
            // Add RectTransform - let Grid Layout control the sizing
            RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
            // Don't set sizeDelta - let GridLayoutGroup handle it
            
            // Add background image
            UnityEngine.UI.Image bg = buttonGO.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0.2f, 0.2f, 0.5f, 0.8f);
            
            // Add main button
            UnityEngine.UI.Button mainButton = buttonGO.AddComponent<UnityEngine.UI.Button>();
            
            // Create concept name text
            GameObject nameTextGO = new GameObject("ConceptName");
            nameTextGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform nameTextRT = nameTextGO.AddComponent<RectTransform>();
            nameTextRT.anchorMin = new Vector2(0, 0.6f);
            nameTextRT.anchorMax = new Vector2(1, 1);
            nameTextRT.sizeDelta = Vector2.zero;
            nameTextRT.anchoredPosition = Vector2.zero;
            
            TMPro.TextMeshProUGUI nameText = nameTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            nameText.text = conceptId;
            nameText.color = Color.white;
            nameText.fontSize = 16;
            nameText.fontStyle = TMPro.FontStyles.Bold;
            nameText.alignment = TMPro.TextAlignmentOptions.Center;
            
            // Create status text
            GameObject statusTextGO = new GameObject("StatusText");
            statusTextGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform statusTextRT = statusTextGO.AddComponent<RectTransform>();
            statusTextRT.anchorMin = new Vector2(0, 0.3f);
            statusTextRT.anchorMax = new Vector2(1, 0.6f);
            statusTextRT.sizeDelta = Vector2.zero;
            statusTextRT.anchoredPosition = Vector2.zero;
            
            TMPro.TextMeshProUGUI statusText = statusTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            statusText.text = $"Cost: {learningSystem.GetConceptCost(conceptId):F0} KP";
            statusText.color = Color.white;
            statusText.fontSize = 12;
            statusText.alignment = TMPro.TextAlignmentOptions.Center;
            
            // Create progress bar background
            GameObject progressBGGO = new GameObject("ProgressBackground");
            progressBGGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform progressBGRT = progressBGGO.AddComponent<RectTransform>();
            progressBGRT.anchorMin = new Vector2(0.1f, 0.1f);
            progressBGRT.anchorMax = new Vector2(0.9f, 0.25f);
            progressBGRT.sizeDelta = Vector2.zero;
            progressBGRT.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Image progressBG = progressBGGO.AddComponent<UnityEngine.UI.Image>();
            progressBG.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            
            // Create progress bar fill
            GameObject progressFillGO = new GameObject("ProgressFill");
            progressFillGO.transform.SetParent(progressBGGO.transform, false);
            
            RectTransform progressFillRT = progressFillGO.AddComponent<RectTransform>();
            progressFillRT.anchorMin = Vector2.zero;
            progressFillRT.anchorMax = Vector2.one;
            progressFillRT.sizeDelta = Vector2.zero;
            progressFillRT.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Image progressFill = progressFillGO.AddComponent<UnityEngine.UI.Image>();
            progressFill.color = Color.white;
            progressFill.type = UnityEngine.UI.Image.Type.Filled;
            progressFill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            progressFill.fillAmount = 0f;
            
            // Create challenge button (initially hidden)
            GameObject challengeBtnGO = new GameObject("ChallengeButton");
            challengeBtnGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform challengeBtnRT = challengeBtnGO.AddComponent<RectTransform>();
            challengeBtnRT.anchorMin = new Vector2(0.7f, 0.7f);
            challengeBtnRT.anchorMax = new Vector2(0.95f, 0.95f);
            challengeBtnRT.sizeDelta = Vector2.zero;
            challengeBtnRT.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Image challengeBtnImg = challengeBtnGO.AddComponent<UnityEngine.UI.Image>();
            challengeBtnImg.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            
            UnityEngine.UI.Button challengeBtn = challengeBtnGO.AddComponent<UnityEngine.UI.Button>();
            challengeBtnGO.SetActive(false); // Initially hidden
            
            // Add challenge button text
            GameObject challengeTextGO = new GameObject("Text");
            challengeTextGO.transform.SetParent(challengeBtnGO.transform, false);
            
            RectTransform challengeTextRT = challengeTextGO.AddComponent<RectTransform>();
            challengeTextRT.anchorMin = Vector2.zero;
            challengeTextRT.anchorMax = Vector2.one;
            challengeTextRT.sizeDelta = Vector2.zero;
            challengeTextRT.anchoredPosition = Vector2.zero;
            
            TMPro.TextMeshProUGUI challengeText = challengeTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            challengeText.text = "⚡";
            challengeText.color = Color.white;
            challengeText.fontSize = 10;
            challengeText.alignment = TMPro.TextAlignmentOptions.Center;
            
            // Add EnhancedConceptButton component
            EnhancedConceptButton conceptButton = buttonGO.AddComponent<EnhancedConceptButton>();
            conceptButton.mainButton = mainButton;
            conceptButton.conceptNameText = nameText;
            conceptButton.statusText = statusText;
            conceptButton.backgroundImage = bg;
            conceptButton.progressFillImage = progressFill;
            conceptButton.challengeButton = challengeBtn;
            
            // Get concept from learning system and setup
            ProgrammingConcept concept = learningSystem.GetConcept(conceptId);
            conceptButton.Setup(conceptId, concept, learningSystem);
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