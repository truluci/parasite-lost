using UnityEngine;
using UnityEngine.SceneManagement;
using ParasiteLost.Core.Player;
using ParasiteLost.Core.Hosts;
using ParasiteLost.Rhythm.System;
// GameEvents now lives in ParasiteLost.Managers (same namespace), no extra using required

namespace ParasiteLost.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        public GameState currentState;
        
        [Header("References")]
        public ParasiteController parasiteController;
        public RhythmManager rhythmManager;
        public GameStateManager gameStateManager;
        public UI.UIManager uiManager;
        
        [Header("Lifespan Settings")]
        public float baseLifespan = 5f;
        public bool isLifespanActive = true;
        
        public enum GameState
        {
            MainMenu,
            Playing,
            RhythmBattle,
            Paused,
            GameOver
        }
        private bool pendingRestart = false;
        
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameManager>();
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
            InitializeGame();
            
            // Auto-start game if we're in a level scene (not main menu)
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (!currentSceneName.ToLower().Contains("menu"))
            {
                StartGame();
            }
        }
        
        private void InitializeGame()
        {
            currentState = GameState.MainMenu;
            
            // Initialize game state manager if not already done
            if (gameStateManager == null)
            {
                gameStateManager = FindFirstObjectByType<GameStateManager>();
                if (gameStateManager == null)
                {
                    GameObject stateManagerObj = new GameObject("GameStateManager");
                    gameStateManager = stateManagerObj.AddComponent<GameStateManager>();
                }
            }
            
            // Initialize UI manager if not already done
            if (uiManager == null)
            {
                uiManager = FindFirstObjectByType<UI.UIManager>();
                if (uiManager == null)
                {
                    GameObject uiManagerObj = new GameObject("UIManager");
                    uiManager = uiManagerObj.AddComponent<UI.UIManager>();
                }
            }
            
            Debug.Log("Game initialized");
        }
        
        public void StartGame()
        {
            // Avoid duplicate start
            if (currentState == GameState.Playing) return;

            currentState = GameState.Playing;

            // Reacquire parasite if lost (e.g., after scene reload)
            if (parasiteController == null)
            {
                parasiteController = FindFirstObjectByType<ParasiteController>();
            }

            // Initialize parasite lifespan (use its own base value to avoid divergence)
            if (parasiteController != null)
            {
                parasiteController.ResetLifespan();
            }
            else
            {
                Debug.LogWarning("GameManager.StartGame: ParasiteController not found.");
            }

            Debug.Log("Game started");
        }
        
        public void StartFishInteraction(FishHost fish)
        {
            if (gameStateManager != null)
            {
                gameStateManager.StartRhythmBattle(fish);
                currentState = GameState.RhythmBattle;
            }
        }
        
        public void StartRhythmBattle()
        {
            currentState = GameState.RhythmBattle;
            rhythmManager?.StartRhythmBattle();
            Debug.Log("Rhythm battle started");
        }
        
        public void EndRhythmBattle(bool battleWon = false)
        {
            currentState = GameState.Playing;
            rhythmManager?.StopRhythmBattle();
            
            // Handle battle result
            if (battleWon && gameStateManager?.currentInteractingFish != null)
            {
                var fish = gameStateManager.currentInteractingFish;
                var lifespanBonus = fish.GetLifespanBonus();
                
                if (parasiteController != null)
                {
                    parasiteController.ExtendLifespan(lifespanBonus);
                }
                
                // Show UI feedback for lifespan bonus
                if (uiManager != null)
                {
                    uiManager.ShowLifespanBonus(lifespanBonus);
                }
                
                Debug.Log($"Battle won! Lifespan extended by {lifespanBonus} seconds");
            }
            
            // End the battle in game state manager
            if (gameStateManager != null)
            {
                gameStateManager.EndRhythmBattle(battleWon);
            }
            
            Debug.Log("Rhythm battle ended");
        }
        
        public void PauseGame()
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;
            Debug.Log("Game paused");
        }
        
        public void ResumeGame()
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;
            Debug.Log("Game resumed");
        }
        
        public void GameOver()
        {
            if (currentState == GameState.GameOver) return; // prevent multiple triggers
            currentState = GameState.GameOver;
            Debug.LogError("Game Over! Parasite lifespan expired!");
        }
        
        public void RestartLevel()
        {
            // Ensure time scale is normal
            Time.timeScale = 1f;
            // Mark restart (event system deferred / removed for now)
            pendingRestart = true;

            // Reset saved state (fresh start after reload)
            if (gameStateManager != null)
            {
                gameStateManager.ResetLevelState();
            }

            // Move to MainMenu interim state until scene fully loads
            currentState = GameState.MainMenu;

            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Reacquire references after scene load
            if (parasiteController == null)
            {
                parasiteController = FindFirstObjectByType<ParasiteController>();
            }
            if (uiManager == null)
            {
                uiManager = FindFirstObjectByType<UI.UIManager>();
            }
            if (gameStateManager == null)
            {
                gameStateManager = FindFirstObjectByType<GameStateManager>();
            }

            // If returning from rhythm battle, restore state; otherwise start fresh game
            if (pendingRestart)
            {
                pendingRestart = false;
                StartGame();
                return;
            }

            if (gameStateManager != null && gameStateManager.isInRhythmBattle && currentState == GameState.Playing)
            {
                gameStateManager.RestoreLevelState();
            }
            else if (parasiteController != null)
            {
                StartGame();
            }
        }
    }
}
