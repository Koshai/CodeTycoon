using UnityEngine;
using UnityEngine.UI;

namespace CodeTycoon.Core
{
    /// <summary>
    /// Fixes scrollbar width and positioning issues
    /// </summary>
    public class ScrollbarFixer : MonoBehaviour
    {
        [Header("Scrollbar Settings")]
        public float scrollbarWidth = 10f;
        public bool hideScrollbarWhenNotNeeded = true;
        
        private void Start()
        {
            FixScrollbars();
        }
        
        [ContextMenu("Fix Scrollbars")]
        public void FixScrollbars()
        {
            Debug.Log("[ScrollbarFixer] Fixing scrollbar issues...");
            
            GameObject learningScreen = GameObject.Find("LearningScreen");
            if (learningScreen == null) return;
            
            Transform scrollView = learningScreen.transform.Find("ConceptScrollView");
            if (scrollView == null) return;
            
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            if (scrollRect == null) return;
            
            FixVerticalScrollbar(scrollRect);
            AdjustContentMargins(scrollView);
        }
        
        private void FixVerticalScrollbar(ScrollRect scrollRect)
        {
            // Find or create vertical scrollbar
            Scrollbar verticalScrollbar = scrollRect.verticalScrollbar;
            
            if (verticalScrollbar == null)
            {
                // Look for existing scrollbar in children
                Transform scrollbarTransform = scrollRect.transform.Find("Scrollbar Vertical");
                if (scrollbarTransform != null)
                {
                    verticalScrollbar = scrollbarTransform.GetComponent<Scrollbar>();
                    scrollRect.verticalScrollbar = verticalScrollbar;
                }
            }
            
            if (verticalScrollbar != null)
            {
                RectTransform scrollbarRT = verticalScrollbar.GetComponent<RectTransform>();
                
                // Make scrollbar thinner and position it properly
                scrollbarRT.sizeDelta = new Vector2(scrollbarWidth, scrollbarRT.sizeDelta.y);
                
                // Position scrollbar on the right edge, outside content area
                scrollbarRT.anchorMin = new Vector2(1, 0);
                scrollbarRT.anchorMax = new Vector2(1, 1);
                scrollbarRT.anchoredPosition = new Vector2(scrollbarWidth / 2, 0);
                
                Debug.Log($"[ScrollbarFixer] Fixed scrollbar width to {scrollbarWidth}px");
            }
            else
            {
                // Hide scrollbar entirely if not found
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                Debug.Log("[ScrollbarFixer] Set scrollbar to auto-hide");
            }
            
            // Configure scrollbar behavior
            if (hideScrollbarWhenNotNeeded)
            {
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            }
        }
        
        private void AdjustContentMargins(Transform scrollView)
        {
            // Adjust viewport to account for scrollbar space
            Transform viewport = scrollView.Find("Viewport");
            if (viewport != null)
            {
                RectTransform viewportRT = viewport.GetComponent<RectTransform>();
                
                // Add right margin for scrollbar
                viewportRT.offsetMin = new Vector2(5, 5); // Left, Bottom margins
                viewportRT.offsetMax = new Vector2(-(scrollbarWidth + 5), -5); // Right, Top margins
                
                Debug.Log("[ScrollbarFixer] Adjusted viewport margins for scrollbar");
            }
        }
        
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 470, 200, 80));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Scrollbar Fixer");
            
            if (GUILayout.Button("Fix Scrollbars"))
            {
                FixScrollbars();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}