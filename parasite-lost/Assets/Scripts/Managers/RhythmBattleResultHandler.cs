using UnityEngine;
using UnityEngine.SceneManagement;
using ParasiteLost.Managers;
using ParasiteLost.Core.Hosts;

namespace ParasiteLost.Managers
{
    public class RhythmBattleResultHandler : MonoBehaviour
    {
        [Header("Battle Settings")]
        public float battleTimeLimit = 30f;
        public float scoreThreshold = 0.7f; // 70% accuracy to win
        
        [Header("Result Events")]
        public UnityEngine.Events.UnityEvent OnBattleWon;
        public UnityEngine.Events.UnityEvent OnBattleLost;
        
        private bool battleInProgress = false;
        private float battleStartTime;
        private float currentScore = 0f;
        private GameManager gameManager;
        private GameStateManager gameStateManager;
        
        private static RhythmBattleResultHandler instance;
        public static RhythmBattleResultHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<RhythmBattleResultHandler>();
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
            gameManager = GameManager.Instance;
            gameStateManager = GameStateManager.Instance;
        }
        
        public void StartBattle()
        {
            battleInProgress = true;
            battleStartTime = Time.time;
            currentScore = 0f;
            
            Debug.Log("Rhythm battle started!");
        }
        
        public void EndBattle(bool won)
        {
            if (!battleInProgress) return;
            
            battleInProgress = false;
            
            Debug.Log($"Rhythm battle ended. Won: {won}");
            
            if (won)
            {
                OnBattleWon?.Invoke();
                HandleBattleWin();
            }
            else
            {
                OnBattleLost?.Invoke();
                HandleBattleLoss();
            }
            
            // Return to the main level
            if (gameManager != null)
            {
                gameManager.EndRhythmBattle(won);
            }
        }
        
        private void HandleBattleWin()
        {
            // Mark the fish as successfully interacted with
            if (gameStateManager?.currentInteractingFish != null)
            {
                var fish = gameStateManager.currentInteractingFish;
                var fishId = fish.GetInstanceID().ToString();
                gameStateManager.MarkFishAsInteracted(fishId);
                
                // Mark fish as no longer interactable
                fish.OnPossessed();
                
                Debug.Log($"Successfully possessed {fish.fishSize} fish!");
            }
        }
        
        private void HandleBattleLoss()
        {
            Debug.Log("Battle lost - no lifespan bonus awarded");
        }
        
        public void UpdateScore(float accuracy)
        {
            currentScore = accuracy;
        }
        
        public void CheckBattleConditions()
        {
            if (!battleInProgress) return;
            
            // Check if time limit exceeded
            if (Time.time - battleStartTime >= battleTimeLimit)
            {
                EndBattle(currentScore >= scoreThreshold);
            }
        }
        
        public void ForceEndBattle(bool won)
        {
            EndBattle(won);
        }
        
        public bool IsBattleInProgress()
        {
            return battleInProgress;
        }
        
        public float GetBattleProgress()
        {
            if (!battleInProgress) return 0f;
            return (Time.time - battleStartTime) / battleTimeLimit;
        }
        
        public float GetCurrentScore()
        {
            return currentScore;
        }
        
        // Call this from your rhythm game when the player hits notes correctly
        public void OnNoteHit(float accuracy)
        {
            UpdateScore(accuracy);
            
            // Check if we've reached the win condition
            if (accuracy >= scoreThreshold)
            {
                EndBattle(true);
            }
        }
        
        // Call this from your rhythm game when the player misses too many notes
        public void OnNoteMiss()
        {
            // You can implement penalty logic here
            // For now, we'll just check conditions
            CheckBattleConditions();
        }
        
        // Call this when the rhythm game ends naturally
        public void OnRhythmGameComplete(float finalAccuracy)
        {
            UpdateScore(finalAccuracy);
            EndBattle(finalAccuracy >= scoreThreshold);
        }
    }
}
