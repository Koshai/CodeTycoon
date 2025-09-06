using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Debug utility to inspect UI hierarchy and layout issues
    /// </summary>
    public class UIDebugger : MonoBehaviour
    {
        [Header("Debug Options")]
        public bool showDebugInfo = true;
        
        private void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(220, 10, 400, 600));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("UI Debugger", GUI.skin.label);
            
            if (GUILayout.Button("Debug Learning Screen"))
            {
                DebugLearningScreen();
            }
            
            if (GUILayout.Button("Debug ScrollView"))
            {
                DebugScrollView();
            }
            
            if (GUILayout.Button("Fix Content Layout"))
            {
                FixContentLayout();
            }
            
            if (GUILayout.Button("Force Rebuild Layout"))
            {
                ForceRebuildLayout();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void DebugLearningScreen()
        {
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen == null)
            {
                Debug.LogError("[UIDebugger] LearningScreen not found!");
                return;
            }
            
            Debug.Log("[UIDebugger] === Learning Screen Debug ===");
            Debug.Log($"LearningScreen active: {learningScreen.activeInHierarchy}");
            
            RectTransform screenRT = learningScreen.GetComponent<RectTransform>();
            if (screenRT != null)
            {
                Debug.Log($"LearningScreen size: {screenRT.rect.size}");
                Debug.Log($"LearningScreen anchors: Min({screenRT.anchorMin}) Max({screenRT.anchorMax})");
            }
            
            // Debug children
            Debug.Log($"LearningScreen has {learningScreen.transform.childCount} children:");
            for (int i = 0; i < learningScreen.transform.childCount; i++)
            {
                Transform child = learningScreen.transform.GetChild(i);
                Debug.Log($"  Child {i}: {child.name}");
            }
        }
        
        private void DebugScrollView()
        {
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen == null) return;
            
            Transform scrollViewTransform = learningScreen.transform.Find("ConceptScrollView");
            if (scrollViewTransform == null)
            {
                Debug.LogError("[UIDebugger] ConceptScrollView not found!");
                return;
            }
            
            Debug.Log("[UIDebugger] === ScrollView Debug ===");
            
            GameObject scrollViewGO = scrollViewTransform.gameObject;
            RectTransform scrollRT = scrollViewGO.GetComponent<RectTransform>();
            
            if (scrollRT != null)
            {
                Debug.Log($"ScrollView size: {scrollRT.rect.size}");
                Debug.Log($"ScrollView anchors: Min({scrollRT.anchorMin}) Max({scrollRT.anchorMax})");
                Debug.Log($"ScrollView offsets: Min({scrollRT.offsetMin}) Max({scrollRT.offsetMax})");
            }
            
            // Find content area
            Transform content = FindContentTransform(scrollViewTransform);
            if (content != null)
            {
                DebugContent(content);
            }
            else
            {
                Debug.LogError("[UIDebugger] Content not found in ScrollView!");
                
                // List all children to see structure
                Debug.Log($"ScrollView children:");
                for (int i = 0; i < scrollViewTransform.childCount; i++)
                {
                    Transform child = scrollViewTransform.GetChild(i);
                    Debug.Log($"  {child.name}");
                    
                    for (int j = 0; j < child.childCount; j++)
                    {
                        Transform grandchild = child.GetChild(j);
                        Debug.Log($"    {grandchild.name}");
                    }
                }
            }
        }
        
        private Transform FindContentTransform(Transform scrollView)
        {
            // Try different paths to find Content
            Transform content = scrollView.Find("Viewport/Content");
            if (content != null) return content;
            
            content = scrollView.Find("Content");
            if (content != null) return content;
            
            // Look for any child with "Content" in the name
            for (int i = 0; i < scrollView.childCount; i++)
            {
                Transform child = scrollView.GetChild(i);
                if (child.name.Contains("Content"))
                {
                    return child;
                }
                
                // Check grandchildren
                for (int j = 0; j < child.childCount; j++)
                {
                    Transform grandchild = child.GetChild(j);
                    if (grandchild.name.Contains("Content"))
                    {
                        return grandchild;
                    }
                }
            }
            
            return null;
        }
        
        private void DebugContent(Transform content)
        {
            Debug.Log("[UIDebugger] === Content Debug ===");
            
            RectTransform contentRT = content.GetComponent<RectTransform>();
            if (contentRT != null)
            {
                Debug.Log($"Content size: {contentRT.rect.size}");
                Debug.Log($"Content anchors: Min({contentRT.anchorMin}) Max({contentRT.anchorMax})");
                Debug.Log($"Content pivot: {contentRT.pivot}");
                Debug.Log($"Content position: {contentRT.anchoredPosition}");
            }
            
            // Check Grid Layout Group
            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid != null)
            {
                Debug.Log($"GridLayoutGroup found:");
                Debug.Log($"  Cell Size: {grid.cellSize}");
                Debug.Log($"  Spacing: {grid.spacing}");
                Debug.Log($"  Constraint: {grid.constraint} ({grid.constraintCount})");
                Debug.Log($"  Child Alignment: {grid.childAlignment}");
            }
            else
            {
                Debug.LogWarning("[UIDebugger] No GridLayoutGroup on Content!");
            }
            
            // Check Content Size Fitter
            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            if (fitter != null)
            {
                Debug.Log($"ContentSizeFitter: H({fitter.horizontalFit}) V({fitter.verticalFit})");
            }
            else
            {
                Debug.LogWarning("[UIDebugger] No ContentSizeFitter on Content!");
            }
            
            // Debug child buttons
            Debug.Log($"Content has {content.childCount} children:");
            for (int i = 0; i < content.childCount; i++)
            {
                Transform child = content.GetChild(i);
                RectTransform childRT = child.GetComponent<RectTransform>();
                
                Debug.Log($"  Child {i}: {child.name}");
                if (childRT != null)
                {
                    Debug.Log($"    Size: {childRT.rect.size}");
                    Debug.Log($"    Position: {childRT.anchoredPosition}");
                }
            }
        }
        
        private void FixContentLayout()
        {
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen == null) return;
            
            Transform scrollViewTransform = learningScreen.transform.Find("ConceptScrollView");
            if (scrollViewTransform == null) return;
            
            Transform content = FindContentTransform(scrollViewTransform);
            if (content == null) return;
            
            Debug.Log("[UIDebugger] Fixing Content Layout...");
            
            // Ensure proper components
            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                grid = content.gameObject.AddComponent<GridLayoutGroup>();
                Debug.Log("[UIDebugger] Added GridLayoutGroup");
            }
            
            // Configure grid
            grid.cellSize = new Vector2(300, 120);
            grid.spacing = new Vector2(15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            
            // Ensure Content Size Fitter
            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = content.gameObject.AddComponent<ContentSizeFitter>();
                Debug.Log("[UIDebugger] Added ContentSizeFitter");
            }
            
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Fix RectTransform
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = Vector2.zero;
            
            Debug.Log("[UIDebugger] Content layout fixed!");
        }
        
        private void ForceRebuildLayout()
        {
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen == null) return;
            
            Transform scrollViewTransform = learningScreen.transform.Find("ConceptScrollView");
            if (scrollViewTransform == null) return;
            
            Transform content = FindContentTransform(scrollViewTransform);
            if (content == null) return;
            
            Debug.Log("[UIDebugger] Force rebuilding layout...");
            
            // Force layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewTransform.GetComponent<RectTransform>());
            
            Debug.Log("[UIDebugger] Layout rebuild complete!");
        }
    }
}