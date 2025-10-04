using UnityEngine;
using ParasiteLost.Managers;

namespace ParasiteLost.Core.Player
{
    public class ParasiteController : MonoBehaviour
    {
        [Header("Parasite Settings")]
        public float baseLifespan = 5f;
        public float currentLifespan;
        public bool isLifespanActive = true;
    private bool hasDied = false;
        
        [Header("Movement")]
        public float moveSpeed = 5f;
        
        [Header("Tags")]
        public string parasiteTag = "Player";
        
        private GameManager gameManager;
        private GameStateManager gameStateManager;
        
        private void Start()
        {
            // Set the tag for collision detection
            if (!gameObject.CompareTag(parasiteTag))
            {
                gameObject.tag = parasiteTag;
            }
            
            // Always initialize lifespan to baseLifespan on start
            currentLifespan = baseLifespan;
            hasDied = false;
            
            // Get managers
            gameManager = GameManager.Instance;
            gameStateManager = GameStateManager.Instance;
        }
        
        private void OnEnable()
        {
            // Ensure lifespan is reset when object is enabled (after scene reload)
            currentLifespan = baseLifespan;
            hasDied = false;
        }
        
        private void Update()
        {
            // Try to find GameManager if not found
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }
            
            // Update lifespan if active and in playing state (or if no game manager found)
            if (isLifespanActive && (gameManager?.currentState == GameManager.GameState.Playing || gameManager == null))
            {
                UpdateLifespan();
            }
            
            // Movement is handled by ParasiteMovement script
            // HandleMovement(); // Disabled to avoid conflict
            
            SaveState();
        }
        
        private void UpdateLifespan()
        {
            currentLifespan -= Time.deltaTime;
            
            if (currentLifespan <= 0)
            {
                currentLifespan = 0;
                if (!hasDied)
                {
                    hasDied = true;
                    Debug.LogError("Parasite lifespan expired!");
                
                // Refresh GameManager reference if lost
                if (gameManager == null)
                {
                    gameManager = GameManager.Instance;
                }

                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
                else
                {
                    Debug.LogWarning("ParasiteController: GameManager missing when lifespan expired. Attempting direct death screen display.");
                    var uiManager = ParasiteLost.UI.UIManager.Instance;
                    uiManager?.ForceShowDeathScreen();
                }
                }
            }
        }
        
        private void HandleMovement()
        {
            // Only allow movement when playing
            if (gameManager?.currentState != GameManager.GameState.Playing)
                return;
                
            // Basic movement input handling - using new Input System
            Vector2 inputVector = Vector2.zero;
            
            // Use WASD or Arrow keys
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed || 
                    UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed)
                    inputVector.y += 1f;
                if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed || 
                    UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed)
                    inputVector.y -= 1f;
                if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed || 
                    UnityEngine.InputSystem.Keyboard.current.leftArrowKey.isPressed)
                    inputVector.x -= 1f;
                if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed || 
                    UnityEngine.InputSystem.Keyboard.current.rightArrowKey.isPressed)
                    inputVector.x += 1f;
            }
            
            Vector3 movement = new Vector3(inputVector.x, inputVector.y, 0) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
        
        private void SaveState()
        {
            // Try to find managers if not found
            if (gameStateManager == null)
            {
                gameStateManager = GameStateManager.Instance;
            }
            
            // Save state periodically for persistence
            if (gameStateManager != null && (gameManager?.currentState == GameManager.GameState.Playing || gameManager == null))
            {
                gameStateManager.SaveLevelState(transform.position, currentLifespan, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
        
        public void ExtendLifespan(float amount)
        {
            currentLifespan += amount;
            Debug.Log($"Lifespan extended by {amount}. Current lifespan: {currentLifespan}");
        }
        
        public void SetLifespan(float newLifespan)
        {
            currentLifespan = newLifespan;
            Debug.Log($"Lifespan set to {currentLifespan}");
        }
        
        public void ResetLifespan()
        {
            currentLifespan = baseLifespan;
            Debug.Log($"Lifespan reset to {currentLifespan}");
            hasDied = false;
        }
        
        public float GetLifespanPercentage()
        {
            return currentLifespan / baseLifespan;
        }
        
        public bool IsAlive()
        {
            return currentLifespan > 0;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // This will be handled by the FishHost's OnTriggerEnter2D
            // But we can add additional parasite-specific logic here if needed
        }
    }
}
