# Code Tycoon Unity Setup Instructions

## Phase 1: Unity Project Setup

### Step 1: Create Unity Project
1. Open **Unity Hub**
2. Click **New Project**
3. Select **2D (Built-in Render Pipeline)** template
4. Set Project Name: **CodeTycoon**
5. Set Location: **D:\UnityProjects\CodeTycoon**
6. Click **Create Project**

### Step 2: Install Required Packages
1. Open **Window** → **Package Manager**
2. Install these packages:
   - **TextMeshPro** (if not already installed)
   - **Newtonsoft Json** (com.unity.nuget.newtonsoft-json)
   
**For Newtonsoft Json:**
- In Package Manager, click **+ (Plus)** button
- Select **Add package by name**
- Enter: `com.unity.nuget.newtonsoft-json`
- Click **Add**

### Step 3: Copy Scripts to Unity
1. Copy all the scripts from the generated folders into your Unity project:
   ```
   D:\UnityProjects\CodeTycoon\Assets\Scripts\
   ```
2. Unity will automatically import and compile them

### Step 4: Create Basic Scene
1. Create a new scene: **File** → **New Scene**
2. Save it as: **Assets/Scenes/MainGame.scene**

### Step 5: Auto-Setup Scene Structure
1. Create an empty **GameObject** in the scene
2. Add the **SceneSetupHelper** component to it
3. Check these options in the inspector:
   - ✅ Setup On Start
   - ✅ Create Game Manager
   - ✅ Create Canvas
   - ✅ Create Basic UI
4. **Press Play** - the scene will auto-generate!
5. **Stop Play Mode** - the GameObjects will remain

### Step 6: Manual Setup (Alternative to Step 5)
If auto-setup doesn't work, create manually:

#### Create GameManager
1. Create empty GameObject named **GameManager**
2. Add **GameManager** component
3. Create child GameObjects:
   - **UIManager** (add UIManager component)
   - **CompanyManager** (add CompanyManager component)  
   - **LearningSystem** (add LearningSystem component)
   - **AudioManager** (add AudioManager component)

#### Create Canvas
1. Right-click in Hierarchy → **UI** → **Canvas**
2. Name it **MainCanvas**
3. Set Canvas Scaler:
   - UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: **1920 x 1080**
   - Screen Match Mode: **Match Width Or Height**
   - Match: **0.5**

#### Create EventSystem
1. Right-click in Hierarchy → **UI** → **Event System**

### Step 7: Connect References
1. Select **GameManager** in Hierarchy
2. In the **GameManager** component, drag the child managers to their respective slots:
   - UI Manager → **UIManager** child
   - Company Manager → **CompanyManager** child
   - Learning System → **LearningSystem** child
   - Audio Manager → **AudioManager** child

### Step 8: Test the Game
1. Press **Play**
2. You should see:
   - Dashboard with resource display (Knowledge Points, Cash, Company Stage)
   - Large **"Earn Knowledge"** button in center
   - Navigation buttons at bottom
   - Console messages showing system initialization

### Step 9: Test Basic Gameplay
1. Click the **"Earn Knowledge"** button
2. Watch Knowledge Points increase
3. Console should show KP gain messages
4. Try clicking **"Learning"** button (should show learning screen)

## Troubleshooting

### Common Issues:

**Scripts won't compile:**
- Make sure **Newtonsoft Json** package is installed
- Check Console for specific errors
- Ensure all script files are in correct folders

**UI doesn't appear:**
- Check Canvas is set to **Screen Space - Overlay**
- Verify Canvas Scaler settings
- Make sure EventSystem exists in scene

**GameManager errors:**
- Ensure all manager child objects exist
- Check component references are assigned
- Look for null reference exceptions in Console

**No click responses:**
- Verify EventSystem exists
- Check button components have **GraphicRaycaster**
- Ensure Canvas has **GraphicRaycaster** component

### Success Criteria
✅ Game starts without errors  
✅ Can click "Earn Knowledge" button  
✅ Knowledge Points increase when clicked  
✅ Can navigate between screens  
✅ Console shows system initialization messages  

## Next Steps
Once basic setup works:
1. Test unlocking first concept (Variables) when you have 10+ KP
2. Verify save/load system works (game state persists between sessions)
3. Begin Phase 2: Expanding the learning system

## File Structure Check
Your project should look like this:
```
Assets/
├── Scenes/
│   └── MainGame.scene
├── Scripts/
│   ├── Core/
│   │   └── GameManager.cs
│   ├── Managers/
│   │   ├── UIManager.cs
│   │   ├── CompanyManager.cs
│   │   └── AudioManager.cs
│   ├── Data/
│   │   ├── GameData.cs
│   │   ├── CompanyData.cs
│   │   ├── LearningProgress.cs
│   │   ├── ProjectData.cs
│   │   └── GameSettings.cs
│   ├── Systems/
│   │   ├── SaveSystem.cs
│   │   └── LearningSystem.cs
│   ├── UI/
│   │   ├── DashboardUI.cs
│   │   └── ConceptButton.cs
│   └── Utilities/
│       └── SceneSetupHelper.cs
```

## Ready to Code!
The foundation is now complete. You have a working idle/incremental game with:
- ✅ Save/Load system
- ✅ Basic UI with click-to-earn mechanics  
- ✅ Learning system with concept unlocking
- ✅ Company progression system
- ✅ Audio management
- ✅ Expandable architecture for Phase 2+

Time to start adding more programming concepts and coding challenges!