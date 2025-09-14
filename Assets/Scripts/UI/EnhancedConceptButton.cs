using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Enhanced concept button with progress bars, mastery tracking, and challenge integration
    /// </summary>
    public class EnhancedConceptButton : MonoBehaviour
    {
        [Header("UI References")]
        public Button mainButton;
        public TextMeshProUGUI conceptNameText;
        public TextMeshProUGUI statusText;
        public Image backgroundImage;
        public Image progressFillImage;
        public Button challengeButton;
        
        [Header("Visual States")]
        public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        public Color availableColor = new Color(0.2f, 0.2f, 0.8f, 0.8f);
        public Color unlockedColor = new Color(0.2f, 0.6f, 0.2f, 0.8f);
        public Color masteredColor = new Color(0.8f, 0.6f, 0.1f, 0.8f);
        
        private string conceptId;
        private ProgrammingConcept concept;
        private LearningSystem learningSystem;
        
        public void Setup(string id, ProgrammingConcept programmingConcept, LearningSystem system)
        {
            conceptId = id;
            concept = programmingConcept;
            learningSystem = system;
            
            // Setup button click handlers
            if (mainButton != null)
            {
                mainButton.onClick.RemoveAllListeners();
                mainButton.onClick.AddListener(OnMainButtonClick);
            }
            
            if (challengeButton != null)
            {
                challengeButton.onClick.RemoveAllListeners();
                challengeButton.onClick.AddListener(OnChallengeButtonClick);
            }
            
            UpdateDisplay();
        }
        
        public void UpdateDisplay()
        {
            if (concept == null || learningSystem == null) return;
            
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) return;
            
            // Update concept name
            if (conceptNameText != null)
                conceptNameText.text = concept.name;
            
            // Determine current state
            ConceptButtonState state = GetConceptState(gameData);
            
            // Update visual appearance
            UpdateVisualState(state);
            
            // Update progress bar
            UpdateProgressBar(gameData);
            
            // Update status text
            UpdateStatusText(state);
            
            // Update challenge button
            UpdateChallengeButton(state);
        }
        
        private ConceptButtonState GetConceptState(GameData gameData)
        {
            LearningProgress progress = gameData.learningProgress;
            
            if (progress.IsConceptMastered(conceptId))
                return ConceptButtonState.Mastered;
            else if (progress.IsConceptUnlocked(conceptId))
                return ConceptButtonState.Unlocked;
            else if (learningSystem.CanUnlockConcept(conceptId))
                return ConceptButtonState.Available;
            else
                return ConceptButtonState.Locked;
        }
        
        private void UpdateVisualState(ConceptButtonState state)
        {
            if (backgroundImage == null) return;
            
            switch (state)
            {
                case ConceptButtonState.Locked:
                    backgroundImage.color = lockedColor;
                    break;
                case ConceptButtonState.Available:
                    backgroundImage.color = availableColor;
                    break;
                case ConceptButtonState.Unlocked:
                    backgroundImage.color = unlockedColor;
                    break;
                case ConceptButtonState.Mastered:
                    backgroundImage.color = masteredColor;
                    break;
            }
        }
        
        private void UpdateProgressBar(GameData gameData)
        {
            if (progressFillImage == null) return;
            
            if (gameData.learningProgress.IsConceptUnlocked(conceptId))
            {
                float progress = gameData.learningProgress.GetConceptProgress(conceptId);
                progressFillImage.fillAmount = progress / 100f;
                progressFillImage.gameObject.SetActive(true);
                
                // Change progress bar color based on progress
                if (progress >= 100f)
                    progressFillImage.color = Color.yellow; // Mastered
                else if (progress >= 75f)
                    progressFillImage.color = Color.green; // Advanced
                else if (progress >= 50f)
                    progressFillImage.color = Color.blue; // Intermediate
                else
                    progressFillImage.color = Color.white; // Basic
            }
            else
            {
                progressFillImage.gameObject.SetActive(false);
            }
        }
        
        private void UpdateStatusText(ConceptButtonState state)
        {
            if (statusText == null) return;
            
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) return;
            
            switch (state)
            {
                case ConceptButtonState.Locked:
                    double cost = learningSystem.GetConceptCost(conceptId);
                    statusText.text = $"Cost: {FormatNumber(cost)} KP\nLocked";
                    break;
                    
                case ConceptButtonState.Available:
                    double availableCost = learningSystem.GetConceptCost(conceptId);
                    statusText.text = $"Cost: {FormatNumber(availableCost)} KP\nReady to unlock!";
                    break;
                    
                case ConceptButtonState.Unlocked:
                    float progress = gameData.learningProgress.GetConceptProgress(conceptId);
                    statusText.text = $"Progress: {progress:F0}%\nClick to practice!";
                    break;
                    
                case ConceptButtonState.Mastered:
                    statusText.text = "MASTERED!\nEarning max rewards";
                    break;
            }
        }
        
        private void UpdateChallengeButton(ConceptButtonState state)
        {
            if (challengeButton == null) return;
            
            // Show challenge button only for unlocked (but not mastered) concepts
            bool showChallengeButton = state == ConceptButtonState.Unlocked;
            challengeButton.gameObject.SetActive(showChallengeButton);
        }
        
        private void OnMainButtonClick()
        {
            ConceptButtonState state = GetConceptState(GameManager.Instance.GetGameData());
            
            switch (state)
            {
                case ConceptButtonState.Available:
                    TryUnlockConcept();
                    break;
                    
                case ConceptButtonState.Unlocked:
                    OpenConceptDetails();
                    break;
                    
                case ConceptButtonState.Mastered:
                    OpenConceptDetails();
                    break;
                    
                case ConceptButtonState.Locked:
                    ShowPrerequisites();
                    break;
            }
        }
        
        private void OnChallengeButtonClick()
        {
            Debug.Log($"[EnhancedConceptButton] Opening challenge for: {conceptId}");
            
            // Find CodingChallengeManager and open challenge
            CodingChallengeManager challengeManager = FindFirstObjectByType<CodingChallengeManager>();
            if (challengeManager != null)
            {
                challengeManager.OpenChallenge(conceptId);
            }
            else
            {
                Debug.LogWarning("[EnhancedConceptButton] CodingChallengeManager not found!");
            }
        }
        
        private void TryUnlockConcept()
        {
            if (learningSystem.TryUnlockConcept(conceptId))
            {
                UpdateDisplay();
                
                // Play success sound
                FindFirstObjectByType<AudioManager>()?.PlayUnlockSound();
                
                // Show notification
                FindFirstObjectByType<UIManager>()?.ShowNotification($"🎉 {concept.name} unlocked!", NotificationType.Success);
            }
            else
            {
                FindFirstObjectByType<UIManager>()?.ShowNotification("Not enough Knowledge Points!", NotificationType.Error);
            }
        }
        
        private void OpenConceptDetails()
        {
            Debug.Log($"[EnhancedConceptButton] Opening details for: {concept.name}");
            // TODO: Implement concept details popup
            FindFirstObjectByType<UIManager>()?.ShowNotification($"📖 {concept.name}: {concept.explanation}", NotificationType.Info);
        }
        
        private void ShowPrerequisites()
        {
            var missing = learningSystem.GetMissingPrerequisites(conceptId);
            if (missing.Count > 0)
            {
                string prereqList = string.Join(", ", missing);
                FindFirstObjectByType<UIManager>()?.ShowNotification($"Prerequisites needed: {prereqList}", NotificationType.Info);
            }
            else
            {
                FindFirstObjectByType<UIManager>()?.ShowNotification("Not enough Knowledge Points!", NotificationType.Error);
            }
        }
        
        private string FormatNumber(double number)
        {
            if (number >= 1000000)
                return (number / 1000000).ToString("F1") + "M";
            if (number >= 1000)
                return (number / 1000).ToString("F1") + "K";
            
            return number.ToString("F0");
        }
        
        private void Update()
        {
            // Update display periodically
            if (Time.frameCount % 60 == 0) // Every 60 frames (~1 second at 60fps)
            {
                UpdateDisplay();
            }
        }
    }

    public enum ConceptButtonState
    {
        Locked,
        Available,
        Unlocked,
        Mastered
    }
}