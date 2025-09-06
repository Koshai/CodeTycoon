using UnityEngine;
using UnityEngine.UI;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Utility to fix ScrollView and Grid Layout issues
    /// </summary>
    public class ScrollViewFixer : MonoBehaviour
    {
        [Header("Fix Options")]
        public bool fixOnStart = true;
        
        private void Start()
        {
            if (fixOnStart)
            {
                FixScrollViews();
            }
        }
        
        [ContextMenu("Fix ScrollViews")]
        public void FixScrollViews()
        {
            Debug.Log("[ScrollViewFixer] Fixing ScrollView layouts...");
            
            // Find LearningScreen
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen != null)
            {
                FixLearningScreenScrollView(learningScreen);
            }
            else
            {
                Debug.LogWarning("[ScrollViewFixer] LearningScreen not found!");
            }
        }
        
        private void FixLearningScreenScrollView(GameObject learningScreen)
        {
            // Find the scroll view
            Transform scrollViewTransform = learningScreen.transform.Find("ConceptScrollView");
            if (scrollViewTransform == null)
            {
                Debug.LogWarning("[ScrollViewFixer] ConceptScrollView not found!");
                return;
            }
            
            GameObject scrollViewGO = scrollViewTransform.gameObject;
            
            // Fix ScrollView RectTransform
            RectTransform scrollRT = scrollViewGO.GetComponent<RectTransform>();
            if (scrollRT != null)
            {
                scrollRT.anchorMin = new Vector2(0.05f, 0.1f);
                scrollRT.anchorMax = new Vector2(0.95f, 0.85f);
                scrollRT.offsetMin = Vector2.zero;
                scrollRT.offsetMax = Vector2.zero;
                Debug.Log("[ScrollViewFixer] Fixed ScrollView size");
            }
            
            // Find Content area
            Transform content = scrollViewTransform.Find("Viewport/Content");
            if (content == null)
            {
                content = scrollViewTransform.Find("Content");
            }
            
            if (content != null)
            {
                FixContentGridLayout(content.gameObject);
            }
            else
            {
                Debug.LogWarning("[ScrollViewFixer] Content area not found!");
            }
        }
        
        private void FixContentGridLayout(GameObject contentGO)
        {
            Debug.Log("[ScrollViewFixer] Fixing Content Grid Layout...");
            
            // Get or add Grid Layout Group
            GridLayoutGroup grid = contentGO.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                grid = contentGO.AddComponent<GridLayoutGroup>();
                Debug.Log("[ScrollViewFixer] Added GridLayoutGroup");
            }
            
            // Configure Grid Layout
            grid.cellSize = new Vector2(300, 120);
            grid.spacing = new Vector2(20, 20);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2; // 2 columns
            
            Debug.Log("[ScrollViewFixer] Configured GridLayoutGroup: 2 columns, 300x120 cells");
            
            // Get or add Content Size Fitter
            ContentSizeFitter fitter = contentGO.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = contentGO.AddComponent<ContentSizeFitter>();
                Debug.Log("[ScrollViewFixer] Added ContentSizeFitter");
            }
            
            // Configure Content Size Fitter
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            Debug.Log("[ScrollViewFixer] Content area should now auto-size vertically");
            
            // Fix RectTransform pivot and anchor
            RectTransform contentRT = contentGO.GetComponent<RectTransform>();
            if (contentRT != null)
            {
                contentRT.anchorMin = new Vector2(0, 1);
                contentRT.anchorMax = new Vector2(1, 1);
                contentRT.pivot = new Vector2(0.5f, 1);
                contentRT.anchoredPosition = Vector2.zero;
            }
        }
        
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 270, 200, 80));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("ScrollView Fixer");
            
            if (GUILayout.Button("Fix ScrollViews"))
            {
                FixScrollViews();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}