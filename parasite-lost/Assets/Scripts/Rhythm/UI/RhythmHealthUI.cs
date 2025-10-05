using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ParasiteLost.Rhythm.UI
{
    public class RhythmHealthUI : MonoBehaviour
    {
        [Header("Health UI Settings")]
        public GameObject heartPrefab; // Assign a heart sprite prefab in inspector
        public Transform heartsContainer; // Parent transform to hold heart UI elements
        public int maxHearts = 3;
        
        [Header("Heart Positioning")]
        public float heartSpacing = 60f; // Distance between hearts
        public Vector2 heartStartPosition = new Vector2(30f, 30f); // Starting position from bottom-left (x, y offset)
        public Vector2 heartSize = new Vector2(50f, 50f); // Size of each heart
        
        [Header("Heart Animation")]
        public float heartDisappearDuration = 0.5f;
        public AnimationCurve heartDisappearCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        
        private List<GameObject> heartObjects = new List<GameObject>();
        private int currentHearts;
        
        public global::System.Action OnAllHeartsLost;
        
        private void Start()
        {
            InitializeHearts();
        }
        
        private void InitializeHearts()
        {
            currentHearts = maxHearts;
            heartObjects.Clear();
            
            // Clear any existing hearts
            foreach (Transform child in heartsContainer)
            {
                if (child != heartsContainer)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            
            // Create heart UI elements
            for (int i = 0; i < maxHearts; i++)
            {
                GameObject heart = CreateHeartUI(i);
                heartObjects.Add(heart);
            }
        }
        
        private GameObject CreateHeartUI(int index)
        {
            GameObject heart;
            
            if (heartPrefab != null)
            {
                heart = Instantiate(heartPrefab, heartsContainer);
            }
            else
            {
                // Create a simple heart UI if no prefab is assigned
                heart = new GameObject($"Heart_{index}");
                heart.transform.SetParent(heartsContainer);
                
                Image heartImage = heart.AddComponent<Image>();
                // You'll need to assign a heart sprite here or in the inspector
                heartImage.color = Color.red;
            }
            
            // Set up RectTransform for proper positioning
            RectTransform rectTransform = heart.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = heart.AddComponent<RectTransform>();
            }
            
            // Set heart size
            rectTransform.sizeDelta = heartSize;
            
            // Position hearts in bottom-left corner with proper spacing
            float xPosition = heartStartPosition.x + (index * heartSpacing);
            float yPosition = heartStartPosition.y;
            rectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
            
            // Set anchor to bottom-left
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            return heart;
        }
        
        public void LoseHeart()
        {
            if (currentHearts <= 0) return;
            
            currentHearts--;
            
            // Animate the heart disappearing
            if (currentHearts >= 0 && currentHearts < heartObjects.Count)
            {
                GameObject heartToRemove = heartObjects[currentHearts];
                StartCoroutine(AnimateHeartDisappear(heartToRemove));
            }
            
            Debug.Log($"Heart lost! Remaining hearts: {currentHearts}");
            
            // Check if all hearts are lost
            if (currentHearts <= 0)
            {
                OnAllHeartsLost?.Invoke();
                Debug.Log("All hearts lost! Game over!");
            }
        }
        
        private IEnumerator AnimateHeartDisappear(GameObject heart)
        {
            if (heart == null) yield break;
            
            Vector3 originalScale = heart.transform.localScale;
            Image heartImage = heart.GetComponent<Image>();
            Color originalColor = heartImage != null ? heartImage.color : Color.white;
            
            float elapsedTime = 0f;
            
            while (elapsedTime < heartDisappearDuration)
            {
                float progress = elapsedTime / heartDisappearDuration;
                float curveValue = heartDisappearCurve.Evaluate(progress);
                
                // Scale down the heart
                heart.transform.localScale = originalScale * curveValue;
                
                // Fade out the heart
                if (heartImage != null)
                {
                    Color currentColor = originalColor;
                    currentColor.a = curveValue;
                    heartImage.color = currentColor;
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Ensure the heart is completely invisible
            if (heartImage != null)
            {
                Color finalColor = originalColor;
                finalColor.a = 0f;
                heartImage.color = finalColor;
            }
            
            heart.transform.localScale = Vector3.zero;
        }
        
        public void ResetHearts()
        {
            StopAllCoroutines();
            InitializeHearts();
        }
        
        public int GetCurrentHearts()
        {
            return currentHearts;
        }
        
        public bool HasHeartsRemaining()
        {
            return currentHearts > 0;
        }
    }
}