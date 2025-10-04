using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ParasiteLost.Managers;
using System.Collections;

namespace ParasiteLost.UI
{
    public class DeathScreenUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject deathScreenPanel;
        public TextMeshProUGUI deathTitleText;
        public TextMeshProUGUI deathMessageText;
        public Button restartButton;
        public Button mainMenuButton;
        public Image backgroundOverlay;
        
        [Header("Animation Settings")]
        public float fadeInDuration = 1f;
        public float textAppearDelay = 0.5f;
        public float buttonAppearDelay = 1f;
        
        [Header("Text Content")]
        public string deathTitle = "YOU DIED";
        public string deathMessage = "Your parasite's lifespan has expired!\nTry to survive longer by possessing fish.";
        
        private GameManager gameManager;
        private bool isShowing = false;
        
        private void Start()
        {
            gameManager = GameManager.Instance;
            
            // Initialize UI
            if (deathScreenPanel != null)
            {
                deathScreenPanel.SetActive(false);
            }
            
            // Setup button events
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartLevel);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            }
            
            // Initially hide all elements
            SetInitialState();
        }
        
        private void Update()
        {
            // Check if we should show death screen
            if (gameManager?.currentState == GameManager.GameState.GameOver && !isShowing)
            {
                ShowDeathScreen();
            }
            else if (gameManager?.currentState != GameManager.GameState.GameOver && isShowing)
            {
                HideDeathScreen();
            }
        }
        
        private void SetInitialState()
        {
            if (deathScreenPanel != null)
            {
                deathScreenPanel.SetActive(false);
            }
            
            if (backgroundOverlay != null)
            {
                Color overlayColor = backgroundOverlay.color;
                overlayColor.a = 0f;
                backgroundOverlay.color = overlayColor;
            }
            
            if (deathTitleText != null)
            {
                Color titleColor = deathTitleText.color;
                titleColor.a = 0f;
                deathTitleText.color = titleColor;
            }
            
            if (deathMessageText != null)
            {
                Color messageColor = deathMessageText.color;
                messageColor.a = 0f;
                deathMessageText.color = messageColor;
            }
            
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(false);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.gameObject.SetActive(false);
            }
        }
        
        public void ShowDeathScreen()
        {
            if (isShowing) return;
            
            isShowing = true;
            
            if (deathScreenPanel != null)
            {
                // Force activate the panel and all its children
                deathScreenPanel.SetActive(true);
                
                // Force activate all children recursively
                ActivateAllChildren(deathScreenPanel.transform);
                
                // Check if the Canvas is active
                Canvas canvas = deathScreenPanel.GetComponentInParent<Canvas>();
                if (canvas != null && !canvas.gameObject.activeInHierarchy)
                {
                    canvas.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("DeathScreenPanel is null!");
                return;
            }
            
            // Set text content
            if (deathTitleText != null)
            {
                deathTitleText.text = deathTitle;
            }
            
            if (deathMessageText != null)
            {
                deathMessageText.text = deathMessage;
            }
            
            // Show death screen without animation
            ForceShowDeathScreen();
        }
        
        private void ActivateAllChildren(Transform parent)
        {
            // Activate the parent
            parent.gameObject.SetActive(true);
            
            // Activate all children recursively
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                child.gameObject.SetActive(true);
                
                // Recursively activate grandchildren
                ActivateAllChildren(child);
            }
        }
        
        private void ForceShowDeathScreen()
        {
            // Force show all elements without animation
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(true);
                Color overlayColor = backgroundOverlay.color;
                overlayColor.a = 0.8f;
                backgroundOverlay.color = overlayColor;
            }
            
            if (deathTitleText != null)
            {
                deathTitleText.gameObject.SetActive(true);
                Color titleColor = deathTitleText.color;
                titleColor.a = 1f;
                deathTitleText.color = titleColor;
            }
            
            if (deathMessageText != null)
            {
                deathMessageText.gameObject.SetActive(true);
                Color messageColor = deathMessageText.color;
                messageColor.a = 1f;
                deathMessageText.color = messageColor;
            }
            
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.gameObject.SetActive(true);
            }
        }
        
        public void HideDeathScreen()
        {
            if (!isShowing) return;
            
            isShowing = false;
            
            if (deathScreenPanel != null)
            {
                deathScreenPanel.SetActive(false);
            }
            
            SetInitialState();
        }
        
        private IEnumerator FadeInAnimation()
        {
            // Fade in background overlay
            if (backgroundOverlay != null)
            {
                yield return StartCoroutine(FadeImage(backgroundOverlay, 0f, 0.8f, fadeInDuration));
            }
            
            // Wait a bit before showing title
            yield return new WaitForSeconds(textAppearDelay);
            
            // Fade in title text
            if (deathTitleText != null)
            {
                yield return StartCoroutine(FadeText(deathTitleText, 0f, 1f, fadeInDuration * 0.8f));
            }
            
            // Wait a bit before showing message
            yield return new WaitForSeconds(textAppearDelay * 0.5f);
            
            // Fade in message text
            if (deathMessageText != null)
            {
                yield return StartCoroutine(FadeText(deathMessageText, 0f, 1f, fadeInDuration * 0.8f));
            }
            
            // Wait before showing buttons
            yield return new WaitForSeconds(buttonAppearDelay);
            
            // Show buttons
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
                yield return StartCoroutine(FadeButton(restartButton, 0f, 1f, fadeInDuration * 0.6f));
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.gameObject.SetActive(true);
                yield return StartCoroutine(FadeButton(mainMenuButton, 0f, 1f, fadeInDuration * 0.6f));
            }
        }
        
        private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
        {
            if (image == null) yield break;
            
            float elapsed = 0f;
            Color color = image.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                color.a = alpha;
                image.color = color;
                yield return null;
            }
            
            color.a = endAlpha;
            image.color = color;
        }
        
        private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
        {
            if (text == null) yield break;
            
            float elapsed = 0f;
            Color color = text.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                color.a = alpha;
                text.color = color;
                yield return null;
            }
            
            color.a = endAlpha;
            text.color = color;
        }
        
        private IEnumerator FadeButton(Button button, float startAlpha, float endAlpha, float duration)
        {
            if (button == null) yield break;
            
            Image buttonImage = button.GetComponent<Image>();
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                
                if (buttonImage != null)
                {
                    Color imageColor = buttonImage.color;
                    imageColor.a = alpha;
                    buttonImage.color = imageColor;
                }
                
                if (buttonText != null)
                {
                    Color textColor = buttonText.color;
                    textColor.a = alpha;
                    buttonText.color = textColor;
                }
                
                yield return null;
            }
            
            // Ensure final alpha values
            if (buttonImage != null)
            {
                Color imageColor = buttonImage.color;
                imageColor.a = endAlpha;
                buttonImage.color = imageColor;
            }
            
            if (buttonText != null)
            {
                Color textColor = buttonText.color;
                textColor.a = endAlpha;
                buttonText.color = textColor;
            }
        }
        
        public void RestartLevel()
        {
            if (gameManager != null)
            {
                // Reset time scale in case it was paused
                Time.timeScale = 1f;
                gameManager.RestartLevel();
            }
        }
        
        public void GoToMainMenu()
        {
            if (gameManager != null)
            {
                // Reset time scale in case it was paused
                Time.timeScale = 1f;
                
                // Load main menu scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("main");
            }
        }
        
        public void SetDeathMessage(string title, string message)
        {
            deathTitle = title;
            deathMessage = message;
            
            if (deathTitleText != null)
            {
                deathTitleText.text = deathTitle;
            }
            
            if (deathMessageText != null)
            {
                deathMessageText.text = deathMessage;
            }
        }
    }
}
