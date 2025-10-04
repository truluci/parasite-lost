using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using ParasiteLost.Core.Hosts;

namespace ParasiteLost.Managers
{
    public class GameStateManager : MonoBehaviour
    {
        [System.Serializable]
        public class LevelState
        {
            public string levelName;
            public Vector3 parasitePosition;
            public float currentLifespan;
            public List<string> interactedFishIds = new List<string>();
            public float timeSpentInLevel;
        }
        
        [Header("State Management")]
        public LevelState currentLevelState;
        public bool isInRhythmBattle = false;
        public string currentBattleScene;
        public FishHost currentInteractingFish;
        
        private static GameStateManager instance;
        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameStateManager>();
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
                InitializeState();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeState()
        {
            currentLevelState = new LevelState();
        }
        
        public void SaveLevelState(Vector3 parasitePosition, float currentLifespan, string levelName)
        {
            currentLevelState.levelName = levelName;
            currentLevelState.parasitePosition = parasitePosition;
            currentLevelState.currentLifespan = currentLifespan;
            currentLevelState.timeSpentInLevel += Time.deltaTime;
        }
        
        public void StartRhythmBattle(FishHost fish)
        {
            if (isInRhythmBattle) return;
            
            currentInteractingFish = fish;
            currentBattleScene = fish.GetBattleSceneName();
            isInRhythmBattle = true;
            
            // Save current state before switching scenes
            var parasite = FindFirstObjectByType<ParasiteLost.Core.Player.ParasiteController>();
            if (parasite != null)
            {
                SaveLevelState(parasite.transform.position, parasite.currentLifespan, SceneManager.GetActiveScene().name);
            }
            
            Debug.Log($"Starting rhythm battle: {currentBattleScene} for {fish.fishSize} fish");
            SceneManager.LoadScene(currentBattleScene);
        }
        
        public void EndRhythmBattle(bool battleWon)
        {
            if (!isInRhythmBattle) return;
            
            Debug.Log($"Rhythm battle ended. Won: {battleWon}");
            
            // Return to the previous level
            if (!string.IsNullOrEmpty(currentLevelState.levelName))
            {
                SceneManager.LoadScene(currentLevelState.levelName);
            }
            
            // Reset battle state
            isInRhythmBattle = false;
            currentBattleScene = null;
            currentInteractingFish = null;
        }
        
        public void RestoreLevelState()
        {
            if (currentLevelState == null) return;
            
            // Restore parasite position and lifespan
            var parasite = FindFirstObjectByType<ParasiteLost.Core.Player.ParasiteController>();
            if (parasite != null)
            {
                parasite.transform.position = currentLevelState.parasitePosition;
                parasite.currentLifespan = currentLevelState.currentLifespan;
                
                Debug.Log($"Level state restored - Position: {currentLevelState.parasitePosition}, Lifespan: {currentLevelState.currentLifespan}");
            }
        }
        
        public void MarkFishAsInteracted(string fishId)
        {
            if (currentLevelState != null && !currentLevelState.interactedFishIds.Contains(fishId))
            {
                currentLevelState.interactedFishIds.Add(fishId);
            }
        }
        
        public bool HasFishBeenInteracted(string fishId)
        {
            return currentLevelState?.interactedFishIds.Contains(fishId) ?? false;
        }
        
        public void ResetLevelState()
        {
            currentLevelState = new LevelState();
            isInRhythmBattle = false;
            currentBattleScene = null;
            currentInteractingFish = null;
        }
    }
}
