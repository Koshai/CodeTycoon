using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTycoon.Core
{
    [Serializable]
    public class ProjectData
    {
        [Header("Project Info")]
        public string projectId;
        public string projectName;
        public string clientName;
        public ProjectType type;
        public ProjectStatus status = ProjectStatus.Available;
        
        [Header("Requirements")]
        public List<string> requiredConcepts;
        public int difficultyLevel = 1; // 1-10
        public double budget;
        public int estimatedDays;
        
        [Header("Progress")]
        public float completionPercent = 0f;
        public int daysWorked = 0;
        public List<ProjectMilestone> milestones;
        public List<string> completedChallenges;
        
        [Header("Rewards")]
        public double payment;
        public double bonusPayment = 0;
        public double reputationReward;
        public double knowledgePointReward;
        
        [Header("Client Info")]
        public ClientType clientType;
        public float clientSatisfaction = 50f; // 0-100
        public DateTime deadline;
        public DateTime startDate;
        public DateTime? completionDate;
        
        public ProjectData()
        {
            projectId = Guid.NewGuid().ToString();
            requiredConcepts = new List<string>();
            milestones = new List<ProjectMilestone>();
            completedChallenges = new List<string>();
        }
        
        public ProjectData(string name, string client, ProjectType projectType, ClientType clientTypeValue)
        {
            projectId = Guid.NewGuid().ToString();
            projectName = name;
            clientName = client;
            type = projectType;
            clientType = clientTypeValue;
            
            requiredConcepts = new List<string>();
            milestones = new List<ProjectMilestone>();
            completedChallenges = new List<string>();
            
            GenerateProjectRequirements();
        }
        
        private void GenerateProjectRequirements()
        {
            // Set basic requirements based on project type and client type
            switch (type)
            {
                case ProjectType.WebApp:
                    requiredConcepts.AddRange(new[] { "Variables", "Functions", "HTML", "CSS", "JavaScript" });
                    difficultyLevel = 2;
                    estimatedDays = 14;
                    break;
                    
                case ProjectType.MobileApp:
                    requiredConcepts.AddRange(new[] { "Variables", "Functions", "OOP", "API Integration" });
                    difficultyLevel = 4;
                    estimatedDays = 28;
                    break;
                    
                case ProjectType.API:
                    requiredConcepts.AddRange(new[] { "Functions", "Database", "Security", "API Design" });
                    difficultyLevel = 3;
                    estimatedDays = 21;
                    break;
                    
                case ProjectType.EnterpriseSystem:
                    requiredConcepts.AddRange(new[] { "OOP", "Database", "Security", "Architecture", "Testing" });
                    difficultyLevel = 6;
                    estimatedDays = 60;
                    break;
            }
            
            // Adjust based on client type
            switch (clientType)
            {
                case ClientType.Startup:
                    budget = 5000 + (difficultyLevel * 1000);
                    break;
                case ClientType.SMB:
                    budget = 10000 + (difficultyLevel * 2000);
                    break;
                case ClientType.Enterprise:
                    budget = 25000 + (difficultyLevel * 5000);
                    break;
                case ClientType.Government:
                    budget = 50000 + (difficultyLevel * 7500);
                    break;
            }
            
            payment = budget * 0.8; // 80% of budget as payment
            bonusPayment = budget * 0.2; // 20% potential bonus
            reputationReward = difficultyLevel * 2;
            knowledgePointReward = difficultyLevel * 10;
            
            // Set deadline
            deadline = DateTime.Now.AddDays(estimatedDays * 1.5); // 50% buffer
            
            GenerateMilestones();
        }
        
        private void GenerateMilestones()
        {
            milestones.Clear();
            
            switch (type)
            {
                case ProjectType.WebApp:
                    milestones.Add(new ProjectMilestone("Setup & Planning", 20f, new[] { "Variables" }));
                    milestones.Add(new ProjectMilestone("Frontend Development", 40f, new[] { "HTML", "CSS" }));
                    milestones.Add(new ProjectMilestone("Backend Integration", 30f, new[] { "JavaScript", "Functions" }));
                    milestones.Add(new ProjectMilestone("Testing & Deployment", 10f, new[] { "Debugging" }));
                    break;
                    
                case ProjectType.MobileApp:
                    milestones.Add(new ProjectMilestone("App Architecture", 25f, new[] { "OOP" }));
                    milestones.Add(new ProjectMilestone("Core Features", 35f, new[] { "Functions", "Variables" }));
                    milestones.Add(new ProjectMilestone("API Integration", 25f, new[] { "API Integration" }));
                    milestones.Add(new ProjectMilestone("Testing & Polish", 15f, new[] { "Testing" }));
                    break;
                    
                default:
                    milestones.Add(new ProjectMilestone("Analysis & Design", 30f, requiredConcepts.ToArray()));
                    milestones.Add(new ProjectMilestone("Development", 50f, requiredConcepts.ToArray()));
                    milestones.Add(new ProjectMilestone("Testing & Delivery", 20f, new[] { "Testing", "Debugging" }));
                    break;
            }
        }
        
        public void StartProject()
        {
            status = ProjectStatus.InProgress;
            startDate = DateTime.Now;
            Debug.Log($"[ProjectData] Started project: {projectName}");
        }
        
        public void AddProgress(float progressPercent)
        {
            completionPercent = Mathf.Clamp(completionPercent + progressPercent, 0f, 100f);
            
            if (completionPercent >= 100f)
            {
                CompleteProject(true);
            }
        }
        
        public void CompleteProject(bool successful)
        {
            if (successful)
            {
                status = ProjectStatus.Completed;
                completionDate = DateTime.Now;
                
                // Calculate final payment based on timing and quality
                double finalPayment = CalculateFinalPayment();
                GameManager.Instance.GetGameData().AddCash(finalPayment);
                GameManager.Instance.GetGameData().companyData.CompleteProject(true, clientSatisfaction / 20f); // Convert to 1-5 scale
                
                Debug.Log($"[ProjectData] Completed project: {projectName} - Payment: ${finalPayment:F2}");
            }
            else
            {
                status = ProjectStatus.Failed;
                GameManager.Instance.GetGameData().companyData.CompleteProject(false, 1f);
                Debug.Log($"[ProjectData] Failed project: {projectName}");
            }
        }
        
        private double CalculateFinalPayment()
        {
            double finalPayment = payment;
            
            // Time bonus/penalty
            TimeSpan actualTime = DateTime.Now - startDate;
            TimeSpan expectedTime = deadline - startDate;
            
            if (actualTime <= expectedTime)
            {
                // Early completion bonus
                double earlyBonus = bonusPayment * (1.0 - (actualTime.TotalDays / expectedTime.TotalDays));
                finalPayment += earlyBonus;
            }
            
            // Quality bonus based on client satisfaction
            if (clientSatisfaction > 75f)
            {
                finalPayment += bonusPayment * 0.5;
            }
            
            return finalPayment;
        }
        
        public bool CanPlayerAccept()
        {
            var learningProgress = GameManager.Instance.GetGameData().learningProgress;
            
            foreach (string concept in requiredConcepts)
            {
                if (!learningProgress.IsConceptUnlocked(concept))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public List<string> GetMissingConcepts()
        {
            var learningProgress = GameManager.Instance.GetGameData().learningProgress;
            List<string> missing = new List<string>();
            
            foreach (string concept in requiredConcepts)
            {
                if (!learningProgress.IsConceptUnlocked(concept))
                {
                    missing.Add(concept);
                }
            }
            
            return missing;
        }
    }
    
    [Serializable]
    public class ProjectMilestone
    {
        public string name;
        public float progressPercent;
        public string[] requiredConcepts;
        public bool isCompleted = false;
        public DateTime? completionDate;
        
        public ProjectMilestone(string milestoneName, float percent, string[] concepts)
        {
            name = milestoneName;
            progressPercent = percent;
            requiredConcepts = concepts;
        }
        
        public void Complete()
        {
            isCompleted = true;
            completionDate = DateTime.Now;
        }
    }
    
    public enum ProjectType
    {
        WebApp,
        MobileApp,
        API,
        Database,
        EnterpriseSystem,
        GameDevelopment,
        AISystem,
        Research
    }
    
    public enum ProjectStatus
    {
        Available,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }
    
    public enum ClientType
    {
        Individual,
        Startup,
        SMB,
        Enterprise,
        Government,
        NonProfit
    }
}