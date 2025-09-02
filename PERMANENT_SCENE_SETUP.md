# Permanent Scene Setup Guide

## Step-by-Step Manual Unity Setup (No Runtime Generation)

### 1. Create GameManager Hierarchy

1. **Create Empty GameObject**: Right-click in Hierarchy → Create Empty
2. **Name it**: `GameManager`
3. **Add Component**: `GameManager` script
4. **Create Child Objects** (Right-click GameManager → Create Empty):
   - `UIManager` (add UIManager component)
   - `CompanyManager` (add CompanyManager component)  
   - `LearningSystem` (add LearningSystem component)
   - `AudioManager` (add AudioManager component)

### 2. Create Main Canvas

1. **Right-click in Hierarchy** → UI → Canvas
2. **Name it**: `MainCanvas`
3. **Canvas Component Settings**:
   - Render Mode: `Screen Space - Overlay`
   - Sort Order: `0`
4. **Canvas Scaler Settings**:
   - UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1920 x 1080`
   - Screen Match Mode: `Match Width Or Height`
   - Match: `0.5`
5. **Ensure it has**: `GraphicRaycaster` component

### 3. Create Dashboard Screen

1. **Right-click MainCanvas** → Create Empty
2. **Name**: `Dashboard`
3. **Add Component**: `RectTransform` (should be automatic)
4. **RectTransform Settings**:
   - Anchor Min: `(0, 0)`
   - Anchor Max: `(1, 1)`
   - Left/Right/Top/Bottom: all `0`

#### 3a. Create Header Panel
1. **Right-click Dashboard** → UI → Panel
2. **Name**: `Header`
3. **RectTransform**:
   - Anchor Min: `(0, 0.9)`
   - Anchor Max: `(1, 1)`
   - Left/Right/Top/Bottom: all `0`

#### 3b. Create Resource Display Texts
1. **Right-click Header** → UI → Text - TextMeshPro
2. **Name**: `KnowledgePointsText`
3. **RectTransform**:
   - Anchor Min: `(0.05, 0)`
   - Anchor Max: `(0.35, 1)`
   - Text: `Knowledge: 0 KP`
   - Font Size: `24`

4. **Duplicate for Cash** (Ctrl+D):
   - **Name**: `CashText`
   - Anchor Min: `(0.35, 0)`
   - Anchor Max: `(0.65, 1)`
   - Text: `Cash: $100`

5. **Duplicate for Company Stage**:
   - **Name**: `CompanyStageText`
   - Anchor Min: `(0.65, 0)`
   - Anchor Max: `(0.95, 1)`
   - Text: `Stage: Garage`
   - Font Size: `20`

#### 3c. Create Main Earn Knowledge Button
1. **Right-click Dashboard** → UI → Button - TextMeshPro
2. **Name**: `EarnKnowledgeButton`
3. **RectTransform**:
   - Anchor Min: `(0.3, 0.3)`
   - Anchor Max: `(0.7, 0.7)`
   - Left/Right/Top/Bottom: all `0`
4. **Button Text**:
   - Text: `Earn Knowledge`
   - Font Size: `32`
   - Color: White

#### 3d. Create Navigation Panel
1. **Right-click Dashboard** → UI → Panel
2. **Name**: `NavigationPanel`
3. **RectTransform**:
   - Anchor Min: `(0, 0)`
   - Anchor Max: `(1, 0.15)`

#### 3e. Create Navigation Buttons
1. **Right-click NavigationPanel** → UI → Button - TextMeshPro
2. **Name**: `LearningButton`
3. **RectTransform**:
   - Anchor Min: `(0.05, 0.2)`
   - Anchor Max: `(0.23, 0.8)`
4. **Button Text**: `Learning`

5. **Duplicate 3 more times** for:
   - `ProjectsButton` - Anchor Min: `(0.27, 0.2)`, Max: `(0.48, 0.8)`, Text: `Projects`
   - `CompanyButton` - Anchor Min: `(0.52, 0.2)`, Max: `(0.73, 0.8)`, Text: `Company`  
   - `SettingsButton` - Anchor Min: `(0.77, 0.2)`, Max: `(0.95, 0.8)`, Text: `Settings`

#### 3f. Add DashboardUI Component
1. **Select Dashboard GameObject**
2. **Add Component**: `DashboardUI`
3. **Drag References**:
   - Knowledge Points Text → `KnowledgePointsText`
   - Cash Text → `CashText`
   - Company Stage Text → `CompanyStageText`
   - Earn Knowledge Button → `EarnKnowledgeButton`
   - Learning Button → `LearningButton`
   - Projects Button → `ProjectsButton`
   - Company Button → `CompanyButton`
   - Settings Button → `SettingsButton`

### 4. Create Learning Screen

1. **Right-click MainCanvas** → Create Empty
2. **Name**: `LearningScreen`
3. **Set Active**: `False` (uncheck in inspector)
4. **RectTransform**: Same as Dashboard (full screen)
5. **Add Component**: `Image` (dark background)
   - Color: `(0.1, 0.1, 0.1, 0.9)`

#### 4a. Create Title
1. **Right-click LearningScreen** → UI → Text - TextMeshPro
2. **Name**: `Title`
3. **Text**: `Programming Concepts`
4. **Font Size**: `36`
5. **RectTransform**:
   - Anchor Min: `(0, 0.9)`
   - Anchor Max: `(1, 1)`

#### 4b. Create Concept Grid
1. **Right-click LearningScreen** → UI → Scroll View
2. **Name**: `ConceptScrollView`
3. **RectTransform**:
   - Anchor Min: `(0.1, 0.1)`
   - Anchor Max: `(0.9, 0.85)`

4. **Content GameObject Settings**:
   - **Add Component**: `Grid Layout Group`
     - Cell Size: `(280, 100)`
     - Spacing: `(20, 20)`
     - Child Alignment: `Upper Center`
   - **Content Size Fitter**:
     - Vertical Fit: `Preferred Size`

### 5. Create Other Screens (Projects, Settings)

1. **Duplicate LearningScreen** (Ctrl+D)
2. **Rename to**: `ProjectsScreen`
3. **Change Title Text**: `Projects & Clients`

4. **Duplicate again**:
5. **Rename to**: `SettingsScreen`  
6. **Change Title Text**: `Settings`

### 6. Connect GameManager References

1. **Select GameManager**
2. **In GameManager Component**:
   - UI Manager → Drag `UIManager` child object
   - Company Manager → Drag `CompanyManager` child object
   - Learning System → Drag `LearningSystem` child object
   - Audio Manager → Drag `AudioManager` child object

### 7. Connect UIManager References

1. **Select UIManager**
2. **In UIManager Component**:
   - Main Canvas → Drag `MainCanvas`
   - Main Dashboard → Drag `Dashboard`
   - Learning Screen → Drag `LearningScreen`
   - Projects Screen → Drag `ProjectsScreen`
   - Settings Screen → Drag `SettingsScreen`

### 8. Create EventSystem

1. **Right-click in Hierarchy** → UI → Event System
2. **Leave default settings**

### 9. Save as Prefab (Optional)

1. **Drag GameManager to Project** → Create prefab
2. **Drag MainCanvas to Project** → Create prefab
3. **Save Scene**: File → Save Scene As → `MainGame.scene`

## Final Hierarchy Should Look Like:

```
Scene Hierarchy:
├── GameManager
│   ├── UIManager
│   ├── CompanyManager
│   ├── LearningSystem
│   └── AudioManager
├── MainCanvas
│   ├── Dashboard (DashboardUI component)
│   │   ├── Header
│   │   │   ├── KnowledgePointsText
│   │   │   ├── CashText
│   │   │   └── CompanyStageText
│   │   ├── EarnKnowledgeButton
│   │   └── NavigationPanel
│   │       ├── LearningButton
│   │       ├── ProjectsButton
│   │       ├── CompanyButton
│   │       └── SettingsButton
│   ├── LearningScreen (inactive)
│   │   ├── Title
│   │   └── ConceptScrollView
│   ├── ProjectsScreen (inactive)
│   └── SettingsScreen (inactive)
└── EventSystem
```

This creates a permanent, reusable scene structure that doesn't generate anything at runtime!