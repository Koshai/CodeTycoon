using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeTycoon.Core
{
    public class DashboardUI : MonoBehaviour
    {
        [Header("Resource Display")]
        public TextMeshProUGUI knowledgePointsText;
        public TextMeshProUGUI cashText;
        public TextMeshProUGUI companyValueText;
        public TextMeshProUGUI companyStageText;
        
        [Header("Click Button")]
        public Button earnKnowledgeButton;
        public TextMeshProUGUI earnButtonText;
        
        [Header("Navigation")]
        public Button learningButton;
        public Button projectsButton;
        public Button companyButton;
        public Button settingsButton;
        
        [Header("Progress Bars")]
        public Slider companyProgressSlider;
        public TextMeshProUGUI progressText;
        
        private void Start()
        {
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            if (earnKnowledgeButton != null)
            {
                earnKnowledgeButton.onClick.AddListener(OnEarnKnowledgeClick);
            }
            
            if (learningButton != null)
            {
                learningButton.onClick.AddListener(() => FindFirstObjectByType<UIManager>()?.ShowLearningScreen());
            }
            
            if (projectsButton != null)
            {
                projectsButton.onClick.AddListener(() => FindFirstObjectByType<UIManager>()?.ShowProjectsScreen());
            }
            
            if (companyButton != null)
            {
                companyButton.onClick.AddListener(() => Debug.Log("Company screen coming soon!"));
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(() => FindFirstObjectByType<UIManager>()?.ShowSettingsScreen());
            }
        }
        
        private void OnEarnKnowledgeClick()
        {
            if (GameManager.Instance != null)
            {
                GameData gameData = GameManager.Instance.GetGameData();
                
                // Basic click earns 1 KP
                double baseKP = 1.0;
                
                // Apply learning multiplier
                double multiplier = gameData.learningProgress.GetLearningMultiplier();
                double finalKP = baseKP * multiplier;
                
                gameData.AddKnowledgePoints(finalKP);
                
                // Show notification with actual amount earned
                FindFirstObjectByType<UIManager>()?.ShowNotification($"+{finalKP:F1} KP!", NotificationType.Success);
                
                // Play click sound (safely handle if AudioManager not found)
                var audioManager = FindFirstObjectByType<AudioManager>();
                if (audioManager != null)
                {
                    audioManager.PlayClickSound();
                }
            }
        }
        
        public void UpdateDisplay(GameData gameData)
        {
            if (gameData == null) return;
            
            if (knowledgePointsText != null)
                knowledgePointsText.text = $"{FormatNumber(gameData.knowledgePoints)} KP";
                
            if (cashText != null)
                cashText.text = $"${FormatNumber(gameData.cash)}";
                
            if (companyValueText != null)
                companyValueText.text = $"${FormatNumber(gameData.companyValuation)}";
                
            if (companyStageText != null)
                companyStageText.text = gameData.GetCompanyStage().ToString();
                
            UpdateProgressBar(gameData);
        }
        
        private void UpdateProgressBar(GameData gameData)
        {
            if (companyProgressSlider != null)
            {
                CompanyStage currentStage = gameData.GetCompanyStage();
                double currentValue = gameData.companyValuation;
                double nextStageThreshold = GetNextStageThreshold(currentStage);
                double currentStageThreshold = GetCurrentStageThreshold(currentStage);
                
                if (nextStageThreshold > currentStageThreshold)
                {
                    float progress = (float)((currentValue - currentStageThreshold) / (nextStageThreshold - currentStageThreshold));
                    companyProgressSlider.value = Mathf.Clamp01(progress);
                    
                    if (progressText != null)
                    {
                        progressText.text = $"{progress * 100:F1}% to {GetNextStage(currentStage)}";
                    }
                }
                else
                {
                    companyProgressSlider.value = 1.0f;
                    if (progressText != null)
                    {
                        progressText.text = "Max Stage Reached!";
                    }
                }
            }
        }
        
        private double GetCurrentStageThreshold(CompanyStage stage)
        {
            switch (stage)
            {
                case CompanyStage.Garage: return 0;
                case CompanyStage.Startup: return 100000;
                case CompanyStage.ScaleUp: return 1000000;
                case CompanyStage.Corporation: return 100000000;
                case CompanyStage.TechGiant: return 1000000000;
                default: return 0;
            }
        }
        
        private double GetNextStageThreshold(CompanyStage stage)
        {
            switch (stage)
            {
                case CompanyStage.Garage: return 100000;
                case CompanyStage.Startup: return 1000000;
                case CompanyStage.ScaleUp: return 100000000;
                case CompanyStage.Corporation: return 1000000000;
                case CompanyStage.TechGiant: return double.MaxValue;
                default: return double.MaxValue;
            }
        }
        
        private CompanyStage GetNextStage(CompanyStage stage)
        {
            switch (stage)
            {
                case CompanyStage.Garage: return CompanyStage.Startup;
                case CompanyStage.Startup: return CompanyStage.ScaleUp;
                case CompanyStage.ScaleUp: return CompanyStage.Corporation;
                case CompanyStage.Corporation: return CompanyStage.TechGiant;
                case CompanyStage.TechGiant: return CompanyStage.TechGiant;
                default: return CompanyStage.Startup;
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
    }
}