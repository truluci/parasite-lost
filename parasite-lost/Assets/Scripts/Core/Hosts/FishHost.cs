using UnityEngine;
using ParasiteLost.Managers;

namespace ParasiteLost.Core.Hosts
{
    public class FishHost : MonoBehaviour
    {
        [Header("Fish Properties")]
        public FishSize fishSize;
        public float possessionDifficulty = 1f;
        public float lifespanBonus = 5f;
        
        [Header("Visual")]
        public SpriteRenderer spriteRenderer;
        
        [Header("Interaction")]
        public CircleCollider2D interactionCollider;
        public bool isInteractable = true;
        
        public enum FishSize
        {
            Small,
            Medium,
            Large
        }
        
        private void Start()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
                
            if (interactionCollider == null)
            {
                interactionCollider = gameObject.AddComponent<CircleCollider2D>();
                interactionCollider.isTrigger = true;
                interactionCollider.radius = 1f; // Adjust based on fish size
            }
            
            // Set collider size based on fish size
            SetColliderSize();
        }
        
        private void SetColliderSize()
        {
            if (interactionCollider != null)
            {
                switch (fishSize)
                {
                    case FishSize.Small:
                        interactionCollider.radius = 0.8f;
                        break;
                    case FishSize.Medium:
                        interactionCollider.radius = 1.2f;
                        break;
                    case FishSize.Large:
                        interactionCollider.radius = 1.6f;
                        break;
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isInteractable) return;
            
            // Check if the parasite (player) is interacting with this fish
            if (other.CompareTag("Player"))
            {
                Debug.Log($"Parasite entered {fishSize} fish interaction zone!");
                StartRhythmBattle();
            }
        }
        
        private void StartRhythmBattle()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartFishInteraction(this);
            }
        }
        
        public bool CanBePossessed()
        {
            // Check if fish can be possessed (not already possessed, etc.)
            return isInteractable;
        }
        
        public void OnPossessed()
        {
            Debug.Log($"Parasite possessed {fishSize} fish!");
            isInteractable = false;
            // Handle possession logic
        }
        
        public float GetLifespanBonus()
        {
            switch (fishSize)
            {
                case FishSize.Small:
                    return 3f; // 3 seconds for small fish
                case FishSize.Medium:
                    return 5f; // 5 seconds for medium fish
                case FishSize.Large:
                    return 8f; // 8 seconds for large fish
                default:
                    return lifespanBonus;
            }
        }
        
        public float GetPossessionDifficulty()
        {
            switch (fishSize)
            {
                case FishSize.Small:
                    return 0.8f; // Easier for small fish
                case FishSize.Medium:
                    return 1.0f; // Normal difficulty
                case FishSize.Large:
                    return 1.3f; // Harder for large fish
                default:
                    return possessionDifficulty;
            }
        }
        
        public string GetBattleSceneName()
        {
            switch (fishSize)
            {
                case FishSize.Small:
                    return "easy battle";
                case FishSize.Medium:
                    return "medium battle";
                case FishSize.Large:
                    return "hard battle";
                default:
                    return "easy battle";
            }
        }
    }
}
