using UnityEngine;

namespace ParasiteLost.Rhythm.Mechanics
{
    public class GlitchNote : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        
        [Header("Hit Detection")]
        public float hitLineX = 0f; // X position of the hitline
        public float hitTolerance = 0.5f; // Distance tolerance for hitting
        
        private bool isActive = true;
        private bool hasPassedHitLine = false;
        private bool isMissed = false; // Track if glitch was missed but still moving
        private RhythmGameController rhythmController;
        
        private void Start()
        {
            // Find the rhythm controller in the scene
            rhythmController = FindFirstObjectByType<RhythmGameController>();
            if (rhythmController == null)
            {
                Debug.LogError("RhythmGameController not found in scene!");
            }
        }
        
        private void Update()
        {
            // Continue moving even if missed, only stop if successfully hit
            if (!isActive && !isMissed) return;
            
            // Move the glitch from right to left
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            
            // Check if glitch has passed the hit line without being hit
            if (!hasPassedHitLine && !isMissed && transform.position.x <= hitLineX - hitTolerance)
            {
                hasPassedHitLine = true;
                OnMissed();
            }
            
            // Destroy glitch when it moves too far left (off screen/end of lane)
            if (transform.position.x < -10f)
            {
                if (isMissed)
                {
                    Debug.Log("Missed glitch reached end of lane and disappeared");
                }
                Destroy(gameObject);
            }
        }
        
        public bool CanBeHit()
        {
            if (!isActive || hasPassedHitLine || isMissed) return false;
            
            float distanceToHitLine = Mathf.Abs(transform.position.x - hitLineX);
            return distanceToHitLine <= hitTolerance;
        }
        
        public void OnHit()
        {
            if (!isActive) return;
            
            isActive = false;
            
            // Notify the rhythm controller about successful hit
            if (rhythmController != null)
            {
                float accuracy = CalculateHitAccuracy();
                rhythmController.OnGlitchHit(accuracy);
            }
            
            // Play hit effect or animation here if needed
            Debug.Log($"Glitch hit with accuracy: {CalculateHitAccuracy()}");
            
            // Destroy the glitch
            Destroy(gameObject);
        }
        
        private void OnMissed()
        {
            if (!isActive || isMissed) return;
            
            // Mark as missed but keep it moving
            isMissed = true;
            // Don't set isActive to false - let it continue moving to end of lane
            
            // Notify the rhythm controller about miss
            if (rhythmController != null)
            {
                rhythmController.OnGlitchMissed();
            }
            
            Debug.Log("Glitch missed! Continuing to end of lane...");
        }
        
        private float CalculateHitAccuracy()
        {
            float distanceToHitLine = Mathf.Abs(transform.position.x - hitLineX);
            float accuracy = 1f - (distanceToHitLine / hitTolerance);
            return Mathf.Clamp01(accuracy);
        }
        
        public void SetHitLinePosition(float xPosition)
        {
            hitLineX = xPosition;
        }
        
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }
    }
}