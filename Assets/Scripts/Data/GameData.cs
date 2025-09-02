using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTycoon.Core
{
    [Serializable]
    public class GameData
    {
        [Header("Game Info")]
        public string version = "1.0.0";
        public DateTime lastSaveTime;
        public float totalPlayTime = 0f;
        
        [Header("Core Resources")]
        public double knowledgePoints = 0;
        public double cash = 100; // Starting cash
        public double companyValuation = 0;
        
        [Header("Game Progress")]
        public CompanyData companyData;
        public LearningProgress learningProgress;
        public List<ProjectData> activeProjects;
        public List<ProjectData> completedProjects;
        
        [Header("Settings")]
        public GameSettings settings;
        
        public GameData()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            lastSaveTime = DateTime.Now;
            
            companyData = new CompanyData();
            learningProgress = new LearningProgress();
            activeProjects = new List<ProjectData>();
            completedProjects = new List<ProjectData>();
            settings = new GameSettings();
            
            // Initialize with starting values
            knowledgePoints = 0;
            cash = 100;
            companyValuation = 0;
            
            Debug.Log("[GameData] Game data initialized with starting values.");
        }
        
        public void UpdatePlayTime(float deltaTime)
        {
            totalPlayTime += deltaTime;
        }
        
        public void AddKnowledgePoints(double amount)
        {
            knowledgePoints += amount;
            Debug.Log($"[GameData] Added {amount} KP. Total: {knowledgePoints:F1}");
        }
        
        public bool SpendKnowledgePoints(double amount)
        {
            if (knowledgePoints >= amount)
            {
                knowledgePoints -= amount;
                Debug.Log($"[GameData] Spent {amount} KP. Remaining: {knowledgePoints:F1}");
                return true;
            }
            return false;
        }
        
        public void AddCash(double amount)
        {
            cash += amount;
            UpdateCompanyValuation();
            Debug.Log($"[GameData] Added ${amount:F2}. Total: ${cash:F2}");
        }
        
        public bool SpendCash(double amount)
        {
            if (cash >= amount)
            {
                cash -= amount;
                Debug.Log($"[GameData] Spent ${amount:F2}. Remaining: ${cash:F2}");
                return true;
            }
            return false;
        }
        
        private void UpdateCompanyValuation()
        {
            // Simple valuation calculation based on cash and completed projects
            double projectValue = completedProjects.Count * 10000;
            companyValuation = cash + projectValue + (learningProgress.GetTotalConceptsLearned() * 5000);
        }
        
        public CompanyStage GetCompanyStage()
        {
            if (companyValuation >= 1000000000) return CompanyStage.TechGiant;
            if (companyValuation >= 100000000) return CompanyStage.Corporation;
            if (companyValuation >= 1000000) return CompanyStage.ScaleUp;
            if (companyValuation >= 100000) return CompanyStage.Startup;
            return CompanyStage.Garage;
        }
        
        public void OnBeforeSave()
        {
            lastSaveTime = DateTime.Now;
            UpdateCompanyValuation();
        }
    }
    
    public enum CompanyStage
    {
        Garage,      // $0-$100K
        Startup,     // $100K-$1M
        ScaleUp,     // $1M-$100M
        Corporation, // $100M-$1B
        TechGiant    // $1B+
    }
}