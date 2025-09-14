using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Creates the Challenge Popup UI for the CodingChallengeManager
    /// </summary>
    public class ChallengePopupCreator : MonoBehaviour
    {
        [Header("Auto Create")]
        public bool createOnStart = false;
        
        private void Start()
        {
            if (createOnStart)
            {
                CreateChallengePopup();
            }
        }
        
        [ContextMenu("Create Challenge Popup")]
        public void CreateChallengePopup()
        {
            Debug.Log("[ChallengePopupCreator] Creating challenge popup UI...");
            
            // Find main canvas
            Canvas mainCanvas = FindFirstObjectByType<Canvas>();
            if (mainCanvas == null)
            {
                Debug.LogError("[ChallengePopupCreator] No Canvas found!");
                return;
            }
            
            // Check if popup already exists
            Transform existingPopup = mainCanvas.transform.Find("ChallengePopup");
            if (existingPopup != null)
            {
                Debug.Log("[ChallengePopupCreator] Challenge popup already exists!");
                return;
            }
            
            CreatePopupUI(mainCanvas.transform);
            
            // Connect to CodingChallengeManager
            ConnectToChallengeManager();
            
            Debug.Log("[ChallengePopupCreator] Challenge popup created successfully!");
        }
        
        private void CreatePopupUI(Transform canvasTransform)
        {
            // Create main popup background
            GameObject popupGO = new GameObject("ChallengePopup");
            popupGO.transform.SetParent(canvasTransform, false);
            
            RectTransform popupRT = popupGO.AddComponent<RectTransform>();
            popupRT.anchorMin = Vector2.zero;
            popupRT.anchorMax = Vector2.one;
            popupRT.sizeDelta = Vector2.zero;
            popupRT.anchoredPosition = Vector2.zero;
            
            // Add semi-transparent background
            Image popupBG = popupGO.AddComponent<Image>();
            popupBG.color = new Color(0, 0, 0, 0.7f);
            
            // Create main dialog box
            GameObject dialogGO = new GameObject("Dialog");
            dialogGO.transform.SetParent(popupGO.transform, false);
            
            RectTransform dialogRT = dialogGO.AddComponent<RectTransform>();
            dialogRT.anchorMin = new Vector2(0.15f, 0.15f);
            dialogRT.anchorMax = new Vector2(0.85f, 0.85f);
            dialogRT.sizeDelta = Vector2.zero;
            dialogRT.anchoredPosition = Vector2.zero;
            
            Image dialogBG = dialogGO.AddComponent<Image>();
            dialogBG.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
            
            // Create header
            CreateHeader(dialogGO.transform);
            
            // Create challenge content area
            CreateContentArea(dialogGO.transform);
            
            // Create code input area
            CreateCodeInputArea(dialogGO.transform);
            
            // Create button area
            CreateButtonArea(dialogGO.transform);
            
            // Create feedback area
            CreateFeedbackArea(dialogGO.transform);
            
            // Initially hide popup
            popupGO.SetActive(false);
        }
        
        private void CreateHeader(Transform dialogTransform)
        {
            // Header background
            GameObject headerGO = new GameObject("Header");
            headerGO.transform.SetParent(dialogTransform, false);
            
            RectTransform headerRT = headerGO.AddComponent<RectTransform>();
            headerRT.anchorMin = new Vector2(0, 0.85f);
            headerRT.anchorMax = new Vector2(1, 1f);
            headerRT.sizeDelta = Vector2.zero;
            headerRT.anchoredPosition = Vector2.zero;
            
            Image headerBG = headerGO.AddComponent<Image>();
            headerBG.color = new Color(0.1f, 0.1f, 0.4f, 0.8f);
            
            // Challenge title
            GameObject titleGO = new GameObject("ChallengeTitle");
            titleGO.transform.SetParent(headerGO.transform, false);
            
            RectTransform titleRT = titleGO.AddComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0.05f, 0);
            titleRT.anchorMax = new Vector2(0.85f, 1);
            titleRT.sizeDelta = Vector2.zero;
            titleRT.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "Coding Challenge";
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;
            
            // Close button
            GameObject closeBtnGO = new GameObject("CloseButton");
            closeBtnGO.transform.SetParent(headerGO.transform, false);
            
            RectTransform closeBtnRT = closeBtnGO.AddComponent<RectTransform>();
            closeBtnRT.anchorMin = new Vector2(0.9f, 0.2f);
            closeBtnRT.anchorMax = new Vector2(0.98f, 0.8f);
            closeBtnRT.sizeDelta = Vector2.zero;
            closeBtnRT.anchoredPosition = Vector2.zero;
            
            Image closeBtnImg = closeBtnGO.AddComponent<Image>();
            closeBtnImg.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            
            Button closeBtn = closeBtnGO.AddComponent<Button>();
            
            // Close button text
            GameObject closeTextGO = new GameObject("Text");
            closeTextGO.transform.SetParent(closeBtnGO.transform, false);
            
            RectTransform closeTextRT = closeTextGO.AddComponent<RectTransform>();
            closeTextRT.anchorMin = Vector2.zero;
            closeTextRT.anchorMax = Vector2.one;
            closeTextRT.sizeDelta = Vector2.zero;
            closeTextRT.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI closeText = closeTextGO.AddComponent<TextMeshProUGUI>();
            closeText.text = "✕";
            closeText.fontSize = 16;
            closeText.color = Color.white;
            closeText.alignment = TextAlignmentOptions.Center;
        }
        
        private void CreateContentArea(Transform dialogTransform)
        {
            // Content area
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(dialogTransform, false);
            
            RectTransform contentRT = contentGO.AddComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0.05f, 0.6f);
            contentRT.anchorMax = new Vector2(0.95f, 0.82f);
            contentRT.sizeDelta = Vector2.zero;
            contentRT.anchoredPosition = Vector2.zero;
            
            // Challenge description
            GameObject descGO = new GameObject("ChallengeDescription");
            descGO.transform.SetParent(contentGO.transform, false);
            
            RectTransform descRT = descGO.AddComponent<RectTransform>();
            descRT.anchorMin = Vector2.zero;
            descRT.anchorMax = Vector2.one;
            descRT.sizeDelta = Vector2.zero;
            descRT.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
            descText.text = "Challenge description will appear here...";
            descText.fontSize = 16;
            descText.color = Color.white;
            descText.alignment = TextAlignmentOptions.TopLeft;
            descText.enableWordWrapping = true;
        }
        
        private void CreateCodeInputArea(Transform dialogTransform)
        {
            // Code input label
            GameObject labelGO = new GameObject("CodeInputLabel");
            labelGO.transform.SetParent(dialogTransform, false);
            
            RectTransform labelRT = labelGO.AddComponent<RectTransform>();
            labelRT.anchorMin = new Vector2(0.05f, 0.52f);
            labelRT.anchorMax = new Vector2(0.95f, 0.58f);
            labelRT.sizeDelta = Vector2.zero;
            labelRT.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI labelText = labelGO.AddComponent<TextMeshProUGUI>();
            labelText.text = "💻 Your Code:";
            labelText.fontSize = 14;
            labelText.fontStyle = FontStyles.Bold;
            labelText.color = Color.cyan;
            labelText.alignment = TextAlignmentOptions.MidlineLeft;
            
            // Code input field
            GameObject inputGO = new GameObject("CodeInputField");
            inputGO.transform.SetParent(dialogTransform, false);
            
            RectTransform inputRT = inputGO.AddComponent<RectTransform>();
            inputRT.anchorMin = new Vector2(0.05f, 0.25f);
            inputRT.anchorMax = new Vector2(0.95f, 0.5f);
            inputRT.sizeDelta = Vector2.zero;
            inputRT.anchoredPosition = Vector2.zero;
            
            Image inputBG = inputGO.AddComponent<Image>();
            inputBG.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            TMP_InputField inputField = inputGO.AddComponent<TMP_InputField>();
            inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
            
            // Input field text area
            GameObject textAreaGO = new GameObject("Text Area");
            textAreaGO.transform.SetParent(inputGO.transform, false);
            
            RectTransform textAreaRT = textAreaGO.AddComponent<RectTransform>();
            textAreaRT.anchorMin = Vector2.zero;
            textAreaRT.anchorMax = Vector2.one;
            textAreaRT.sizeDelta = new Vector2(-10, -10);
            textAreaRT.anchoredPosition = Vector2.zero;
            
            RectMask2D mask = textAreaGO.AddComponent<RectMask2D>();
            
            // Input field text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(textAreaGO.transform, false);
            
            RectTransform textRT = textGO.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI inputText = textGO.AddComponent<TextMeshProUGUI>();
            inputText.text = "";
            inputText.fontSize = 14;
            inputText.color = Color.white;
            inputText.alignment = TextAlignmentOptions.TopLeft;
            inputText.enableWordWrapping = true;
            
            // Placeholder
            GameObject placeholderGO = new GameObject("Placeholder");
            placeholderGO.transform.SetParent(textAreaGO.transform, false);
            
            RectTransform placeholderRT = placeholderGO.AddComponent<RectTransform>();
            placeholderRT.anchorMin = Vector2.zero;
            placeholderRT.anchorMax = Vector2.one;
            placeholderRT.sizeDelta = Vector2.zero;
            placeholderRT.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI placeholderText = placeholderGO.AddComponent<TextMeshProUGUI>();
            placeholderText.text = "Type your code here...";
            placeholderText.fontSize = 14;
            placeholderText.color = Color.gray;
            placeholderText.alignment = TextAlignmentOptions.TopLeft;
            placeholderText.enableWordWrapping = true;
            
            // Connect input field components
            inputField.textViewport = textAreaRT;
            inputField.textComponent = inputText;
            inputField.placeholder = placeholderText;
        }
        
        private void CreateButtonArea(Transform dialogTransform)
        {
            // Button area
            GameObject buttonAreaGO = new GameObject("ButtonArea");
            buttonAreaGO.transform.SetParent(dialogTransform, false);
            
            RectTransform buttonAreaRT = buttonAreaGO.AddComponent<RectTransform>();
            buttonAreaRT.anchorMin = new Vector2(0.05f, 0.15f);
            buttonAreaRT.anchorMax = new Vector2(0.95f, 0.23f);
            buttonAreaRT.sizeDelta = Vector2.zero;
            buttonAreaRT.anchoredPosition = Vector2.zero;
            
            // Submit button
            GameObject submitBtnGO = new GameObject("SubmitButton");
            submitBtnGO.transform.SetParent(buttonAreaGO.transform, false);
            
            RectTransform submitBtnRT = submitBtnGO.AddComponent<RectTransform>();
            submitBtnRT.anchorMin = new Vector2(0.6f, 0);
            submitBtnRT.anchorMax = new Vector2(0.9f, 1);
            submitBtnRT.sizeDelta = Vector2.zero;
            submitBtnRT.anchoredPosition = Vector2.zero;
            
            Image submitBtnImg = submitBtnGO.AddComponent<Image>();
            submitBtnImg.color = new Color(0.2f, 0.7f, 0.2f, 0.8f);
            
            Button submitBtn = submitBtnGO.AddComponent<Button>();
            
            // Submit button text
            GameObject submitTextGO = new GameObject("Text");
            submitTextGO.transform.SetParent(submitBtnGO.transform, false);
            
            RectTransform submitTextRT = submitTextGO.AddComponent<RectTransform>();
            submitTextRT.anchorMin = Vector2.zero;
            submitTextRT.anchorMax = Vector2.one;
            submitTextRT.sizeDelta = Vector2.zero;
            submitTextRT.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI submitText = submitTextGO.AddComponent<TextMeshProUGUI>();
            submitText.text = "🚀 Submit Code";
            submitText.fontSize = 14;
            submitText.fontStyle = FontStyles.Bold;
            submitText.color = Color.white;
            submitText.alignment = TextAlignmentOptions.Center;
        }
        
        private void CreateFeedbackArea(Transform dialogTransform)
        {
            // Feedback area
            GameObject feedbackGO = new GameObject("Feedback");
            feedbackGO.transform.SetParent(dialogTransform, false);
            
            RectTransform feedbackRT = feedbackGO.AddComponent<RectTransform>();
            feedbackRT.anchorMin = new Vector2(0.05f, 0.02f);
            feedbackRT.anchorMax = new Vector2(0.95f, 0.13f);
            feedbackRT.sizeDelta = Vector2.zero;
            feedbackRT.anchoredPosition = Vector2.zero;
            
            Image feedbackBG = feedbackGO.AddComponent<Image>();
            feedbackBG.color = new Color(0.3f, 0.3f, 0.1f, 0.3f);
            
            TextMeshProUGUI feedbackText = feedbackGO.AddComponent<TextMeshProUGUI>();
            feedbackText.text = "💡 Hints and feedback will appear here...";
            feedbackText.fontSize = 12;
            feedbackText.color = Color.yellow;
            feedbackText.alignment = TextAlignmentOptions.TopLeft;
            feedbackText.enableWordWrapping = true;
        }
        
        private void ConnectToChallengeManager()
        {
            CodingChallengeManager challengeManager = FindFirstObjectByType<CodingChallengeManager>();
            if (challengeManager == null)
            {
                Debug.LogWarning("[ChallengePopupCreator] CodingChallengeManager not found! Create one first.");
                return;
            }
            
            // Find the created UI elements
            Canvas mainCanvas = FindFirstObjectByType<Canvas>();
            Transform popup = mainCanvas.transform.Find("ChallengePopup");
            
            if (popup != null)
            {
                // Get references to UI components
                GameObject challengePopup = popup.gameObject;
                TextMeshProUGUI titleText = popup.Find("Dialog/Header/ChallengeTitle")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI descText = popup.Find("Dialog/Content/ChallengeDescription")?.GetComponent<TextMeshProUGUI>();
                TMP_InputField codeInput = popup.Find("Dialog/CodeInputField")?.GetComponent<TMP_InputField>();
                Button submitBtn = popup.Find("Dialog/ButtonArea/SubmitButton")?.GetComponent<Button>();
                Button closeBtn = popup.Find("Dialog/Header/CloseButton")?.GetComponent<Button>();
                TextMeshProUGUI feedbackText = popup.Find("Dialog/Feedback")?.GetComponent<TextMeshProUGUI>();
                
                // Use reflection to set private fields in CodingChallengeManager
                var challengeManagerType = typeof(CodingChallengeManager);
                
                challengeManagerType.GetField("challengePopup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(challengeManager, challengePopup);
                challengeManagerType.GetField("challengeTitleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(challengeManager, titleText);
                challengeManagerType.GetField("challengeDescriptionText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(challengeManager, descText);
                challengeManagerType.GetField("codeInputField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(challengeManager, codeInput);
                challengeManagerType.GetField("submitButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(challengeManager, submitBtn);
                challengeManagerType.GetField("closeButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(challengeManager, closeBtn);
                challengeManagerType.GetField("feedbackText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(challengeManager, feedbackText);
                
                Debug.Log("[ChallengePopupCreator] Connected UI components to CodingChallengeManager!");
            }
        }
        
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 560, 200, 80));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Challenge UI Creator");
            
            if (GUILayout.Button("Create Challenge UI"))
            {
                CreateChallengePopup();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}