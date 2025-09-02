using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Helper script to automatically set up the basic scene structure.
    /// Attach this to an empty GameObject and run in Play Mode to auto-generate the UI.
    /// </summary>
    public class SceneSetupHelper : MonoBehaviour
    {
        [Header("Auto Setup")]
        [SerializeField] private bool setupOnStart = true;
        [SerializeField] private bool createGameManager = true;
        [SerializeField] private bool createCanvas = true;
        [SerializeField] private bool createBasicUI = true;
        
        private void Start()
        {
            if (setupOnStart)
            {
                SetupScene();
            }
        }
        
        [ContextMenu("Setup Scene")]
        public void SetupScene()
        {
            Debug.Log("[SceneSetupHelper] Setting up basic scene structure...");
            
            if (createGameManager)
            {
                CreateGameManager();
            }
            
            if (createCanvas)
            {
                CreateMainCanvas();
            }
            
            if (createBasicUI)
            {
                CreateBasicUI();
            }
            
            Debug.Log("[SceneSetupHelper] Scene setup complete!");
        }
        
        private void CreateGameManager()
        {
            GameObject existingGM = GameObject.Find("GameManager");
            if (existingGM != null)
            {
                Debug.Log("[SceneSetupHelper] GameManager already exists.");
                return;
            }
            
            GameObject gameManagerGO = new GameObject("GameManager");
            gameManagerGO.AddComponent<GameManager>();
            
            // Create child objects for managers
            GameObject uiManagerGO = new GameObject("UIManager");
            uiManagerGO.transform.SetParent(gameManagerGO.transform);
            uiManagerGO.AddComponent<UIManager>();
            
            GameObject companyManagerGO = new GameObject("CompanyManager");
            companyManagerGO.transform.SetParent(gameManagerGO.transform);
            companyManagerGO.AddComponent<CompanyManager>();
            
            GameObject learningSystemGO = new GameObject("LearningSystem");
            learningSystemGO.transform.SetParent(gameManagerGO.transform);
            learningSystemGO.AddComponent<LearningSystem>();
            
            GameObject audioManagerGO = new GameObject("AudioManager");
            audioManagerGO.transform.SetParent(gameManagerGO.transform);
            audioManagerGO.AddComponent<AudioManager>();
            
            Debug.Log("[SceneSetupHelper] Created GameManager with all child managers.");
        }
        
        private void CreateMainCanvas()
        {
            GameObject existingCanvas = GameObject.Find("MainCanvas");
            if (existingCanvas != null)
            {
                Debug.Log("[SceneSetupHelper] Canvas already exists.");
                return;
            }
            
            GameObject canvasGO = new GameObject("MainCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            Debug.Log("[SceneSetupHelper] Created main canvas with proper scaling.");
        }
        
        private void CreateBasicUI()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[SceneSetupHelper] No canvas found! Create canvas first.");
                return;
            }
            
            CreateDashboardScreen(canvas.transform);
            CreateLearningScreen(canvas.transform);
            CreateEventSystem();
            
            Debug.Log("[SceneSetupHelper] Created basic UI screens.");
        }
        
        private void CreateDashboardScreen(Transform parent)
        {
            GameObject dashboardGO = new GameObject("Dashboard");
            dashboardGO.transform.SetParent(parent, false);
            
            RectTransform dashboardRT = dashboardGO.AddComponent<RectTransform>();
            dashboardRT.anchorMin = Vector2.zero;
            dashboardRT.anchorMax = Vector2.one;
            dashboardRT.sizeDelta = Vector2.zero;
            dashboardRT.anchoredPosition = Vector2.zero;
            
            DashboardUI dashboard = dashboardGO.AddComponent<DashboardUI>();
            
            // Create header panel
            GameObject headerGO = CreateUIPanel("Header", dashboardGO.transform);
            RectTransform headerRT = headerGO.GetComponent<RectTransform>();
            headerRT.anchorMin = new Vector2(0, 0.9f);
            headerRT.anchorMax = new Vector2(1, 1f);
            headerRT.sizeDelta = Vector2.zero;
            headerRT.anchoredPosition = Vector2.zero;
            
            // Create resource display
            CreateResourceDisplay(headerGO.transform, dashboard);
            
            // Create main click button
            CreateEarnKnowledgeButton(dashboardGO.transform, dashboard);
            
            // Create navigation buttons
            CreateNavigationButtons(dashboardGO.transform, dashboard);
        }
        
        private void CreateResourceDisplay(Transform parent, DashboardUI dashboard)
        {
            // Knowledge Points
            GameObject kpGO = CreateTextElement("KnowledgePointsText", parent);
            TextMeshProUGUI kpText = kpGO.GetComponent<TextMeshProUGUI>();
            kpText.text = "Knowledge: 0 KP";
            kpText.fontSize = 24;
            dashboard.knowledgePointsText = kpText;
            
            RectTransform kpRT = kpGO.GetComponent<RectTransform>();
            kpRT.anchorMin = new Vector2(0.1f, 0.5f);
            kpRT.anchorMax = new Vector2(0.4f, 1f);
            kpRT.sizeDelta = Vector2.zero;
            kpRT.anchoredPosition = Vector2.zero;
            
            // Cash
            GameObject cashGO = CreateTextElement("CashText", parent);
            TextMeshProUGUI cashText = cashGO.GetComponent<TextMeshProUGUI>();
            cashText.text = "Cash: $100";
            cashText.fontSize = 24;
            dashboard.cashText = cashText;
            
            RectTransform cashRT = cashGO.GetComponent<RectTransform>();
            cashRT.anchorMin = new Vector2(0.4f, 0.5f);
            cashRT.anchorMax = new Vector2(0.7f, 1f);
            cashRT.sizeDelta = Vector2.zero;
            cashRT.anchoredPosition = Vector2.zero;
            
            // Company Stage
            GameObject stageGO = CreateTextElement("CompanyStageText", parent);
            TextMeshProUGUI stageText = stageGO.GetComponent<TextMeshProUGUI>();
            stageText.text = "Stage: Garage";
            stageText.fontSize = 20;
            dashboard.companyStageText = stageText;
            
            RectTransform stageRT = stageGO.GetComponent<RectTransform>();
            stageRT.anchorMin = new Vector2(0.7f, 0.5f);
            stageRT.anchorMax = new Vector2(1f, 1f);
            stageRT.sizeDelta = Vector2.zero;
            stageRT.anchoredPosition = Vector2.zero;
        }
        
        private void CreateEarnKnowledgeButton(Transform parent, DashboardUI dashboard)
        {
            GameObject buttonGO = CreateButton("EarnKnowledgeButton", parent);
            Button button = buttonGO.GetComponent<Button>();
            dashboard.earnKnowledgeButton = button;
            
            RectTransform buttonRT = buttonGO.GetComponent<RectTransform>();
            buttonRT.anchorMin = new Vector2(0.35f, 0.35f);
            buttonRT.anchorMax = new Vector2(0.65f, 0.65f);
            buttonRT.sizeDelta = Vector2.zero;
            buttonRT.anchoredPosition = Vector2.zero;
            
            // Button text
            GameObject textGO = CreateTextElement("ButtonText", buttonGO.transform);
            TextMeshProUGUI buttonText = textGO.GetComponent<TextMeshProUGUI>();
            buttonText.text = "Earn Knowledge";
            buttonText.fontSize = 32;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;
        }
        
        private void CreateNavigationButtons(Transform parent, DashboardUI dashboard)
        {
            GameObject navPanelGO = CreateUIPanel("NavigationPanel", parent);
            RectTransform navRT = navPanelGO.GetComponent<RectTransform>();
            navRT.anchorMin = new Vector2(0, 0);
            navRT.anchorMax = new Vector2(1, 0.1f);
            navRT.sizeDelta = Vector2.zero;
            navRT.anchoredPosition = Vector2.zero;
            
            // Learning Button
            GameObject learningBtnGO = CreateButton("LearningButton", navPanelGO.transform);
            dashboard.learningButton = learningBtnGO.GetComponent<Button>();
            SetButtonPosition(learningBtnGO, 0.1f, 0.3f);
            SetButtonText(learningBtnGO, "Learning");
            
            // Projects Button
            GameObject projectsBtnGO = CreateButton("ProjectsButton", navPanelGO.transform);
            dashboard.projectsButton = projectsBtnGO.GetComponent<Button>();
            SetButtonPosition(projectsBtnGO, 0.35f, 0.55f);
            SetButtonText(projectsBtnGO, "Projects");
            
            // Company Button
            GameObject companyBtnGO = CreateButton("CompanyButton", navPanelGO.transform);
            dashboard.companyButton = companyBtnGO.GetComponent<Button>();
            SetButtonPosition(companyBtnGO, 0.6f, 0.8f);
            SetButtonText(companyBtnGO, "Company");
            
            // Settings Button
            GameObject settingsBtnGO = CreateButton("SettingsButton", navPanelGO.transform);
            dashboard.settingsButton = settingsBtnGO.GetComponent<Button>();
            SetButtonPosition(settingsBtnGO, 0.85f, 1.0f);
            SetButtonText(settingsBtnGO, "Settings");
        }
        
        private void CreateLearningScreen(Transform parent)
        {
            GameObject learningGO = new GameObject("LearningScreen");
            learningGO.transform.SetParent(parent, false);
            learningGO.SetActive(false); // Start inactive
            
            RectTransform learningRT = learningGO.AddComponent<RectTransform>();
            learningRT.anchorMin = Vector2.zero;
            learningRT.anchorMax = Vector2.one;
            learningRT.sizeDelta = Vector2.zero;
            learningRT.anchoredPosition = Vector2.zero;
            
            // Add background
            Image bg = learningGO.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Create title
            GameObject titleGO = CreateTextElement("Title", learningGO.transform);
            TextMeshProUGUI titleText = titleGO.GetComponent<TextMeshProUGUI>();
            titleText.text = "Programming Concepts";
            titleText.fontSize = 36;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 0.9f);
            titleRT.anchorMax = new Vector2(1, 1f);
            titleRT.sizeDelta = Vector2.zero;
            titleRT.anchoredPosition = Vector2.zero;
            
            // Create concept grid
            GameObject scrollGO = new GameObject("ConceptScrollView");
            scrollGO.transform.SetParent(learningGO.transform, false);
            
            RectTransform scrollRT = scrollGO.AddComponent<RectTransform>();
            scrollRT.anchorMin = new Vector2(0.1f, 0.1f);
            scrollRT.anchorMax = new Vector2(0.9f, 0.85f);
            scrollRT.sizeDelta = Vector2.zero;
            scrollRT.anchoredPosition = Vector2.zero;
            
            ScrollRect scroll = scrollGO.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            
            // Create content area
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(scrollGO.transform, false);
            
            RectTransform contentRT = contentGO.AddComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.anchoredPosition = Vector2.zero;
            contentRT.sizeDelta = new Vector2(0, 600); // Will be adjusted by Content Size Fitter
            
            scroll.content = contentRT;
            
            GridLayoutGroup grid = contentGO.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(300, 100);
            grid.spacing = new Vector2(20, 20);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            
            ContentSizeFitter fitter = contentGO.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        private void CreateEventSystem()
        {
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
            {
                Debug.Log("[SceneSetupHelper] EventSystem already exists.");
                return;
            }
            
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            Debug.Log("[SceneSetupHelper] Created EventSystem.");
        }
        
        // Helper methods
        private GameObject CreateUIPanel(string name, Transform parent)
        {
            GameObject panelGO = new GameObject(name);
            panelGO.transform.SetParent(parent, false);
            
            RectTransform rt = panelGO.AddComponent<RectTransform>();
            Image img = panelGO.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            
            return panelGO;
        }
        
        private GameObject CreateButton(string name, Transform parent)
        {
            GameObject buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            
            RectTransform rt = buttonGO.AddComponent<RectTransform>();
            Image img = buttonGO.AddComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.8f, 1f);
            
            Button button = buttonGO.AddComponent<Button>();
            
            return buttonGO;
        }
        
        private GameObject CreateTextElement(string name, Transform parent)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            
            RectTransform rt = textGO.AddComponent<RectTransform>();
            TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
            text.color = Color.white;
            text.fontSize = 18;
            
            return textGO;
        }
        
        private void SetButtonPosition(GameObject button, float minX, float maxX)
        {
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(minX, 0.2f);
            rt.anchorMax = new Vector2(maxX, 0.8f);
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }
        
        private void SetButtonText(GameObject button, string text)
        {
            GameObject textGO = CreateTextElement("Text", button.transform);
            TextMeshProUGUI textComp = textGO.GetComponent<TextMeshProUGUI>();
            textComp.text = text;
            textComp.fontSize = 16;
            textComp.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;
        }
    }
}