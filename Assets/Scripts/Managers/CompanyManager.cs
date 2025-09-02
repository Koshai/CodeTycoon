using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CodeTycoon.Core
{
    public class CompanyManager : MonoBehaviour
    {
        [Header("Company Settings")]
        [SerializeField] private float monthlyUpdateInterval = 30f; // 30 seconds = 1 month in game
        [SerializeField] private bool debugMode = false;
        
        [Header("Passive Income")]
        [SerializeField] private float passiveIncomeInterval = 1f; // Generate income every second
        
        private Coroutine monthlyUpdateCoroutine;
        private Coroutine passiveIncomeCoroutine;
        
        // Events
        public delegate void CompanyStageChanged(CompanyStage newStage);
        public static event CompanyStageChanged OnCompanyStageChanged;
        
        public delegate void MonthlyUpdate(CompanyData companyData);
        public static event MonthlyUpdate OnMonthlyUpdate;
        
        public void Initialize()
        {
            Debug.Log("[CompanyManager] Initializing Company Manager...");
            
            StartMonthlyUpdates();
            StartPassiveIncome();
            
            Debug.Log("[CompanyManager] Company Manager initialized.");
        }
        
        private void StartMonthlyUpdates()
        {
            if (monthlyUpdateCoroutine != null)
            {
                StopCoroutine(monthlyUpdateCoroutine);
            }
            
            monthlyUpdateCoroutine = StartCoroutine(MonthlyUpdateCoroutine());
        }
        
        private void StartPassiveIncome()
        {
            if (passiveIncomeCoroutine != null)
            {
                StopCoroutine(passiveIncomeCoroutine);
            }
            
            passiveIncomeCoroutine = StartCoroutine(PassiveIncomeCoroutine());
        }
        
        private IEnumerator MonthlyUpdateCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(monthlyUpdateInterval);
                ProcessMonthlyUpdate();
            }
        }
        
        private IEnumerator PassiveIncomeCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(passiveIncomeInterval);
                GeneratePassiveIncome();
            }
        }
        
        private void ProcessMonthlyUpdate()
        {
            if (GameManager.Instance == null) return;
            
            GameData gameData = GameManager.Instance.GetGameData();
            if (gameData?.companyData == null) return;
            
            CompanyData company = gameData.companyData;
            
            Debug.Log("[CompanyManager] Processing monthly update...");
            
            // Calculate monthly finances
            double monthlyIncome = company.GetNetIncome();
            
            // Apply monthly income/expenses
            gameData.AddCash(monthlyIncome);
            
            // Update company stage based on valuation
            CompanyStage currentStage = company.currentStage;
            CompanyStage newStage = gameData.GetCompanyStage();
            
            if (newStage != currentStage)
            {
                company.UpdateStage(newStage);
                OnCompanyStageChanged?.Invoke(newStage);
                
                // Stage-specific bonuses and unlocks
                HandleStageProgression(currentStage, newStage);
            }
            
            // Update reputation based on recent performance
            UpdateReputation(company);
            
            OnMonthlyUpdate?.Invoke(company);
            
            if (debugMode)
            {
                Debug.Log($"[CompanyManager] Monthly update: Income: ${monthlyIncome:F2}, Stage: {newStage}");
            }
        }
        
        private void GeneratePassiveIncome()
        {
            if (GameManager.Instance == null) return;
            
            GameData gameData = GameManager.Instance.GetGameData();
            if (gameData == null) return;
            
            // Calculate passive income based on learned concepts and automation
            double passiveKP = CalculatePassiveKnowledgePoints();
            double passiveCash = CalculatePassiveCash();
            
            if (passiveKP > 0)
            {
                gameData.AddKnowledgePoints(passiveKP);
            }
            
            if (passiveCash > 0)
            {
                gameData.AddCash(passiveCash);
            }
        }
        
        private double CalculatePassiveKnowledgePoints()
        {
            GameData gameData = GameManager.Instance.GetGameData();
            LearningProgress learning = gameData.learningProgress;
            
            double basePassiveKP = 0;
            
            // Each mastered concept provides passive KP
            foreach (string masteredConcept in learning.masteredConcepts)
            {
                basePassiveKP += GetConceptPassiveValue(masteredConcept);
            }
            
            // Apply learning multiplier
            double multiplier = learning.GetLearningMultiplier();
            
            // Scale based on company stage
            double stageMultiplier = GetStageMultiplier(gameData.GetCompanyStage());
            
            return basePassiveKP * multiplier * stageMultiplier / 60f; // Convert to per-second rate
        }
        
        private double CalculatePassiveCash()
        {
            GameData gameData = GameManager.Instance.GetGameData();
            CompanyData company = gameData.companyData;
            
            // Base passive cash from completed projects and reputation
            double baseCash = company.projectsCompleted * 0.1; // $0.10 per second per completed project
            
            // Reputation bonus
            double reputationMultiplier = 1.0 + (company.reputation / 200.0); // Up to 50% bonus at max reputation
            
            return baseCash * reputationMultiplier;
        }
        
        private double GetConceptPassiveValue(string conceptId)
        {
            // Different concepts provide different passive KP rates
            switch (conceptId.ToLower())
            {
                case "variables": return 0.5;
                case "functions": return 1.0;
                case "loops": return 1.5;
                case "oop": return 2.0;
                case "database": return 2.5;
                case "api": return 3.0;
                case "security": return 3.5;
                case "ai": return 5.0;
                default: return 0.5;
            }
        }
        
        private double GetStageMultiplier(CompanyStage stage)
        {
            switch (stage)
            {
                case CompanyStage.Garage: return 1.0;
                case CompanyStage.Startup: return 1.5;
                case CompanyStage.ScaleUp: return 2.5;
                case CompanyStage.Corporation: return 4.0;
                case CompanyStage.TechGiant: return 6.0;
                default: return 1.0;
            }
        }
        
        private void HandleStageProgression(CompanyStage oldStage, CompanyStage newStage)
        {
            Debug.Log($"[CompanyManager] Company progressed from {oldStage} to {newStage}!");
            
            // Stage-specific unlocks and bonuses
            switch (newStage)
            {
                case CompanyStage.Startup:
                    UnlockStartupFeatures();
                    break;
                case CompanyStage.ScaleUp:
                    UnlockScaleUpFeatures();
                    break;
                case CompanyStage.Corporation:
                    UnlockCorporationFeatures();
                    break;
                case CompanyStage.TechGiant:
                    UnlockTechGiantFeatures();
                    break;
            }
        }
        
        private void UnlockStartupFeatures()
        {
            Debug.Log("[CompanyManager] Unlocked Startup features: Team hiring, basic client management");
            // TODO: Unlock team hiring system
            // TODO: Unlock basic client management
        }
        
        private void UnlockScaleUpFeatures()
        {
            Debug.Log("[CompanyManager] Unlocked Scale-Up features: Enterprise clients, complex projects");
            // TODO: Unlock enterprise clients
            // TODO: Unlock complex project types
        }
        
        private void UnlockCorporationFeatures()
        {
            Debug.Log("[CompanyManager] Unlocked Corporation features: International expansion, product development");
            // TODO: Unlock international markets
            // TODO: Unlock product development system
        }
        
        private void UnlockTechGiantFeatures()
        {
            Debug.Log("[CompanyManager] Unlocked Tech Giant features: Industry leadership, research projects");
            // TODO: Unlock industry leadership mechanics
            // TODO: Unlock advanced research projects
        }
        
        private void UpdateReputation(CompanyData company)
        {
            // Slowly decay reputation if no recent activity
            if (company.reputation > 50)
            {
                company.reputation = Mathf.Clamp((float)(company.reputation - 0.5), 0, 100);
            }
            
            // Boost reputation based on successful projects
            float successRate = company.projectsCompleted / (float)Mathf.Max(1, company.projectsCompleted + company.projectsFailed);
            if (successRate > 0.8f)
            {
                company.reputation = Mathf.Clamp((float)(company.reputation + 0.2), 0, 100);
            }
        }
        
        public void HireEmployee(EmployeeRole role, int skillLevel)
        {
            if (GameManager.Instance == null) return;
            
            CompanyData company = GameManager.Instance.GetGameData().companyData;
            
            // Calculate salary based on role and skill level
            double salary = CalculateEmployeeSalary(role, skillLevel);
            
            // Check if company can afford the employee
            if (GameManager.Instance.GetGameData().SpendCash(salary))
            {
                string employeeName = GenerateEmployeeName(role);
                EmployeeData newEmployee = new EmployeeData(employeeName, role, skillLevel, salary);
                
                company.AddEmployee(newEmployee);
                
                Debug.Log($"[CompanyManager] Hired {employeeName} as {role} for ${salary:F2}/month");
            }
            else
            {
                Debug.Log($"[CompanyManager] Cannot afford to hire {role} (${salary:F2}/month)");
            }
        }
        
        private double CalculateEmployeeSalary(EmployeeRole role, int skillLevel)
        {
            double baseSalary = 3000; // Base monthly salary
            
            // Role multipliers
            switch (role)
            {
                case EmployeeRole.Developer: baseSalary *= 1.2; break;
                case EmployeeRole.Designer: baseSalary *= 1.0; break;
                case EmployeeRole.ProjectManager: baseSalary *= 1.3; break;
                case EmployeeRole.QATester: baseSalary *= 0.9; break;
                case EmployeeRole.DevOps: baseSalary *= 1.4; break;
                case EmployeeRole.DataScientist: baseSalary *= 1.5; break;
                case EmployeeRole.Marketing: baseSalary *= 1.1; break;
                case EmployeeRole.Sales: baseSalary *= 1.2; break;
            }
            
            // Skill level multiplier
            baseSalary *= (1.0 + (skillLevel - 1) * 0.2);
            
            return baseSalary;
        }
        
        private string GenerateEmployeeName(EmployeeRole role)
        {
            string[] firstNames = { "Alex", "Jordan", "Taylor", "Casey", "Morgan", "Riley", "Avery", "Quinn" };
            string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Davis", "Miller", "Wilson", "Garcia" };
            
            return firstNames[Random.Range(0, firstNames.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
        }
        
        private void OnDestroy()
        {
            if (monthlyUpdateCoroutine != null)
            {
                StopCoroutine(monthlyUpdateCoroutine);
            }
            
            if (passiveIncomeCoroutine != null)
            {
                StopCoroutine(passiveIncomeCoroutine);
            }
        }
    }
}