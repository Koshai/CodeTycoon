using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Quick utility to fix UI issues without manual setup
    /// </summary>
    public class QuickUIFixer : MonoBehaviour
    {
        [Header("Auto Fix Options")]
        public bool fixOnStart = true;
        public bool addBackButtons = true;
        
        private void Start()
        {
            if (fixOnStart)
            {
                FixUIIssues();
            }
        }
        
        [ContextMenu("Fix UI Issues")]
        public void FixUIIssues()
        {
            Debug.Log("[QuickUIFixer] Fixing UI issues...");
            
            if (addBackButtons)
            {
                AddBackButtonsToScreens();
            }
        }
        
        private void AddBackButtonsToScreens()
        {
            // Find all screens that need back buttons
            string[] screenNames = { "LearningScreen", "ProjectsScreen", "SettingsScreen" };
            
            foreach (string screenName in screenNames)
            {
                GameObject screen = GameObject.Find(screenName);
                if (screen != null)
                {
                    AddBackButtonToScreen(screen);
                }
                else
                {
                    Debug.LogWarning($"[QuickUIFixer] Screen not found: {screenName}");
                }
            }
        }
        
        void AddBackButtonToScreen(GameObject screen)
        {
            // Check if back button already exists
            Transform existingBackButton = screen.transform.Find("BackButton");
            if (existingBackButton != null)
            {
                Debug.Log($"[QuickUIFixer] BackButton already exists in {screen.name}");

                // Make sure it has the right functionality
                Button foundBackButton;
                if (existingBackButton.TryGetComponent<Button>(out foundBackButton))
                {
                    foundBackButton.onClick.RemoveAllListeners();
                    foundBackButton.onClick.AddListener(() =>
                    {
                        UIManager uiManager = FindFirstObjectByType<UIManager>();
                        if (uiManager != null)
                        {
                            uiManager.ShowDashboard();
                        }
                    });
                }
                return;
            }

            Debug.Log($"[QuickUIFixer] Creating BackButton for {screen.name}");

            // Create back button
            GameObject backButtonGO = new GameObject("BackButton");
            backButtonGO.transform.SetParent(screen.transform, false);

            // Set up RectTransform
            RectTransform backRT = backButtonGO.AddComponent<RectTransform>();
            backRT.anchorMin = new Vector2(0.02f, 0.9f);
            backRT.anchorMax = new Vector2(0.15f, 0.98f);
            backRT.sizeDelta = Vector2.zero;
            backRT.anchoredPosition = Vector2.zero;

            // Add Image component
            Image backImage = backButtonGO.AddComponent<Image>();
            backImage.color = new Color(0.2f, 0.2f, 0.8f, 0.8f);

            // Add Button component
            Button backButton = backButtonGO.AddComponent<Button>();
            backButton.onClick.AddListener(() =>
            {
                UIManager uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowDashboard();
                }
            });

            // Create text child
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(backButtonGO.transform, false);

            RectTransform textRT = textGO.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;

            TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = "← Back";
            text.color = Color.white;
            text.fontSize = 16;
            text.alignment = TextAlignmentOptions.Center;

            Debug.Log($"[QuickUIFixer] Created BackButton for {screen.name}");
        }
        
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 160, 200, 100));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Quick UI Fixes");
            
            if (GUILayout.Button("Fix UI Issues"))
            {
                FixUIIssues();
            }
            
            if (GUILayout.Button("Add Back Buttons"))
            {
                AddBackButtonsToScreens();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}