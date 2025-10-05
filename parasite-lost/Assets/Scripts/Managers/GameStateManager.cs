using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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
        public FishHost.FishSize currentInteractingFishSize;
        public string currentInteractingFishId;
        
        [Header("Battle Rewards")]
        public float smallFishBonus = 5f; // Bonus lifespan for small fish
        public float mediumFishBonus = 10f; // Bonus lifespan for medium fish  
        public float largeFishBonus = 15f; // Bonus lifespan for large fish
        
        [Header("Fish Interaction Cooldown")]
        private bool fishInteractionsCooldown = false;
        private float cooldownEndTime = 0f;
        private const float INTERACTION_COOLDOWN_DURATION = 3f;
        
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
                
                // Subscribe to scene loaded event to restore state
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void InitializeState()
        {
            currentLevelState = new LevelState();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[GameStateManager] Scene loaded: {scene.name}, isInRhythmBattle: {isInRhythmBattle}, savedLevelName: {currentLevelState?.levelName}");
            
            // If we're returning from a battle, restore the level state
            if (!isInRhythmBattle && currentLevelState != null && scene.name == currentLevelState.levelName)
            {
                Debug.Log($"[GameStateManager] Conditions met for state restoration. Starting coroutine...");
                // Wait a frame for objects to be initialized
                StartCoroutine(RestoreStateAfterFrame());
            }
            else
            {
                Debug.Log($"[GameStateManager] State restoration skipped. Reason: isInRhythmBattle={isInRhythmBattle}, currentLevelState={currentLevelState != null}, sceneMatch={currentLevelState?.levelName == scene.name}");
            }
        }
        
        private System.Collections.IEnumerator RestoreStateAfterFrame()
        {
            Debug.Log("[GameStateManager] 🔄 RestoreStateAfterFrame coroutine started");
            yield return null; // Wait one frame
            
            // Wait a bit more to ensure scene objects are fully loaded
            yield return new WaitForSeconds(0.1f);
            
            Debug.Log("[GameStateManager] 🔄 About to call RestoreLevelState()");
            RestoreLevelState();
        }
        
        public void SaveLevelState(Vector3 parasitePosition, float currentLifespan, string levelName)
        {
            currentLevelState.levelName = levelName;
            currentLevelState.parasitePosition = parasitePosition;
            currentLevelState.currentLifespan = currentLifespan;
            currentLevelState.timeSpentInLevel += Time.deltaTime;
            
            // Debug.Log($"[GameStateManager] Level state saved - Scene: {levelName}, Position: {parasitePosition}, Lifespan: {currentLifespan}");
        }
        
        public void StartRhythmBattle(FishHost fish)
        {
            if (isInRhythmBattle) return;
            
            currentInteractingFish = fish;
            currentInteractingFishSize = fish.fishSize;
            currentInteractingFishId = fish.GetInstanceID().ToString();
            currentBattleScene = fish.GetBattleSceneName();
            isInRhythmBattle = true;
            
            Debug.Log($"[GameStateManager] 🐟 Fish data stored: {fish.fishSize} fish (ID: {fish.GetInstanceID()})");
            Debug.Log($"[GameStateManager] 🐟 Fish size stored: {currentInteractingFishSize}");
            
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
            if (!isInRhythmBattle)
            {
                Debug.LogWarning($"[GameStateManager] EndRhythmBattle called but not in rhythm battle! battleWon: {battleWon}");
                return;
            }
            
            Debug.Log($"[GameStateManager] 🎵 Rhythm battle ended. Won: {battleWon}");
            Debug.Log($"[GameStateManager] 🐟 Fish reference check: {(currentInteractingFish != null ? currentInteractingFish.fishSize.ToString() : "NULL")}");
            Debug.Log($"[GameStateManager] 🐟 Fish size stored: {currentInteractingFishSize}");
            
            // Store battle result before scene change
            string returnScene = currentLevelState.levelName;
            
            // Calculate lifespan bonus if battle was won (use stored fish data)
            if (battleWon)
            {
                if (!string.IsNullOrEmpty(currentInteractingFishId))
                {
                    float lifespanBonus = GetLifespanBonus(currentInteractingFishSize);
                    float originalLifespan = currentLevelState.currentLifespan;
                    currentLevelState.currentLifespan += lifespanBonus;
                    
                    // Mark fish as interacted using stored ID
                    MarkFishAsInteracted(currentInteractingFishId);
                    
                    Debug.Log($"[GameStateManager] ✅ Battle won! Fish size: {currentInteractingFishSize}");
                    Debug.Log($"[GameStateManager] ✅ Lifespan: {originalLifespan} + {lifespanBonus} = {currentLevelState.currentLifespan} seconds");
                }
                else
                {
                    Debug.LogWarning($"[GameStateManager] ⚠️ Battle won but fish data is missing! No lifespan bonus awarded.");
                }
            }
            else
            {
                Debug.Log($"[GameStateManager] ❌ Battle lost - no lifespan bonus awarded. Current lifespan: {currentLevelState.currentLifespan}");
            }
            
            // Reset battle state AFTER lifespan calculation
            isInRhythmBattle = false;
            currentBattleScene = null;
            currentInteractingFish = null;
            currentInteractingFishSize = FishHost.FishSize.Small; // Reset to default
            currentInteractingFishId = null;
            
            Debug.Log($"[GameStateManager] 🔄 Returning to scene: {returnScene}");
            
            // Return to the previous level
            if (!string.IsNullOrEmpty(returnScene))
            {
                SceneManager.LoadScene(returnScene);
            }
            else
            {
                Debug.LogError("[GameStateManager] ❌ No return scene saved! Cannot return to level.");
            }
        }
        
        public void RestoreLevelState()
        {
            if (currentLevelState == null)
            {
                Debug.LogWarning("[GameStateManager] Cannot restore state - currentLevelState is null");
                return;
            }
            
            Debug.Log($"[GameStateManager] Attempting to restore state - Position: {currentLevelState.parasitePosition}, Lifespan: {currentLevelState.currentLifespan}");
            
            // Try multiple ways to find the parasite
            var parasite = FindFirstObjectByType<ParasiteLost.Core.Player.ParasiteController>();
            
            if (parasite == null)
            {
                Debug.LogWarning("[GameStateManager] ParasiteController not found with FindFirstObjectByType, trying GameObject.Find...");
                
                // Try finding by common names
                string[] possibleNames = { "Parasite", "Player", "ParasiteController", "parasite" };
                foreach (string name in possibleNames)
                {
                    GameObject obj = GameObject.Find(name);
                    if (obj != null)
                    {
                        parasite = obj.GetComponent<ParasiteLost.Core.Player.ParasiteController>();
                        if (parasite != null)
                        {
                            Debug.Log($"[GameStateManager] Found ParasiteController on GameObject named '{name}'");
                            break;
                        }
                    }
                }
            }
            
            if (parasite != null)
            {
                Vector3 oldPosition = parasite.transform.position;
                float oldLifespan = parasite.currentLifespan;
                
                parasite.transform.position = currentLevelState.parasitePosition;
                parasite.currentLifespan = currentLevelState.currentLifespan;
                
                Debug.Log($"[GameStateManager] ✅ Level state restored successfully!");
                Debug.Log($"[GameStateManager]   Position: {oldPosition} → {currentLevelState.parasitePosition}");
                Debug.Log($"[GameStateManager]   Lifespan: {oldLifespan} → {currentLevelState.currentLifespan}");
                
                // Start fish interaction cooldown to prevent immediate re-triggering
                fishInteractionsCooldown = true;
                cooldownEndTime = Time.time + INTERACTION_COOLDOWN_DURATION;
                Debug.Log($"[GameStateManager] Fish interactions disabled for {INTERACTION_COOLDOWN_DURATION} seconds");
                
                // Restore fish interaction states
                RestoreFishStates();
            }
            else
            {
                Debug.LogError("[GameStateManager] ❌ ParasiteController not found in scene! Cannot restore state.");
                Debug.LogError("[GameStateManager] Available GameObjects in scene:");
                GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                for (int i = 0; i < Mathf.Min(allObjects.Length, 10); i++)
                {
                    Debug.LogError($"[GameStateManager]   - {allObjects[i].name}");
                }
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
        
        public bool AreFishInteractionsOnCooldown()
        {
            if (fishInteractionsCooldown && Time.time < cooldownEndTime)
            {
                return true;
            }
            
            // Clear cooldown if time has passed
            if (fishInteractionsCooldown && Time.time >= cooldownEndTime)
            {
                fishInteractionsCooldown = false;
                Debug.Log("[GameStateManager] Fish interaction cooldown ended - interactions re-enabled");
            }
            
            return false;
        }
        
        private void RestoreFishStates()
        {
            if (currentLevelState == null || currentLevelState.interactedFishIds.Count == 0)
            {
                return;
            }
            
            // Find all fish hosts in the scene and disable the ones that have been interacted with
            var fishHosts = FindObjectsByType<ParasiteLost.Core.Hosts.FishHost>(FindObjectsSortMode.None);
            
            foreach (var fish in fishHosts)
            {
                string fishId = fish.GetInstanceID().ToString();
                if (currentLevelState.interactedFishIds.Contains(fishId))
                {
                    // Disable the fish's interactability instead of destroying it
                    fish.isInteractable = false;
                    
                    // Optionally, add visual indication that fish has been interacted with
                    // You could change color, add effect, etc.
                    Debug.Log($"[GameStateManager] Restored fish state - {fish.fishSize} fish marked as interacted");
                }
            }
        }
        
        public float GetLifespanBonus(FishHost.FishSize fishSize)
        {
            switch (fishSize)
            {
                case FishHost.FishSize.Small:
                    return smallFishBonus;
                case FishHost.FishSize.Medium:
                    return mediumFishBonus;
                case FishHost.FishSize.Large:
                    return largeFishBonus;
                default:
                    return smallFishBonus;
            }
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
