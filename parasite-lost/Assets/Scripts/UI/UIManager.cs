using UnityEngine;
using ParasiteLost.Managers;
using ParasiteLost.Core.Player;

namespace ParasiteLost.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Components")]
        public LifespanUI lifespanUI;
        public DeathScreenUI deathScreenUI;
        
        [Header("Game State UI")]
        public GameObject gameplayUI;
        public GameObject pauseUI;
        public GameObject mainMenuUI;
        
        private GameManager gameManager;
        private ParasiteController parasiteController;
        
        private static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<UIManager>();
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            gameManager = GameManager.Instance;
            parasiteController = FindFirstObjectByType<ParasiteController>();
            
            // Find UI components if not assigned
            if (lifespanUI == null)
            {
                lifespanUI = FindFirstObjectByType<LifespanUI>();
            }
            
            // Attempt to find DeathScreenUI even if its GameObject is inactive in hierarchy
            EnsureDeathScreenUI();
            
            // Set initial UI state
            UpdateUIForGameState(GameManager.GameState.MainMenu);
        }
        
        private GameManager.GameState lastGameState;
        
        private void Update()
        {
            if (gameManager != null && gameManager.currentState != lastGameState)
            {
                lastGameState = gameManager.currentState;
                UpdateUIForGameState(gameManager.currentState);
            }
        }
        
        private void UpdateUIForGameState(GameManager.GameState gameState)
        {
            switch (gameState)
            {
                case GameManager.GameState.MainMenu:
                    ShowMainMenuUI();
                    break;
                    
                case GameManager.GameState.Playing:
                    ShowGameplayUI();
                    break;
                    
                case GameManager.GameState.RhythmBattle:
                    ShowRhythmBattleUI();
                    break;
                    
                case GameManager.GameState.Paused:
                    ShowPauseUI();
                    break;
                    
                case GameManager.GameState.GameOver:
                    ShowGameOverUI();
                    break;
            }
        }
        
        private void ShowMainMenuUI()
        {
            SetUIActive(gameplayUI, false);
            SetUIActive(pauseUI, false);
            SetUIActive(mainMenuUI, true);
            
            if (lifespanUI != null)
            {
                lifespanUI.gameObject.SetActive(false);
            }
            
            if (deathScreenUI != null)
            {
                deathScreenUI.HideDeathScreen();
            }
        }
        
        private void ShowGameplayUI()
        {
            SetUIActive(gameplayUI, true);
            SetUIActive(pauseUI, false);
            SetUIActive(mainMenuUI, false);
            
            if (lifespanUI != null)
            {
                lifespanUI.gameObject.SetActive(true);
            }
            
            if (deathScreenUI != null)
            {
                deathScreenUI.HideDeathScreen();
            }
        }
        
        private void ShowRhythmBattleUI()
        {
            // Hide gameplay UI during rhythm battles
            SetUIActive(gameplayUI, false);
            SetUIActive(pauseUI, false);
            SetUIActive(mainMenuUI, false);
            
            if (lifespanUI != null)
            {
                lifespanUI.gameObject.SetActive(false);
            }
            
            if (deathScreenUI != null)
            {
                deathScreenUI.HideDeathScreen();
            }
        }
        
        private void ShowPauseUI()
        {
            SetUIActive(gameplayUI, false);
            SetUIActive(pauseUI, true);
            SetUIActive(mainMenuUI, false);
            
            if (lifespanUI != null)
            {
                lifespanUI.gameObject.SetActive(false);
            }
            
            if (deathScreenUI != null)
            {
                deathScreenUI.HideDeathScreen();
            }
        }
        
        private void ShowGameOverUI()
        {
            // Don't disable the main canvas - just hide specific gameplay elements
            // SetUIActive(gameplayUI, false); // This was disabling the main canvas!
            SetUIActive(pauseUI, false);
            SetUIActive(mainMenuUI, false);
            
            if (lifespanUI != null)
            {
                lifespanUI.gameObject.SetActive(false);
            }
            
            // Re-acquire death screen (may have been inactive at startup)
            EnsureDeathScreenUI();
            if (deathScreenUI != null)
            {
                deathScreenUI.ShowDeathScreen();
            }
            else
            {
                Debug.LogWarning("UIManager: GameOver state reached but DeathScreenUI not found. Ensure a DeathScreenUI exists in the scene (root object can stay active while panel child is disabled)." );
            }
        }
        
        private void SetUIActive(GameObject uiObject, bool active)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(active);
            }
        }
        
        public void ShowLifespanBonus(float bonusAmount)
        {
            if (lifespanUI != null)
            {
                lifespanUI.ShowLifespanBonus(bonusAmount);
            }
        }
        
        public void ResetLifespanUI()
        {
            if (lifespanUI != null)
            {
                lifespanUI.ResetUI();
            }
        }
        
        public void SetCustomDeathMessage(string title, string message)
        {
            if (deathScreenUI != null)
            {
                deathScreenUI.SetDeathMessage(title, message);
            }
        }

        // --- Robust lookup helpers -------------------------------------------------
        private void EnsureDeathScreenUI()
        {
            if (deathScreenUI != null) return;

            // First try normal active object lookup
            deathScreenUI = FindFirstObjectByType<DeathScreenUI>();
            if (deathScreenUI != null) return;

            // Fallback: include inactive objects via Resources API
            var allDeathScreens = Resources.FindObjectsOfTypeAll<DeathScreenUI>();
            foreach (var ds in allDeathScreens)
            {
                // Skip assets / prefabs not in a valid scene
                if (!ds.gameObject.scene.IsValid()) continue;
                deathScreenUI = ds;
                break;
            }
        }

        // Can be called directly if parasite death needs to force UI without relying on polling
        public void ForceShowDeathScreen()
        {
            EnsureDeathScreenUI();
            if (deathScreenUI != null)
            {
                deathScreenUI.ShowDeathScreen();
            }
        }
        
        // Public methods for external UI control
        public void OnGameStart()
        {
            if (gameManager != null)
            {
                gameManager.StartGame();
            }
        }
        
        public void OnGamePause()
        {
            if (gameManager != null)
            {
                gameManager.PauseGame();
            }
        }
        
        public void OnGameResume()
        {
            if (gameManager != null)
            {
                gameManager.ResumeGame();
            }
        }
        
        public void OnRestartLevel()
        {
            if (gameManager != null)
            {
                gameManager.RestartLevel();
            }
        }
        
        public void OnQuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
