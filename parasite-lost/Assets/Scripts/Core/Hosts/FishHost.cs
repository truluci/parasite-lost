using UnityEngine;

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
        }
        
        public bool CanBePossessed()
        {
            // Check if fish can be possessed (not already possessed, etc.)
            return true;
        }
        
        public void OnPossessed()
        {
            Debug.Log($"Parasite possessed {fishSize} fish!");
            // Handle possession logic
        }
        
        public float GetLifespanBonus()
        {
            return lifespanBonus;
        }
        
        public float GetPossessionDifficulty()
        {
            return possessionDifficulty;
        }
    }
}
