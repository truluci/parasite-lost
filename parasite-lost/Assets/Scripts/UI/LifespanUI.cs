using UnityEngine;
using TMPro;
using ParasiteLost.Core.Player;
using ParasiteLost.Managers;
using System.Collections;

namespace ParasiteLost.UI
{
    public class LifespanUI : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI lifespanText;
        
        [Header("Visual Settings")]
        public Color normalColor = Color.white;
        public Color warningColor = Color.yellow;
        public Color dangerColor = Color.red;
        public float warningThreshold = 2f; // 2 seconds remaining
        public float dangerThreshold = 1f;  // 1 second remaining
        
        [Header("Animation Settings")]
        public float pulseSpeed = 3f;
        public float pulseIntensity = 0.5f;
        public bool enablePulseEffect = true;
        public float fontSize = 48f;
        public float warningFontSize = 56f;
        public float dangerFontSize = 64f;
        
        private ParasiteController parasiteController;
        private GameManager gameManager;
        private Coroutine pulseCoroutine;
        private bool isPulsing = false;
        
        private void Start()
        {
            // Find the parasite controller
            parasiteController = FindFirstObjectByType<ParasiteController>();
            gameManager = GameManager.Instance;
            
            // Initialize text
            if (lifespanText != null)
            {
                lifespanText.fontSize = fontSize;
                lifespanText.color = normalColor;
                lifespanText.text = "5"; // Set initial text
            }
            
            // Make sure the UI is visible initially and stays visible
            gameObject.SetActive(true);
            
        }
        
        private void OnEnable()
        {
            // Ensure UI is always visible when enabled
            gameObject.SetActive(true);
        }
        
        private void Update()
        {
            UpdateLifespanDisplay();
        }
        
        private void UpdateLifespanDisplay()
        {
            // Keep UI always visible for now - simplified for debugging
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            
            // Try to find ParasiteController if not found yet
            if (parasiteController == null)
            {
                parasiteController = FindFirstObjectByType<ParasiteController>();
                if (parasiteController == null)
                {
                    // Show "NO PARASITE" text if still not found
                    if (lifespanText != null)
                    {
                        lifespanText.text = "NO PARASITE";
                        lifespanText.color = Color.red;
                    }
                    return;
                }
            }
            
            float currentLifespan = parasiteController.currentLifespan;
            
            // Update text with countdown
            if (lifespanText != null)
            {
                // Show countdown from 5 to 0
                int countdown = Mathf.CeilToInt(currentLifespan);
                countdown = Mathf.Clamp(countdown, 0, 5);
                lifespanText.text = countdown.ToString();
            }
            
            // Update color and effects based on remaining lifespan
            UpdateVisualEffects(currentLifespan);
        }
        
        private void UpdateVisualEffects(float currentLifespan)
        {
            if (lifespanText == null) return;
            
            Color targetColor;
            float targetFontSize;
            bool shouldPulse = false;
            
            if (currentLifespan <= dangerThreshold)
            {
                targetColor = dangerColor;
                targetFontSize = dangerFontSize;
                shouldPulse = true;
            }
            else if (currentLifespan <= warningThreshold)
            {
                targetColor = warningColor;
                targetFontSize = warningFontSize;
                shouldPulse = true;
            }
            else
            {
                targetColor = normalColor;
                targetFontSize = fontSize;
                shouldPulse = false;
            }
            
            // Apply color and font size
            lifespanText.color = targetColor;
            lifespanText.fontSize = targetFontSize;
            
            // Handle pulsing effect
            if (shouldPulse && enablePulseEffect && !isPulsing)
            {
                StartPulseEffect();
            }
            else if (!shouldPulse && isPulsing)
            {
                StopPulseEffect();
            }
        }
        
        private void StartPulseEffect()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
            }
            pulseCoroutine = StartCoroutine(PulseEffect());
            isPulsing = true;
        }
        
        private void StopPulseEffect()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
            isPulsing = false;
            
            // Reset to normal alpha
            if (lifespanText != null)
            {
                Color currentColor = lifespanText.color;
                currentColor.a = 1f;
                lifespanText.color = currentColor;
            }
        }
        
        private IEnumerator PulseEffect()
        {
            while (isPulsing)
            {
                float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
                float alpha = Mathf.Lerp(1f - pulseIntensity, 1f, pulse);
                
                if (lifespanText != null)
                {
                    Color currentColor = lifespanText.color;
                    currentColor.a = alpha;
                    lifespanText.color = currentColor;
                }
                
                yield return null;
            }
        }
        
        public void ShowLifespanBonus(float bonusAmount)
        {
            if (lifespanText != null)
            {
                StartCoroutine(ShowBonusEffect(bonusAmount));
            }
        }
        
        private IEnumerator ShowBonusEffect(float bonusAmount)
        {
            if (lifespanText == null) yield break;
            
            string originalText = lifespanText.text;
            Color originalColor = lifespanText.color;
            float originalFontSize = lifespanText.fontSize;
            
            // Show bonus text
            lifespanText.text = $"+{bonusAmount:F0}s!";
            lifespanText.color = Color.green;
            lifespanText.fontSize = fontSize;
            
            // Animate the text
            float duration = 2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                Color currentColor = lifespanText.color;
                currentColor.a = alpha;
                lifespanText.color = currentColor;
                
                yield return null;
            }
            
            // Restore original text
            lifespanText.text = originalText;
            lifespanText.color = originalColor;
            lifespanText.fontSize = originalFontSize;
        }
        
        public void ResetUI()
        {
            if (lifespanText != null)
            {
                lifespanText.color = normalColor;
                lifespanText.fontSize = fontSize;
            }
            
            StopPulseEffect();
        }
    }
}
