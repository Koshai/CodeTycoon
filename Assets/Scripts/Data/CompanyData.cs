using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTycoon.Core
{
    [Serializable]
    public class CompanyData
    {
        [Header("Company Identity")]
        public string companyName = "Garage Dev Co.";
        public int foundedYear;
        public CompanyStage currentStage = CompanyStage.Garage;
        
        [Header("Financial Metrics")]
        public double monthlyRevenue = 0;
        public double monthlyExpenses = 10; // Reduced basic living expenses for early game
        public double reputation = 50; // 0-100 scale
        
        [Header("Team & Resources")]
        public List<EmployeeData> employees;
        public OfficeData office;
        public List<InfrastructureUpgrade> infrastructure;
        
        [Header("Business Metrics")]
        public int clientsSatisfied = 0;
        public int projectsCompleted = 0;
        public int projectsFailed = 0;
        public float avgProjectRating = 0f;
        
        public CompanyData()
        {
            foundedYear = DateTime.Now.Year;
            employees = new List<EmployeeData>();
            office = new OfficeData();
            infrastructure = new List<InfrastructureUpgrade>();
        }
        
        public void UpdateStage(CompanyStage newStage)
        {
            if (newStage != currentStage)
            {
                CompanyStage oldStage = currentStage;
                currentStage = newStage;
                
                Debug.Log($"[CompanyData] Company stage updated: {oldStage} -> {newStage}");
                
                // Update company name based on stage
                UpdateCompanyName();
            }
        }
        
        private void UpdateCompanyName()
        {
            switch (currentStage)
            {
                case CompanyStage.Garage:
                    if (companyName == "Garage Dev Co.") return; // Keep default
                    break;
                case CompanyStage.Startup:
                    if (!companyName.Contains("Studios") && !companyName.Contains("Labs"))
                        companyName = companyName.Replace("Co.", "Studios");
                    break;
                case CompanyStage.ScaleUp:
                    if (!companyName.Contains("Technologies"))
                        companyName = companyName.Replace("Studios", "Technologies");
                    break;
                case CompanyStage.Corporation:
                    if (!companyName.Contains("Systems"))
                        companyName = companyName.Replace("Technologies", "Systems");
                    break;
                case CompanyStage.TechGiant:
                    if (!companyName.Contains("Corporation"))
                        companyName = companyName.Replace("Systems", "Corporation");
                    break;
            }
        }
        
        public double GetNetIncome()
        {
            return monthlyRevenue - monthlyExpenses;
        }
        
        public void AddEmployee(EmployeeData employee)
        {
            employees.Add(employee);
            monthlyExpenses += employee.salary;
            Debug.Log($"[CompanyData] Hired {employee.name} - Monthly expenses now: ${monthlyExpenses:F2}");
        }
        
        public void CompleteProject(bool successful, float rating)
        {
            if (successful)
            {
                projectsCompleted++;
                clientsSatisfied++;
                
                // Update average rating
                avgProjectRating = ((avgProjectRating * (projectsCompleted - 1)) + rating) / projectsCompleted;
                
                // Improve reputation based on rating
                reputation = Mathf.Clamp((float)(reputation + (rating - 3) * 2), 0, 100);
            }
            else
            {
                projectsFailed++;
                reputation = Mathf.Clamp((float)(reputation - 5), 0, 100);
            }
        }
        
        public int GetTotalEmployees()
        {
            return employees.Count;
        }
        
        public double GetTotalSalaryExpenses()
        {
            double total = 0;
            foreach (var employee in employees)
            {
                total += employee.salary;
            }
            return total;
        }
    }
    
    [Serializable]
    public class EmployeeData
    {
        public string name;
        public EmployeeRole role;
        public int skillLevel; // 1-10
        public double salary;
        public DateTime hiredDate;
        public float productivity = 1.0f;
        public List<string> skills;
        
        public EmployeeData(string employeeName, EmployeeRole employeeRole, int level, double monthlySalary)
        {
            name = employeeName;
            role = employeeRole;
            skillLevel = level;
            salary = monthlySalary;
            hiredDate = DateTime.Now;
            skills = new List<string>();
        }
    }
    
    [Serializable]
    public class OfficeData
    {
        public OfficeType type = OfficeType.Garage;
        public int capacity = 1;
        public double rentCost = 0;
        public float productivityMultiplier = 1.0f;
        public List<string> amenities;
        
        public OfficeData()
        {
            amenities = new List<string>();
        }
    }
    
    [Serializable]
    public class InfrastructureUpgrade
    {
        public string name;
        public string description;
        public double cost;
        public float effectMultiplier;
        public DateTime purchaseDate;
        public bool isActive;
    }
    
    public enum EmployeeRole
    {
        Developer,
        Designer,
        ProjectManager,
        QATester,
        DevOps,
        DataScientist,
        Marketing,
        Sales
    }
    
    public enum OfficeType
    {
        Garage,
        HomeOffice,
        CoworkingSpace,
        SmallOffice,
        OpenPlan,
        TechCampus,
        Skyscraper
    }
}