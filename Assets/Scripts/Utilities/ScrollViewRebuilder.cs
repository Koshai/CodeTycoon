using UnityEngine;
using UnityEngine.UI;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Completely rebuilds the ScrollView with proper structure
    /// </summary>
    public class ScrollViewRebuilder : MonoBehaviour
    {
        [Header("Rebuild Options")]
        public bool rebuildOnStart = false;
        
        private void Start()
        {
            if (rebuildOnStart)
            {
                RebuildLearningScreenScrollView();
            }
        }
        
        [ContextMenu("Rebuild ScrollView")]
        public void RebuildLearningScreenScrollView()
        {
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen == null)
            {
                Debug.LogError("[ScrollViewRebuilder] LearningScreen not found!");
                return;
            }
            
            Debug.Log("[ScrollViewRebuilder] Rebuilding Learning Screen ScrollView...");
            
            // Remove existing scroll view if it exists
            Transform existingScrollView = learningScreen.transform.Find("ConceptScrollView");
            if (existingScrollView != null)
            {
                if (Application.isPlaying)
                    DestroyImmediate(existingScrollView.gameObject);
                else
                    DestroyImmediate(existingScrollView.gameObject);
            }
            
            // Create new ScrollView structure
            CreateScrollView(learningScreen);
            
            Debug.Log("[ScrollViewRebuilder] ScrollView rebuilt successfully!");
        }
        
        private void CreateScrollView(GameObject parentScreen)
        {
            // Create ScrollView root
            GameObject scrollViewGO = new GameObject("ConceptScrollView");
            scrollViewGO.transform.SetParent(parentScreen.transform, false);
            
            // Setup ScrollView RectTransform
            RectTransform scrollRT = scrollViewGO.AddComponent<RectTransform>();
            scrollRT.anchorMin = new Vector2(0.05f, 0.1f);
            scrollRT.anchorMax = new Vector2(0.95f, 0.85f);
            scrollRT.offsetMin = Vector2.zero;
            scrollRT.offsetMax = Vector2.zero;
            
            // Add ScrollRect component
            ScrollRect scrollRect = scrollViewGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;
            scrollRect.scrollSensitivity = 30f;
            
            // Add background image
            Image scrollBG = scrollViewGO.AddComponent<Image>();
            scrollBG.color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
            
            // Create Viewport
            GameObject viewportGO = new GameObject("Viewport");
            viewportGO.transform.SetParent(scrollViewGO.transform, false);
            
            RectTransform viewportRT = viewportGO.AddComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.offsetMin = Vector2.zero;
            viewportRT.offsetMax = Vector2.zero;
            
            // Add Mask to Viewport
            Mask viewportMask = viewportGO.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            // Add Image to Viewport for masking
            Image viewportImage = viewportGO.AddComponent<Image>();
            viewportImage.color = Color.clear;
            
            // Create Content
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(viewportGO.transform, false);
            
            RectTransform contentRT = contentGO.AddComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = Vector2.zero;
            contentRT.sizeDelta = new Vector2(0, 600); // Initial height
            
            // Add GridLayoutGroup to Content
            GridLayoutGroup gridLayout = contentGO.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(300, 120);
            gridLayout.spacing = new Vector2(15, 15);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 2;
            
            // Add ContentSizeFitter to Content
            ContentSizeFitter contentFitter = contentGO.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Connect ScrollRect references
            scrollRect.viewport = viewportRT;
            scrollRect.content = contentRT;
            
            Debug.Log("[ScrollViewRebuilder] Created complete ScrollView structure:");
            Debug.Log($"  ScrollView size: {scrollRT.rect.size}");
            Debug.Log($"  Grid Layout: 2 columns, 300x120 cells, 15px spacing");
            Debug.Log($"  Content will auto-size vertically");
        }
        
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 360, 200, 100));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("ScrollView Rebuilder");
            
            if (GUILayout.Button("Rebuild ScrollView"))
            {
                RebuildLearningScreenScrollView();
            }
            
            if (GUILayout.Button("Test Populate"))
            {
                TestPopulateButtons();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void TestPopulateButtons()
        {
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen == null) return;
            
            Transform content = learningScreen.transform.Find("ConceptScrollView/Viewport/Content");
            if (content == null)
            {
                Debug.LogError("[ScrollViewRebuilder] Content not found!");
                return;
            }
            
            // Clear existing buttons
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(content.GetChild(i).gameObject);
                else
                    DestroyImmediate(content.GetChild(i).gameObject);
            }
            
            // Create test buttons
            string[] testConcepts = { "Variables", "Functions", "Loops", "Debugging", "OOP", "HTML", "CSS", "JavaScript" };
            
            foreach (string concept in testConcepts)
            {
                CreateTestButton(concept, content);
            }
            
            Debug.Log($"[ScrollViewRebuilder] Created {testConcepts.Length} test buttons");
        }
        
        private void CreateTestButton(string conceptName, Transform parent)
        {
            GameObject buttonGO = new GameObject($"Test_{conceptName}");
            buttonGO.transform.SetParent(parent, false);
            
            // Add Image background
            Image bg = buttonGO.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.8f, 0.8f);
            
            // Add Button component
            Button button = buttonGO.AddComponent<Button>();
            
            // Add text child
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform textRT = textGO.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            
            TMPro.TextMeshProUGUI text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = $"{conceptName}\nCost: 10 KP";
            text.color = Color.white;
            text.fontSize = 14;
            text.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }
}