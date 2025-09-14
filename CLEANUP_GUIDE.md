# Code Tycoon - File Cleanup Guide

## 🗑️ Files to Remove (No Longer Needed)

### **Debugging/Setup Utilities (Remove After Testing)**
These were created for development and can be removed once everything works:

**Safe to Delete:**
- `Assets/Scripts/Utilities/UIDebugger.cs` - Used for debugging UI issues
- `Assets/Scripts/Utilities/ScrollViewRebuilder.cs` - Used for rebuilding broken ScrollViews  
- `Assets/Scripts/Utilities/ScrollViewFixer.cs` - Fixed ScrollView layout issues
- `Assets/Scripts/Utilities/ScrollbarFixer.cs` - Fixed scrollbar width issues
- `Assets/Scripts/Utilities/QuickUIFixer.cs` - Added back buttons automatically

**Consider Keeping (Useful for Development):**
- `Assets/Scripts/Utilities/SaveDataResetter.cs` - **KEEP** - Useful for testing with fresh saves
- `Assets/Scripts/Utilities/ProgressionTracker.cs` - **KEEP** - Shows helpful progression info
- `Assets/Scripts/Utilities/ChallengePopupCreator.cs` - **KEEP** - Creates challenge UI

### **Obsolete UI Components**
- `Assets/Scripts/UI/ConceptButton.cs` - **REMOVE** - Replaced by EnhancedConceptButton.cs

## 🎯 **Recommended Action Plan:**

### **Phase 1: Remove Debug Utilities**
Once your game works properly, delete these files:
```
UIDebugger.cs
ScrollViewRebuilder.cs  
ScrollViewFixer.cs
ScrollbarFixer.cs
QuickUIFixer.cs
```

### **Phase 2: Remove Old UI Components** 
```
ConceptButton.cs (replaced by EnhancedConceptButton.cs)
```

### **Keep for Production:**
```
SaveDataResetter.cs - Useful for testing
ProgressionTracker.cs - Good for development/debugging
ChallengePopupCreator.cs - Needed to create challenge UI
```

## 📂 **Final Clean Project Structure:**

### **Core Scripts (Keep All):**
```
Assets/Scripts/
├── Core/
│   └── GameManager.cs
├── Managers/
│   ├── UIManager.cs
│   ├── CompanyManager.cs
│   └── AudioManager.cs
├── Data/
│   ├── GameData.cs
│   ├── CompanyData.cs
│   ├── LearningProgress.cs
│   ├── ProjectData.cs
│   └── GameSettings.cs
├── Systems/
│   ├── SaveSystem.cs
│   ├── LearningSystem.cs
│   └── CodingChallengeManager.cs
├── UI/
│   ├── DashboardUI.cs
│   └── EnhancedConceptButton.cs ✅
└── Utilities/
    ├── SaveDataResetter.cs ✅
    ├── ProgressionTracker.cs ✅
    └── ChallengePopupCreator.cs ✅
```

## 🚀 **Manual Cleanup Steps:**

1. **Test your game thoroughly first**
2. **Delete the debug utilities** (UIDebugger, ScrollViewFixer, etc.)
3. **Remove ConceptButton.cs** (replaced by EnhancedConceptButton.cs)
4. **Keep the 3 useful utilities** for ongoing development

This reduces the project from **8 utility files** down to **3 essential ones**.

## ⚠️ **Important Notes:**
- Don't delete anything until you've confirmed the game works
- SaveDataResetter is very useful for testing
- ProgressionTracker helps you see what's happening in the game
- ChallengePopupCreator is needed to set up the challenge UI

The cleanup can wait until after you test the enhanced learning system!