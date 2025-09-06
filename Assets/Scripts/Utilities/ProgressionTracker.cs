using UnityEngine;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Tracks and displays player progression milestones
    /// </summary>
    public class ProgressionTracker : MonoBehaviour
    {
        [Header("Debug Display")]
        public bool showProgressionInfo = true;
        
        private GameData lastGameData;
        private float updateInterval = 1f;
        private float lastUpdate = 0f;
        
        private void Update()
        {
            if (Time.time - lastUpdate > updateInterval)
            {
                CheckProgression();
                lastUpdate = Time.time;
            }
        }
        
        private void CheckProgression()
        {
            if (GameManager.Instance == null) return;
            
            GameData gameData = GameManager.Instance.GetGameData();
            if (gameData == null) return;
            
            // Check if this is first time checking
            if (lastGameData == null)
            {
                lastGameData = gameData;
                return;
            }
            
            // Check for new concepts unlocked
            CheckNewConceptsUnlocked(gameData);
            
            // Update reference
            lastGameData = gameData;
        }
        
        private void CheckNewConceptsUnlocked(GameData currentData)
        {
            // Compare current unlocked concepts with previous
            foreach (string conceptId in currentData.learningProgress.unlockedConcepts)
            {
                bool wasAlreadyUnlocked = lastGameData.learningProgress.unlockedConcepts.Contains(conceptId);
                
                if (!wasAlreadyUnlocked)
                {
                    OnConceptUnlocked(conceptId);
                }
            }
        }
        
        private void OnConceptUnlocked(string conceptId)
        {
            Debug.Log($"[ProgressionTracker] 🎉 CONCEPT UNLOCKED: {conceptId}!");
            
            // Show immediate benefits
            ShowUnlockBenefits(conceptId);
            
            // Give immediate bonus rewards
            GiveUnlockBonus(conceptId);
        }
        
        private void ShowUnlockBenefits(string conceptId)
        {
            CompanyManager companyManager = FindFirstObjectByType<CompanyManager>();
            
            if (companyManager != null)
            {
                double passiveKP = companyManager.GetComponent<CompanyManager>().GetType()
                    .GetMethod("GetConceptPassiveValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(companyManager, new object[] { conceptId }) as double? ?? 0;
                    
                double passiveRate = passiveKP * 0.3; // 30% for unlocked
                
                Debug.Log($"[ProgressionTracker] ✨ BENEFITS UNLOCKED:");
                Debug.Log($"  📈 Passive KP: +{passiveRate:F2} per minute");
                Debug.Log($"  💰 Cash bonus: +{passiveRate * 0.02:F2} per second"); 
                Debug.Log($"  🔓 New concepts may be available");
                
                // Show in UI if UIManager exists
                UIManager uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowNotification($"🎉 {conceptId} unlocked! Now earning passive KP!", NotificationType.Success);
                }
            }
        }
        
        private void GiveUnlockBonus(string conceptId)
        {
            if (GameManager.Instance == null) return;
            
            GameData gameData = GameManager.Instance.GetGameData();
            
            // Give immediate rewards for unlocking
            double bonusKP = 5; // Immediate bonus
            double bonusCash = 25; // Immediate cash bonus
            
            gameData.AddKnowledgePoints(bonusKP);
            gameData.AddCash(bonusCash);
            
            Debug.Log($"[ProgressionTracker] 💎 UNLOCK BONUS: +{bonusKP} KP, +${bonusCash}");
        }
        
        private void OnGUI()
        {
            if (!showProgressionInfo) return;
            
            if (GameManager.Instance == null) return;
            GameData gameData = GameManager.Instance.GetGameData();
            if (gameData == null) return;
            
            GUILayout.BeginArea(new Rect(630, 10, 300, 400));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("🎯 Progression Tracker", GUI.skin.label);
            
            // Show current status
            GUILayout.Label($"💰 Cash: ${gameData.cash:F1}");
            GUILayout.Label($"🧠 Knowledge: {gameData.knowledgePoints:F1} KP");
            GUILayout.Label($"🏢 Company: {gameData.GetCompanyStage()}");
            GUILayout.Label($"💼 Valuation: ${gameData.companyValuation:F1}");
            
            GUILayout.Space(10);
            
            // Show unlocked concepts
            GUILayout.Label("🔓 Unlocked Concepts:");
            if (gameData.learningProgress.unlockedConcepts.Count > 0)
            {
                foreach (string concept in gameData.learningProgress.unlockedConcepts)
                {
                    bool isMastered = gameData.learningProgress.masteredConcepts.Contains(concept);
                    string status = isMastered ? "✅" : "🟡";
                    GUILayout.Label($"  {status} {concept}");
                }
            }
            else
            {
                GUILayout.Label("  None yet - unlock Variables first!");
            }
            
            GUILayout.Space(10);
            
            // Show passive income rates
            GUILayout.Label("📈 Passive Income:");
            // This would need access to CompanyManager's passive calculation
            GUILayout.Label($"  Base: ${0.05:F3}/sec");
            GUILayout.Label($"  Concepts: +{gameData.learningProgress.GetTotalConceptsLearned() * 0.02:F3}/sec");
            
            GUILayout.Space(10);
            
            // Show next milestones
            GUILayout.Label("🎯 Next Goals:");
            if (gameData.learningProgress.unlockedConcepts.Count == 0)
            {
                GUILayout.Label("  • Unlock Variables (10 KP)");
            }
            else if (!gameData.learningProgress.unlockedConcepts.Contains("Functions"))
            {
                GUILayout.Label("  • Unlock Functions (30 KP)");
            }
            else if (gameData.GetCompanyStage() == CompanyStage.Garage)
            {
                GUILayout.Label("  • Reach Startup stage ($100K)");
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}