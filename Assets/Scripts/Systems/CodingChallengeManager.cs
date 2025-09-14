using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Manages coding challenges for each programming concept
    /// </summary>
    public class CodingChallengeManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject challengePopup;
        [SerializeField] private TextMeshProUGUI challengeTitleText;
        [SerializeField] private TextMeshProUGUI challengeDescriptionText;
        [SerializeField] private TMP_InputField codeInputField;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI feedbackText;
        
        private Dictionary<string, List<CodingChallenge>> conceptChallenges;
        private string currentConceptId;
        private CodingChallenge currentChallenge;
        
        public void Initialize()
        {
            Debug.Log("[CodingChallengeManager] Initializing...");
            
            InitializeChallenges();
            SetupUI();
            
            Debug.Log("[CodingChallengeManager] Initialized with challenges for concepts.");
        }
        
        private void InitializeChallenges()
        {
            conceptChallenges = new Dictionary<string, List<CodingChallenge>>();
            
            // Variables challenges
            conceptChallenges["Variables"] = new List<CodingChallenge>
            {
                new CodingChallenge(
                    "Create a Variable",
                    "Declare a variable called 'playerName' and assign it the value 'Alex'",
                    new[] { "playerName", "=", "Alex", "var", "let", "const" },
                    "Variables store data that can be used later in your program."
                ),
                new CodingChallenge(
                    "Number Variable",
                    "Create a variable called 'score' and set it to 100",
                    new[] { "score", "=", "100" },
                    "Numbers don't need quotes - just write the number directly."
                ),
                new CodingChallenge(
                    "Update a Variable",
                    "Change the 'score' variable to 150",
                    new[] { "score", "=", "150" },
                    "You can change a variable's value by assigning it a new value."
                )
            };
            
            // Functions challenges
            conceptChallenges["Functions"] = new List<CodingChallenge>
            {
                new CodingChallenge(
                    "Create a Function",
                    "Write a function called 'sayHello' that prints 'Hello World!'",
                    new[] { "function", "sayHello", "Hello World", "console.log", "print" },
                    "Functions are reusable blocks of code that perform specific tasks."
                ),
                new CodingChallenge(
                    "Function with Parameters",
                    "Create a function called 'greet' that takes a name parameter and says hello to that person",
                    new[] { "function", "greet", "name", "parameter" },
                    "Parameters let functions work with different input values."
                )
            };
            
            // Loops challenges
            conceptChallenges["Loops"] = new List<CodingChallenge>
            {
                new CodingChallenge(
                    "For Loop",
                    "Write a for loop that prints numbers 1 to 5",
                    new[] { "for", "i", "1", "5", "<=", "<" },
                    "Loops repeat code multiple times, saving you from writing the same thing over and over."
                ),
                new CodingChallenge(
                    "While Loop",
                    "Create a while loop that counts down from 10 to 1",
                    new[] { "while", "10", "1", "--", ">" },
                    "While loops continue running as long as a condition is true."
                )
            };
            
            // Add more concepts as needed
            conceptChallenges["HTML"] = new List<CodingChallenge>
            {
                new CodingChallenge(
                    "Basic HTML Structure",
                    "Create a basic HTML page with a title and a paragraph",
                    new[] { "<html>", "<head>", "<title>", "<body>", "<p>", "</html>" },
                    "HTML structures web pages using tags that define different elements."
                )
            };
        }
        
        private void SetupUI()
        {
            if (submitButton != null)
            {
                submitButton.onClick.RemoveAllListeners();
                submitButton.onClick.AddListener(OnSubmitCode);
            }
            
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseChallenge);
            }
            
            // Initially hide challenge popup
            if (challengePopup != null)
            {
                challengePopup.SetActive(false);
            }
        }
        
        public void OpenChallenge(string conceptId)
        {
            Debug.Log($"[CodingChallengeManager] Opening challenge for: {conceptId}");
            
            // Check if manager is properly initialized
            if (conceptChallenges == null)
            {
                Debug.LogError("[CodingChallengeManager] Manager not initialized! Call Initialize() first.");
                return;
            }
            
            if (!conceptChallenges.ContainsKey(conceptId))
            {
                Debug.LogWarning($"[CodingChallengeManager] No challenges found for concept: {conceptId}");
                return;
            }
            
            currentConceptId = conceptId;
            
            // Get next uncompleted challenge for this concept
            currentChallenge = GetNextChallenge(conceptId);
            
            if (currentChallenge == null)
            {
                ShowConceptCompleted(conceptId);
                return;
            }
            
            DisplayChallenge();
        }
        
        private CodingChallenge GetNextChallenge(string conceptId)
        {
            var challenges = conceptChallenges[conceptId];
            
            // For now, just return the first challenge
            // Later: track completed challenges and return next uncompleted one
            return challenges.FirstOrDefault();
        }
        
        private void DisplayChallenge()
        {
            if (challengePopup != null)
                challengePopup.SetActive(true);
            
            if (challengeTitleText != null)
                challengeTitleText.text = currentChallenge.title;
            
            if (challengeDescriptionText != null)
                challengeDescriptionText.text = currentChallenge.description;
            
            if (codeInputField != null)
            {
                codeInputField.text = "";
                codeInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Type your code here...";
            }
            
            if (feedbackText != null)
            {
                feedbackText.text = "💡 " + currentChallenge.hint;
                feedbackText.color = Color.gray;
            }
        }
        
        private void OnSubmitCode()
        {
            if (currentChallenge == null || codeInputField == null) return;
            
            string userCode = codeInputField.text.Trim();
            
            if (string.IsNullOrEmpty(userCode))
            {
                ShowFeedback("❌ Please enter some code!", Color.red);
                return;
            }
            
            bool isCorrect = ValidateCode(userCode, currentChallenge);
            
            if (isCorrect)
            {
                ShowFeedback("✅ Great job! Challenge completed!", Color.green);
                OnChallengeCompleted();
            }
            else
            {
                ShowFeedback("❌ Not quite right. Try again! Check the hint below.", Color.red);
            }
        }
        
        private bool ValidateCode(string userCode, CodingChallenge challenge)
        {
            string codeLower = userCode.ToLower().Trim();
            
            // Use challenge-specific validation based on concept
            switch (currentConceptId)
            {
                case "Variables":
                    return ValidateVariableChallenge(codeLower, challenge);
                case "Functions":
                    return ValidateFunctionChallenge(codeLower, challenge);
                case "Loops":
                    return ValidateLoopChallenge(codeLower, challenge);
                case "HTML":
                    return ValidateHTMLChallenge(codeLower, challenge);
                default:
                    return ValidateGeneric(codeLower, challenge);
            }
        }
        
        private bool ValidateVariableChallenge(string code, CodingChallenge challenge)
        {
            // Check specific patterns for variable challenges
            if (challenge.title == "Create a Variable")
            {
                // Must have: variable declaration, playerName, =, "Alex" (with quotes)
                bool hasDeclaration = code.Contains("var ") || code.Contains("let ") || code.Contains("const ");
                bool hasVariableName = code.Contains("playername");
                bool hasAssignment = code.Contains("=");
                bool hasCorrectValue = code.Contains("\"alex\"") || code.Contains("'alex'");
                
                // Check for common mistakes
                if (code.Contains("int ") || code.Contains("string "))
                {
                    ShowDetailedFeedback("❌ Hint: In JavaScript, use 'var', 'let', or 'const' instead of 'int' or 'string'");
                    return false;
                }
                
                if (hasVariableName && hasAssignment && !hasCorrectValue)
                {
                    ShowDetailedFeedback("❌ Hint: String values need quotes around them. Try \"Alex\" or 'Alex'");
                    return false;
                }
                
                if (!hasVariableName)
                {
                    ShowDetailedFeedback("❌ Hint: The variable must be called 'playerName'");
                    return false;
                }
                
                return hasDeclaration && hasVariableName && hasAssignment && hasCorrectValue;
            }
            else if (challenge.title == "Number Variable")
            {
                // Must have: score, =, 100 (no quotes)
                bool hasVariableName = code.Contains("score");
                bool hasAssignment = code.Contains("=");
                bool hasCorrectValue = code.Contains("100") && !code.Contains("\"100\"") && !code.Contains("'100'");
                
                if (code.Contains("\"100\"") || code.Contains("'100'"))
                {
                    ShowDetailedFeedback("❌ Hint: Numbers don't need quotes. Use 100, not \"100\"");
                    return false;
                }
                
                return hasVariableName && hasAssignment && hasCorrectValue;
            }
            else if (challenge.title == "Update a Variable")
            {
                // Must have: score, =, 150
                bool hasVariableName = code.Contains("score");
                bool hasAssignment = code.Contains("=");
                bool hasCorrectValue = code.Contains("150");
                
                return hasVariableName && hasAssignment && hasCorrectValue;
            }
            
            return false;
        }
        
        private bool ValidateFunctionChallenge(string code, CodingChallenge challenge)
        {
            if (challenge.title == "Create a Function")
            {
                bool hasFunction = code.Contains("function");
                bool hasFunctionName = code.Contains("sayhello");
                bool hasConsoleLog = code.Contains("console.log") || code.Contains("print");
                bool hasCorrectText = code.Contains("hello world");
                
                return hasFunction && hasFunctionName && hasConsoleLog && hasCorrectText;
            }
            
            return ValidateGeneric(code, challenge);
        }
        
        private bool ValidateLoopChallenge(string code, CodingChallenge challenge)
        {
            if (challenge.title == "For Loop")
            {
                bool hasFor = code.Contains("for");
                bool hasVariable = code.Contains("i");
                bool hasCondition = code.Contains("<") || code.Contains("<=");
                
                return hasFor && hasVariable && hasCondition;
            }
            else if (challenge.title == "While Loop")
            {
                bool hasWhile = code.Contains("while");
                bool hasCondition = code.Contains(">") || code.Contains(">=");
                
                return hasWhile && hasCondition;
            }
            
            return ValidateGeneric(code, challenge);
        }
        
        private bool ValidateHTMLChallenge(string code, CodingChallenge challenge)
        {
            if (challenge.title == "Basic HTML Structure")
            {
                bool hasHTML = code.Contains("<html>") && code.Contains("</html>");
                bool hasHead = code.Contains("<head>") && code.Contains("</head>");
                bool hasBody = code.Contains("<body>") && code.Contains("</body>");
                bool hasParagraph = code.Contains("<p>") && code.Contains("</p>");
                
                return hasHTML && hasHead && hasBody && hasParagraph;
            }
            
            return ValidateGeneric(code, challenge);
        }
        
        private bool ValidateGeneric(string code, CodingChallenge challenge)
        {
            // Fallback to original keyword matching with higher threshold
            int matchCount = 0;
            foreach (string keyword in challenge.requiredKeywords)
            {
                if (code.Contains(keyword.ToLower()))
                {
                    matchCount++;
                }
            }
            
            float matchPercentage = (float)matchCount / challenge.requiredKeywords.Length;
            return matchPercentage >= 0.7f; // Increased from 50% to 70%
        }
        
        private void ShowDetailedFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
                feedbackText.color = Color.red;
            }
        }
        
        private void ShowFeedback(string message, Color color)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
                feedbackText.color = color;
            }
        }
        
        private void OnChallengeCompleted()
        {
            Debug.Log($"[CodingChallengeManager] Challenge completed for: {currentConceptId}");
            
            // Award progress to the concept
            if (GameManager.Instance != null)
            {
                LearningSystem learningSystem = FindFirstObjectByType<LearningSystem>();
                if (learningSystem != null)
                {
                    learningSystem.CompleteCodingChallenge(currentConceptId, true, 1.0f);
                }
            }
            
            // Close challenge after a delay
            Invoke(nameof(CloseChallenge), 2f);
        }
        
        private void ShowConceptCompleted(string conceptId)
        {
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowNotification($"🎉 All challenges completed for {conceptId}!", NotificationType.Success);
            }
        }
        
        public void CloseChallenge()
        {
            if (challengePopup != null)
                challengePopup.SetActive(false);
            
            currentChallenge = null;
            currentConceptId = null;
        }
        
        private void Update()
        {
            // Handle Escape key to close challenge
            if (Input.GetKeyDown(KeyCode.Escape) && challengePopup != null && challengePopup.activeInHierarchy)
            {
                CloseChallenge();
            }
        }
    }
    
    [System.Serializable]
    public class CodingChallenge
    {
        public string title;
        public string description;
        public string[] requiredKeywords;
        public string hint;
        
        public CodingChallenge(string challengeTitle, string challengeDescription, string[] keywords, string challengeHint)
        {
            title = challengeTitle;
            description = challengeDescription;
            requiredKeywords = keywords;
            hint = challengeHint;
        }
    }
}