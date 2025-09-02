using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeTycoon.Core
{
    public class ConceptButton : MonoBehaviour
    {
        [Header("UI References")]
        public Button button;
        public TextMeshProUGUI conceptNameText;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI progressText;
        public Image progressFill;
        public Image lockIcon;
        
        [Header("Visual States")]
        public Color lockedColor = Color.gray;
        public Color availableColor = Color.white;
        public Color unlockedColor = Color.green;
        public Color masteredColor = Color.yellow;
        
        private string conceptId;
        private ProgrammingConcept concept;
        private LearningSystem learningSystem;
        
        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
                
            if (button != null)
                button.onClick.AddListener(OnButtonClick);
        }
        
        public void Setup(string id, ProgrammingConcept programmingConcept, LearningSystem system)
        {
            conceptId = id;
            concept = programmingConcept;
            learningSystem = system;
            
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (concept == null) return;
            
            // Update text
            if (conceptNameText != null)
                conceptNameText.text = concept.name;
                
            // Update cost
            if (costText != null)
            {
                double cost = learningSystem.GetConceptCost(conceptId);
                costText.text = $"{FormatNumber(cost)} KP";
            }
            
            // Update state
            UpdateButtonState();
            UpdateProgress();
        }
        
        private void UpdateButtonState()
        {
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) return;
            
            LearningProgress progress = gameData.learningProgress;
            
            if (progress.IsConceptMastered(conceptId))
            {
                // Mastered state
                SetButtonState(ConceptButtonState.Mastered);
            }
            else if (progress.IsConceptUnlocked(conceptId))
            {
                // Unlocked state
                SetButtonState(ConceptButtonState.Unlocked);
            }
            else if (learningSystem.CanUnlockConcept(conceptId))
            {
                // Available to unlock
                SetButtonState(ConceptButtonState.Available);
            }
            else
            {
                // Locked state
                SetButtonState(ConceptButtonState.Locked);
            }
        }
        
        private void UpdateProgress()
        {
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) return;
            
            if (gameData.learningProgress.IsConceptUnlocked(conceptId))
            {
                float progress = gameData.learningProgress.GetConceptProgress(conceptId);
                
                if (progressFill != null)
                {
                    progressFill.fillAmount = progress / 100f;
                    progressFill.gameObject.SetActive(true);
                }
                
                if (progressText != null)
                {
                    progressText.text = $"{progress:F0}%";
                    progressText.gameObject.SetActive(true);
                }
            }
            else
            {
                if (progressFill != null)
                    progressFill.gameObject.SetActive(false);
                    
                if (progressText != null)
                    progressText.gameObject.SetActive(false);
            }
        }
        
        private void SetButtonState(ConceptButtonState state)
        {
            Color targetColor = Color.white;
            bool interactable = false;
            bool showLock = false;
            
            switch (state)
            {
                case ConceptButtonState.Locked:
                    targetColor = lockedColor;
                    interactable = false;
                    showLock = true;
                    break;
                    
                case ConceptButtonState.Available:
                    targetColor = availableColor;
                    interactable = true;
                    showLock = false;
                    break;
                    
                case ConceptButtonState.Unlocked:
                    targetColor = unlockedColor;
                    interactable = true;
                    showLock = false;
                    break;
                    
                case ConceptButtonState.Mastered:
                    targetColor = masteredColor;
                    interactable = false;
                    showLock = false;
                    break;
            }
            
            // Apply visual changes
            if (button != null)
            {
                button.interactable = interactable;
                
                ColorBlock colors = button.colors;
                colors.normalColor = targetColor;
                colors.selectedColor = targetColor;
                button.colors = colors;
            }
            
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(showLock);
            }
        }
        
        private void OnButtonClick()
        {
            if (learningSystem == null) return;
            
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) return;
            
            if (gameData.learningProgress.IsConceptUnlocked(conceptId))
            {
                // Show concept details or challenges
                ShowConceptDetails();
            }
            else if (learningSystem.CanUnlockConcept(conceptId))
            {
                // Try to unlock concept
                if (learningSystem.TryUnlockConcept(conceptId))
                {
                    // Play unlock sound
                    FindFirstObjectByType<AudioManager>()?.PlayUnlockSound();
                    
                    // Update display
                    UpdateDisplay();

                    // Show notification
                    FindFirstObjectByType<UIManager>()?.ShowNotification($"Unlocked: {concept.name}!", NotificationType.Success);
                }
                else
                {
                    // Show error
                    FindFirstObjectByType<UIManager>()?.ShowNotification("Not enough Knowledge Points!", NotificationType.Error);
                }
            }
            else
            {
                // Show prerequisites
                ShowPrerequisites();
            }
        }
        
        private void ShowConceptDetails()
        {
            Debug.Log($"[ConceptButton] Showing details for: {concept.name}");
            // TODO: Implement concept details popup
        }
        
        private void ShowPrerequisites()
        {
            var missing = learningSystem.GetMissingPrerequisites(conceptId);
            string prereqList = string.Join(", ", missing);
            FindFirstObjectByType<UIManager>()?.ShowNotification($"Prerequisites needed: {prereqList}", NotificationType.Info);
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
            if (Time.frameCount % 30 == 0) // Every 30 frames (~0.5 seconds at 60fps)
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