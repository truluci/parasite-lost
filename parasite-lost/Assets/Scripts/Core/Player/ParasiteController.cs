using UnityEngine;

namespace ParasiteLost.Core.Player
{
    public class ParasiteController : MonoBehaviour
    {
        [Header("Parasite Settings")]
        public float baseLifespan = 10f;
        public float currentLifespan;
        
        [Header("Movement")]
        public float moveSpeed = 5f;
        
        private void Start()
        {
            currentLifespan = baseLifespan;
        }
        
        private void Update()
        {
            UpdateLifespan();
            HandleMovement();
        }
        
        private void UpdateLifespan()
        {
            currentLifespan -= Time.deltaTime;
            
            if (currentLifespan <= 0)
            {
                Debug.LogError("Parasite lifespan expired!");
                // Handle game over
            }
        }
        
        private void HandleMovement()
        {
            // Basic movement input handling
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(horizontal, vertical, 0) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
        
        public void ExtendLifespan(float amount)
        {
            currentLifespan += amount;
            Debug.Log($"Lifespan extended by {amount}. Current lifespan: {currentLifespan}");
        }
    }
}
