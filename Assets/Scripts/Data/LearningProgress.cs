using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeTycoon.Core
{
    [Serializable]
    public class LearningProgress
    {
        [Header("Learning Stats")]
        public Dictionary<string, ConceptProgress> conceptProgress;
        public List<string> unlockedConcepts;
        public List<string> masteredConcepts;
        public int totalChallengesCompleted = 0;
        public double totalKnowledgeEarned = 0;
        
        [Header("Learning Metrics")]
        public DateTime lastLearningActivity;
        public int currentStreak = 0;
        public int longestStreak = 0;
        
        public LearningProgress()
        {
            conceptProgress = new Dictionary<string, ConceptProgress>();
            unlockedConcepts = new List<string>();
            masteredConcepts = new List<string>();
            lastLearningActivity = DateTime.Now;
        }
        
        public bool IsConceptUnlocked(string conceptId)
        {
            return unlockedConcepts.Contains(conceptId);
        }
        
        public bool IsConceptMastered(string conceptId)
        {
            return masteredConcepts.Contains(conceptId);
        }
        
        public void UnlockConcept(string conceptId)
        {
            if (!IsConceptUnlocked(conceptId))
            {
                unlockedConcepts.Add(conceptId);
                
                if (!conceptProgress.ContainsKey(conceptId))
                {
                    conceptProgress[conceptId] = new ConceptProgress(conceptId);
                }
                
                UpdateLearningActivity();
                Debug.Log($"[LearningProgress] Unlocked concept: {conceptId}");
            }
        }
        
        public void AddConceptProgress(string conceptId, float progressAmount)
        {
            if (!conceptProgress.ContainsKey(conceptId))
            {
                conceptProgress[conceptId] = new ConceptProgress(conceptId);
            }
            
            var progress = conceptProgress[conceptId];
            progress.AddProgress(progressAmount);
            
            // Check if concept is now mastered
            if (progress.IsMastered() && !IsConceptMastered(conceptId))
            {
                MasterConcept(conceptId);
            }
            
            UpdateLearningActivity();
        }
        
        private void MasterConcept(string conceptId)
        {
            if (!masteredConcepts.Contains(conceptId))
            {
                masteredConcepts.Add(conceptId);
                Debug.Log($"[LearningProgress] Mastered concept: {conceptId}");
            }
        }
        
        public void CompleteChallengeForConcept(string conceptId, bool successful, float difficulty = 1.0f)
        {
            totalChallengesCompleted++;
            
            if (successful)
            {
                // Award progress based on difficulty
                float progressAmount = 10f * difficulty;
                AddConceptProgress(conceptId, progressAmount);
                
                // Award knowledge points
                double kpReward = 5 * difficulty;
                totalKnowledgeEarned += kpReward;
                GameManager.Instance.GetGameData().AddKnowledgePoints(kpReward);
            }
            
            UpdateLearningActivity();
        }
        
        private void UpdateLearningActivity()
        {
            DateTime now = DateTime.Now;
            
            // Check if this continues the streak (within 24 hours of last activity)
            if ((now - lastLearningActivity).TotalHours <= 24)
            {
                currentStreak++;
            }
            else
            {
                currentStreak = 1; // Reset streak
            }
            
            if (currentStreak > longestStreak)
            {
                longestStreak = currentStreak;
            }
            
            lastLearningActivity = now;
        }
        
        public float GetConceptProgress(string conceptId)
        {
            if (conceptProgress.ContainsKey(conceptId))
            {
                return conceptProgress[conceptId].GetProgressPercent();
            }
            return 0f;
        }
        
        public int GetTotalConceptsLearned()
        {
            return unlockedConcepts.Count;
        }
        
        public int GetMasteredConceptsCount()
        {
            return masteredConcepts.Count;
        }
        
        public List<string> GetAvailableConceptsToUnlock()
        {
            // This would check prerequisites and return concepts that can be unlocked
            // For now, return a simple list
            List<string> available = new List<string>();
            
            if (!IsConceptUnlocked("Variables"))
                available.Add("Variables");
            else if (!IsConceptUnlocked("Functions"))
                available.Add("Functions");
            else if (!IsConceptUnlocked("Loops"))
                available.Add("Loops");
            
            return available;
        }
        
        public double GetLearningMultiplier()
        {
            // Learning multiplier based on streak and mastered concepts
            double streakMultiplier = 1.0 + (currentStreak * 0.1);
            double masteryMultiplier = 1.0 + (masteredConcepts.Count * 0.05);
            
            return streakMultiplier * masteryMultiplier;
        }
    }
    
    [Serializable]
    public class ConceptProgress
    {
        public string conceptId;
        public float currentProgress = 0f;
        public float maxProgress = 100f;
        public int challengesCompleted = 0;
        public DateTime firstUnlocked;
        public DateTime lastActivity;
        
        public ConceptProgress(string id)
        {
            conceptId = id;
            firstUnlocked = DateTime.Now;
            lastActivity = DateTime.Now;
        }
        
        public void AddProgress(float amount)
        {
            currentProgress = Mathf.Clamp(currentProgress + amount, 0, maxProgress);
            lastActivity = DateTime.Now;
        }
        
        public float GetProgressPercent()
        {
            return (currentProgress / maxProgress) * 100f;
        }
        
        public bool IsMastered()
        {
            return currentProgress >= maxProgress;
        }
        
        public MasteryLevel GetMasteryLevel()
        {
            float percent = GetProgressPercent();
            
            if (percent >= 100f) return MasteryLevel.Expert;
            if (percent >= 75f) return MasteryLevel.Advanced;
            if (percent >= 50f) return MasteryLevel.Intermediate;
            if (percent >= 25f) return MasteryLevel.Basic;
            return MasteryLevel.Beginner;
        }
    }
    
    public enum MasteryLevel
    {
        Beginner,
        Basic,
        Intermediate,
        Advanced,
        Expert
    }
}