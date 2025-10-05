using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using ParasiteLost.Rhythm.UI;
using ParasiteLost.Managers;

namespace ParasiteLost.Rhythm.Mechanics
{
    public class RhythmGameController : MonoBehaviour
    {
        [Header("Game References")]
        public Transform hitLineTransform; // Reference to the hitline object
        public RhythmHealthUI healthUI;
        public AudioSource backgroundMusicSource; // Just for background music
        
        [Header("Audio")]
        public AudioClip hitSuccessSound; // Sound to play when player hits successfully
        public AudioClip hitMissSound; // Sound to play when player misses
        public AudioSource sfxAudioSource; // Audio source for sound effects
        
        [Header("Glitch Spawning")]
        public GameObject glitchPrefab;
        public Transform spawnPoint; // Where glitches spawn (right side of screen)
        public float spawnInterval = 2f; // Time between glitch spawns
        public float glitchMoveSpeed = 5f;
        
        [Header("Input Settings")]
        public float hitTolerance = 0.5f; // Distance tolerance for successful hits
        
        [Header("Battle Settings")]
        public float battleDuration = 40f; // 40 seconds for first battle
        
        private List<GlitchNote> activeGlitches = new List<GlitchNote>();
        private float nextSpawnTime;
        private bool gameActive = false;
        private float battleStartTime;
        private float hitLineX;
        private int totalGlitchesSpawned = 0;
        private int successfulHits = 0;
        
        // Events
        public global::System.Action<float> OnBattleComplete; // Passes accuracy percentage
        
        private void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            // Get hitline position
            if (hitLineTransform != null)
            {
                hitLineX = hitLineTransform.position.x;
            }
            else
            {
                Debug.LogError("Hit line transform not assigned!");
                hitLineX = 0f;
            }
            
            // Set initial spawn time
            nextSpawnTime = Time.time + 1f; // Small delay before first glitch
            
            // Set up health UI callback
            if (healthUI != null)
            {
                healthUI.OnAllHeartsLost += OnGameOver;
            }
            
            // Find background music source if not assigned
            if (backgroundMusicSource == null)
            {
                backgroundMusicSource = FindFirstObjectByType<AudioSource>();
            }
            
            // Find or setup SFX audio source
            if (sfxAudioSource == null)
            {
                // Try to find a second AudioSource, or create one
                AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                if (audioSources.Length > 1)
                {
                    // Use second AudioSource for SFX
                    sfxAudioSource = audioSources[1];
                }
                else if (backgroundMusicSource != null)
                {
                    // Create a new AudioSource for SFX
                    sfxAudioSource = gameObject.AddComponent<AudioSource>();
                    sfxAudioSource.volume = 0.8f;
                    sfxAudioSource.loop = false;
                }
            }
            
            // Start the battle
            StartBattle();
        }
        
        private void Update()
        {
            if (!gameActive) return;
            
            // Handle input
            HandleInput();
            
            // Spawn glitches
            HandleGlitchSpawning();
            
            // Check battle duration
            CheckBattleDuration();
            
            // Clean up destroyed glitches from list
            CleanupActiveGlitches();
        }
        
        private void HandleInput()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                AttemptHit();
            }
        }
        
        private void HandleGlitchSpawning()
        {
            if (Time.time >= nextSpawnTime)
            {
                Debug.Log($"Time to spawn! Current time: {Time.time}, Next spawn time: {nextSpawnTime}");
                SpawnGlitch();
                nextSpawnTime = Time.time + spawnInterval;
                Debug.Log($"Next spawn scheduled for: {nextSpawnTime}");
            }
        }
        
        private void CheckBattleDuration()
        {
            float elapsedTime = Time.time - battleStartTime;
            
            // End battle when song ends (either after 40 seconds OR when background music stops)
            bool timeUp = elapsedTime >= battleDuration;
            bool musicEnded = backgroundMusicSource != null && !backgroundMusicSource.isPlaying && elapsedTime > 2f; // Give 2 second buffer
            
            if (timeUp || musicEnded)
            {
                Debug.Log($"Battle ending - Time up: {timeUp}, Music ended: {musicEnded}, Elapsed: {elapsedTime}");
                EndBattle();
            }
        }
        
        private void CleanupActiveGlitches()
        {
            activeGlitches.RemoveAll(glitch => glitch == null);
        }
        
        private void SpawnGlitch()
        {
            if (glitchPrefab == null)
            {
                Debug.LogError("Glitch prefab is not assigned!");
                return;
            }
            
            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point is not assigned!");
                return;
            }
            
            // Debug.Log($"Spawning glitch at position: {spawnPoint.position}");
            
            GameObject glitchObj = Instantiate(glitchPrefab, spawnPoint.position, Quaternion.identity);
            GlitchNote glitch = glitchObj.GetComponent<GlitchNote>();
            
            if (glitch == null)
            {
                glitch = glitchObj.AddComponent<GlitchNote>();
                Debug.Log("Added GlitchNote component to spawned object");
            }
            
            // Configure the glitch
            glitch.SetHitLinePosition(hitLineX);
            glitch.SetMoveSpeed(glitchMoveSpeed);
            glitch.hitTolerance = hitTolerance;
            
            activeGlitches.Add(glitch);
            totalGlitchesSpawned++;
            
            // Debug.Log($"Glitch spawned successfully! Total: {totalGlitchesSpawned}, Position: {glitchObj.transform.position}");
        }
        
        private void AttemptHit()
        {
            bool hitSuccessful = false;
            GlitchNote closestGlitch = null;
            float closestDistance = float.MaxValue;
            
            // Find the closest glitch that can be hit
            foreach (GlitchNote glitch in activeGlitches)
            {
                if (glitch == null) continue;
                
                if (glitch.CanBeHit())
                {
                    float distance = Mathf.Abs(glitch.transform.position.x - hitLineX);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestGlitch = glitch;
                    }
                }
            }
            
            // Hit the closest glitch if found
            if (closestGlitch != null)
            {
                closestGlitch.OnHit();
                hitSuccessful = true;
            }
            
            // Play appropriate sound
            if (hitSuccessful)
            {
                PlayHitSuccessSound();
            }
            else
            {
                PlayHitMissSound();
            }
        }
        
        public void OnGlitchHit(float accuracy)
        {
            successfulHits++;
            // Debug.Log($"Glitch hit successfully! Accuracy: {accuracy:F2}");
            
            // Optionally provide bonus points for high accuracy
            if (accuracy > 0.9f)
            {
                Debug.Log("Perfect hit!");
            }
        }
        
        public void OnGlitchMissed()
        {
            Debug.Log("Glitch missed - losing health!");
            
            if (healthUI != null)
            {
                healthUI.LoseHeart();
            }
            
            PlayHitMissSound();
        }
        
        private void PlayHitSuccessSound()
        {
            // Play success sound if available
            if (sfxAudioSource != null && hitSuccessSound != null)
            {
                sfxAudioSource.PlayOneShot(hitSuccessSound);
            }
            else if (backgroundMusicSource != null && hitSuccessSound != null)
            {
                // Fallback to background music source if no SFX source
                backgroundMusicSource.PlayOneShot(hitSuccessSound);
            }
            
            // Debug.Log("Hit Success!");
        }
        
        private void PlayHitMissSound()
        {
            // Play miss sound if available
            if (sfxAudioSource != null && hitMissSound != null)
            {
                sfxAudioSource.PlayOneShot(hitMissSound);
            }
            else if (backgroundMusicSource != null && hitMissSound != null)
            {
                // Fallback to background music source if no SFX source
                backgroundMusicSource.PlayOneShot(hitMissSound);
            }
            
            // Debug.Log("Hit Miss!");
        }
        
        private void StartBattle()
        {
            gameActive = true;
            battleStartTime = Time.time;
            
            // Start background music (should be 40 seconds long, no loop)
            if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
            {
                backgroundMusicSource.loop = false; // Don't loop - song should be exactly 40 seconds
                backgroundMusicSource.Play();
            }
            
            Debug.Log("Rhythm battle started!");
        }
        
        private void EndBattle()
        {
            gameActive = false;
            
            // Stop background music
            if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
            {
                backgroundMusicSource.Stop();
            }
            
            // Calculate final accuracy
            float accuracy = totalGlitchesSpawned > 0 ? (float)successfulHits / totalGlitchesSpawned : 0f;
            bool battleWon = healthUI != null ? healthUI.HasHeartsRemaining() : true;
            
            Debug.Log($"Battle ended! Accuracy: {accuracy:F2} ({successfulHits}/{totalGlitchesSpawned})");
            Debug.Log($"Battle result: {(battleWon ? "WON" : "LOST")}");
            
            // Notify battle complete
            OnBattleComplete?.Invoke(accuracy);
            
            // Return to previous scene after a short delay
            Invoke(nameof(ReturnToPreviousScene), 2f);
        }
        
        private void OnGameOver()
        {
            Debug.Log("Game Over - All hearts lost!");
            EndBattle();
        }
        
        private void ReturnToPreviousScene()
        {
            float accuracy = totalGlitchesSpawned > 0 ? (float)successfulHits / totalGlitchesSpawned : 0f;
            bool battleWon = healthUI != null ? healthUI.HasHeartsRemaining() : true;
            
            Debug.Log($"[RhythmGameController] Returning to previous scene. Battle won: {battleWon}, Hearts remaining: {(healthUI != null ? healthUI.GetCurrentHearts() : 0)}");
            
            // Use the existing battle result handler
            var battleResultHandler = RhythmBattleResultHandler.Instance;
            if (battleResultHandler != null)
            {
                Debug.Log($"[RhythmGameController] Using RhythmBattleResultHandler with accuracy: {accuracy:F2}");
                battleResultHandler.OnRhythmGameComplete(accuracy);
            }
            else
            {
                Debug.LogError("[RhythmGameController] RhythmBattleResultHandler not found! Using fallback.");
                // Fallback to direct scene management
                var gameManager = GameManager.Instance;
                if (gameManager != null)
                {
                    Debug.Log($"[RhythmGameController] Using GameManager fallback with battleWon: {battleWon}");
                    gameManager.EndRhythmBattle(battleWon);
                }
                else
                {
                    Debug.LogError("[RhythmGameController] GameManager also not found! Cannot return to scene.");
                }
            }
        }
        
        public void SetSpawnInterval(float interval)
        {
            spawnInterval = interval;
        }
        
        public void SetGlitchSpeed(float speed)
        {
            glitchMoveSpeed = speed;
        }
        
        public bool IsGameActive()
        {
            return gameActive;
        }
        
        public float GetBattleProgress()
        {
            if (!gameActive) return 1f;
            return (Time.time - battleStartTime) / battleDuration;
        }
    }
}