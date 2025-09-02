using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CodeTycoon.Core
{
    public class LearningSystem : MonoBehaviour
    {
        [Header("Learning Configuration")]
        [SerializeField] private ProgrammingConceptDatabase conceptDatabase;
        [SerializeField] private bool debugMode = false;
        
        [Header("Concept Costs")]
        [SerializeField] private double baseConceptCost = 10.0;
        [SerializeField] private double costMultiplier = 1.5;
        
        // Events
        public delegate void ConceptUnlocked(string conceptId, ProgrammingConcept concept);
        public static event ConceptUnlocked OnConceptUnlocked;
        
        public delegate void ConceptMastered(string conceptId, ProgrammingConcept concept);
        public static event ConceptMastered OnConceptMastered;
        
        private Dictionary<string, ProgrammingConcept> concepts;
        
        public void Initialize()
        {
            Debug.Log("[LearningSystem] Initializing Learning System...");
            
            InitializeConceptDatabase();
            
            Debug.Log("[LearningSystem] Learning System initialized.");
        }
        
        private void InitializeConceptDatabase()
        {
            concepts = new Dictionary<string, ProgrammingConcept>();
            
            // Create basic programming concepts
            CreateFundamentalConcepts();
            CreateWebDevelopmentConcepts();
            CreateBackendConcepts();
            CreateAdvancedConcepts();
            
            if (debugMode)
            {
                Debug.Log($"[LearningSystem] Initialized {concepts.Count} programming concepts");
            }
        }
        
        private void CreateFundamentalConcepts()
        {
            var variables = new ProgrammingConcept(
                "Variables",
                "Learn about storing and manipulating data",
                ConceptCategory.Fundamentals,
                ConceptDifficulty.Beginner,
                new List<string>(), // No prerequisites
                "Variables are containers for storing data values. They have names and can hold different types of information like numbers, text, or boolean values."
            );
            concepts["Variables"] = variables;
            
            var functions = new ProgrammingConcept(
                "Functions",
                "Organize code into reusable blocks",
                ConceptCategory.Fundamentals,
                ConceptDifficulty.Beginner,
                new List<string> { "Variables" },
                "Functions are blocks of code that perform specific tasks. They help organize your code and make it reusable."
            );
            concepts["Functions"] = functions;
            
            var loops = new ProgrammingConcept(
                "Loops",
                "Repeat code efficiently",
                ConceptCategory.Fundamentals,
                ConceptDifficulty.Beginner,
                new List<string> { "Variables", "Functions" },
                "Loops allow you to repeat blocks of code multiple times, making your programs more efficient."
            );
            concepts["Loops"] = loops;
            
            var debugging = new ProgrammingConcept(
                "Debugging",
                "Find and fix errors in code",
                ConceptCategory.Fundamentals,
                ConceptDifficulty.Intermediate,
                new List<string> { "Variables", "Functions" },
                "Debugging is the process of finding and fixing errors (bugs) in your code."
            );
            concepts["Debugging"] = debugging;
            
            var oop = new ProgrammingConcept(
                "OOP",
                "Object-Oriented Programming principles",
                ConceptCategory.Fundamentals,
                ConceptDifficulty.Intermediate,
                new List<string> { "Functions", "Loops" },
                "Object-Oriented Programming organizes code into classes and objects, making it more modular and maintainable."
            );
            concepts["OOP"] = oop;
        }
        
        private void CreateWebDevelopmentConcepts()
        {
            var html = new ProgrammingConcept(
                "HTML",
                "Structure web pages with markup",
                ConceptCategory.WebDevelopment,
                ConceptDifficulty.Beginner,
                new List<string> { "Variables" },
                "HTML (HyperText Markup Language) is used to create the structure and content of web pages."
            );
            concepts["HTML"] = html;
            
            var css = new ProgrammingConcept(
                "CSS",
                "Style and layout web pages",
                ConceptCategory.WebDevelopment,
                ConceptDifficulty.Beginner,
                new List<string> { "HTML" },
                "CSS (Cascading Style Sheets) is used to control the visual presentation of web pages."
            );
            concepts["CSS"] = css;
            
            var javascript = new ProgrammingConcept(
                "JavaScript",
                "Add interactivity to web pages",
                ConceptCategory.WebDevelopment,
                ConceptDifficulty.Intermediate,
                new List<string> { "HTML", "CSS", "Functions" },
                "JavaScript adds dynamic behavior and interactivity to web pages and applications."
            );
            concepts["JavaScript"] = javascript;
        }
        
        private void CreateBackendConcepts()
        {
            var database = new ProgrammingConcept(
                "Database",
                "Store and retrieve data efficiently",
                ConceptCategory.Backend,
                ConceptDifficulty.Intermediate,
                new List<string> { "Functions", "OOP" },
                "Databases store, organize, and retrieve large amounts of data efficiently."
            );
            concepts["Database"] = database;
            
            var api = new ProgrammingConcept(
                "API Integration",
                "Connect systems and services",
                ConceptCategory.Backend,
                ConceptDifficulty.Intermediate,
                new List<string> { "Functions", "Database" },
                "APIs (Application Programming Interfaces) allow different software systems to communicate with each other."
            );
            concepts["API Integration"] = api;
            
            var security = new ProgrammingConcept(
                "Security",
                "Protect applications from threats",
                ConceptCategory.Backend,
                ConceptDifficulty.Advanced,
                new List<string> { "Database", "API Integration" },
                "Security involves protecting applications and data from various threats and vulnerabilities."
            );
            concepts["Security"] = security;
        }
        
        private void CreateAdvancedConcepts()
        {
            var architecture = new ProgrammingConcept(
                "Architecture",
                "Design scalable system structures",
                ConceptCategory.Advanced,
                ConceptDifficulty.Advanced,
                new List<string> { "OOP", "Database", "Security" },
                "Software architecture involves designing the overall structure and organization of complex systems."
            );
            concepts["Architecture"] = architecture;
            
            var testing = new ProgrammingConcept(
                "Testing",
                "Ensure code quality and reliability",
                ConceptCategory.Advanced,
                ConceptDifficulty.Intermediate,
                new List<string> { "Functions", "Debugging" },
                "Testing involves creating automated checks to ensure your code works correctly and reliably."
            );
            concepts["Testing"] = testing;
            
            var ai = new ProgrammingConcept(
                "AI",
                "Machine learning and artificial intelligence",
                ConceptCategory.Advanced,
                ConceptDifficulty.Expert,
                new List<string> { "Architecture", "Database", "Testing" },
                "Artificial Intelligence involves creating systems that can learn, reason, and make decisions."
            );
            concepts["AI"] = ai;
        }
        
        public bool CanUnlockConcept(string conceptId)
        {
            if (!concepts.ContainsKey(conceptId)) return false;
            
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) return false;
            
            // Check if already unlocked
            if (gameData.learningProgress.IsConceptUnlocked(conceptId)) return false;
            
            // Check prerequisites
            ProgrammingConcept concept = concepts[conceptId];
            foreach (string prerequisite in concept.prerequisites)
            {
                if (!gameData.learningProgress.IsConceptUnlocked(prerequisite))
                {
                    return false;
                }
            }
            
            // Check if player has enough knowledge points
            double cost = GetConceptCost(conceptId);
            return gameData.knowledgePoints >= cost;
        }
        
        public bool TryUnlockConcept(string conceptId)
        {
            if (!CanUnlockConcept(conceptId)) return false;
            
            GameData gameData = GameManager.Instance.GetGameData();
            double cost = GetConceptCost(conceptId);
            
            if (gameData.SpendKnowledgePoints(cost))
            {
                gameData.learningProgress.UnlockConcept(conceptId);
                
                ProgrammingConcept concept = concepts[conceptId];
                OnConceptUnlocked?.Invoke(conceptId, concept);
                
                Debug.Log($"[LearningSystem] Unlocked concept: {conceptId} for {cost} KP");
                return true;
            }
            
            return false;
        }
        
        public double GetConceptCost(string conceptId)
        {
            if (!concepts.ContainsKey(conceptId)) return 0;
            
            ProgrammingConcept concept = concepts[conceptId];
            
            // Base cost modified by difficulty and prerequisites
            double cost = baseConceptCost;
            
            // Difficulty multiplier
            switch (concept.difficulty)
            {
                case ConceptDifficulty.Beginner: cost *= 1.0; break;
                case ConceptDifficulty.Intermediate: cost *= 2.0; break;
                case ConceptDifficulty.Advanced: cost *= 4.0; break;
                case ConceptDifficulty.Expert: cost *= 8.0; break;
            }
            
            // Prerequisites multiplier
            cost *= Mathf.Pow((float)costMultiplier, concept.prerequisites.Count);
            
            return cost;
        }
        
        public List<string> GetAvailableConceptsToUnlock()
        {
            List<string> available = new List<string>();
            
            foreach (var kvp in concepts)
            {
                if (CanUnlockConcept(kvp.Key))
                {
                    available.Add(kvp.Key);
                }
            }
            
            // Sort by cost (cheapest first)
            available.Sort((a, b) => GetConceptCost(a).CompareTo(GetConceptCost(b)));
            
            return available;
        }
        
        public ProgrammingConcept GetConcept(string conceptId)
        {
            return concepts.ContainsKey(conceptId) ? concepts[conceptId] : null;
        }
        
        public List<ProgrammingConcept> GetConceptsByCategory(ConceptCategory category)
        {
            return concepts.Values.Where(c => c.category == category).ToList();
        }
        
        public void CompleteCodingChallenge(string conceptId, bool successful, float difficulty = 1.0f)
        {
            if (GameManager.Instance == null) return;
            
            GameData gameData = GameManager.Instance.GetGameData();
            gameData.learningProgress.CompleteChallengeForConcept(conceptId, successful, difficulty);
            
            // Check if concept is now mastered
            if (successful && gameData.learningProgress.IsConceptMastered(conceptId))
            {
                ProgrammingConcept concept = GetConcept(conceptId);
                OnConceptMastered?.Invoke(conceptId, concept);
                
                Debug.Log($"[LearningSystem] Mastered concept: {conceptId}!");
            }
        }
        
        public List<string> GetMissingPrerequisites(string conceptId)
        {
            if (!concepts.ContainsKey(conceptId)) return new List<string>();
            
            GameData gameData = GameManager.Instance?.GetGameData();
            if (gameData == null) return new List<string>();
            
            ProgrammingConcept concept = concepts[conceptId];
            List<string> missing = new List<string>();
            
            foreach (string prerequisite in concept.prerequisites)
            {
                if (!gameData.learningProgress.IsConceptUnlocked(prerequisite))
                {
                    missing.Add(prerequisite);
                }
            }
            
            return missing;
        }
    }
    
    [System.Serializable]
    public class ProgrammingConcept
    {
        public string name;
        public string description;
        public ConceptCategory category;
        public ConceptDifficulty difficulty;
        public List<string> prerequisites;
        public string explanation;
        
        public ProgrammingConcept(string conceptName, string conceptDesc, ConceptCategory conceptCategory, 
            ConceptDifficulty conceptDifficulty, List<string> conceptPrereqs, string conceptExplanation)
        {
            name = conceptName;
            description = conceptDesc;
            category = conceptCategory;
            difficulty = conceptDifficulty;
            prerequisites = conceptPrereqs ?? new List<string>();
            explanation = conceptExplanation;
        }
    }
    
    public enum ConceptCategory
    {
        Fundamentals,
        WebDevelopment,
        Backend,
        Mobile,
        Cloud,
        AI,
        Advanced
    }
    
    public enum ConceptDifficulty
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
    
    [System.Serializable]
    public class ProgrammingConceptDatabase
    {
        public List<ProgrammingConcept> concepts = new List<ProgrammingConcept>();
    }
}