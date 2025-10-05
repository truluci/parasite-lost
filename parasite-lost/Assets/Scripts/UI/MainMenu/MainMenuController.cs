using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace ParasiteLost.UI.MainMenu
{
    /// <summary>
    /// Controls the Main Menu: Start Game (plays intro animation then loads level1),
    /// Quit, and optional Skip Intro functionality.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI References")] 
        public Button startButton;
        public Button quitButton;
        public GameObject introPanel;              // Fullscreen panel (disabled by default) containing the intro image & skip text
        public AnimatedImagePlayer introPlayer;    // Handles the frame-by-frame animation
        public TextMeshProUGUI skipHintText;       // Optional: "Press Space to Skip"

        [Header("Settings")] 
        public string levelSceneName = "level1";   // Target scene to load
        public bool allowSkip = true;              // Can the player skip the intro
        public KeyCode skipKey = KeyCode.Space;
        public bool disableButtonsDuringIntro = true;

        private bool isIntroPlaying = false;
        private bool hasStartRequested = false;

        private void Awake()
        {
            if (introPanel != null)
            {
                introPanel.SetActive(false); // Keep hidden until we actually play intro
            }

            if (skipHintText != null)
            {
                skipHintText.gameObject.SetActive(false);
            }

            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
            }
        }

        private void Update()
        {
            if (isIntroPlaying && allowSkip && Input.GetKeyDown(skipKey))
            {
                SkipIntro();
            }
        }

        private void OnStartClicked()
        {
            if (hasStartRequested) return; // Prevent double clicks
            hasStartRequested = true;
            StartCoroutine(StartGameFlow());
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private IEnumerator StartGameFlow()
        {
            // If we have an intro animation, play it first
            if (introPlayer != null && introPlayer.frames != null && introPlayer.frames.Length > 0)
            {
                isIntroPlaying = true;
                if (introPanel != null) introPanel.SetActive(true);
                if (skipHintText != null && allowSkip) skipHintText.gameObject.SetActive(true);

                if (disableButtonsDuringIntro)
                {
                    SetButtonsInteractable(false);
                }

                introPlayer.onAnimationFinished.AddListener(OnIntroFinished);
                introPlayer.Play();

                // Wait until intro completes (or is skipped)
                while (isIntroPlaying)
                {
                    yield return null;
                }
            }

            // Load gameplay scene
            SceneManager.LoadScene(levelSceneName);
        }

        private void OnIntroFinished()
        {
            isIntroPlaying = false;
        }

        private void SkipIntro()
        {
            if (!isIntroPlaying) return;
            isIntroPlaying = false; // The coroutine loop will continue past waiting
            if (introPlayer != null) introPlayer.Stop();
        }

        private void SetButtonsInteractable(bool value)
        {
            if (startButton != null) startButton.interactable = value;
            if (quitButton != null) quitButton.interactable = value;
        }
    }
}
