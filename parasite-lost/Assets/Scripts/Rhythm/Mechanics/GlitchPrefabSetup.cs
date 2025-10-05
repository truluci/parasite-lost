using UnityEngine;

namespace ParasiteLost.Rhythm.Mechanics
{
    /// <summary>
    /// Helper script to automatically set up glitch prefabs with required components
    /// Add this to your glitch prefab to ensure it has all necessary components
    /// </summary>
    public class GlitchPrefabSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public bool autoSetupOnAwake = true;
        
        [Header("Visual Settings")]
        public Sprite glitchSprite;
        public Color glitchColor = Color.red;
        public Vector2 glitchSize = new Vector2(1f, 1f);
        
        [Header("Physics Settings")]  
        public bool addCollider = true;
        public bool addRigidBody = false;
        
        private void Awake()
        {
            if (autoSetupOnAwake)
            {
                SetupGlitchComponents();
            }
        }
        
        [ContextMenu("Setup Glitch Components")]
        public void SetupGlitchComponents()
        {
            // Ensure SpriteRenderer exists
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            
            // Configure sprite renderer
            if (glitchSprite != null)
            {
                spriteRenderer.sprite = glitchSprite;
            }
            spriteRenderer.color = glitchColor;
            transform.localScale = new Vector3(glitchSize.x, glitchSize.y, 1f);
            
            // Ensure GlitchNote component exists
            if (GetComponent<GlitchNote>() == null)
            {
                gameObject.AddComponent<GlitchNote>();
            }
            
            // Add collider if requested
            if (addCollider && GetComponent<Collider2D>() == null)
            {
                BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
                collider.isTrigger = true; // Usually glitches should be triggers
            }
            
            // Add rigidbody if requested
            if (addRigidBody && GetComponent<Rigidbody2D>() == null)
            {
                Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; // Glitches shouldn't fall
                rb.isKinematic = true; // Movement is controlled by script
            }
            
            Debug.Log($"Glitch prefab '{gameObject.name}' setup completed!");
        }
        
        // This method can be called to create a glitch at runtime
        public static GameObject CreateGlitch(Vector3 position, Transform parent = null)
        {
            GameObject glitch = new GameObject("RuntimeGlitch");
            glitch.transform.position = position;
            if (parent != null)
            {
                glitch.transform.SetParent(parent);
            }
            
            // Add setup component and configure
            GlitchPrefabSetup setup = glitch.AddComponent<GlitchPrefabSetup>();
            setup.SetupGlitchComponents();
            
            return glitch;
        }
        
        private void OnValidate()
        {
            // Update visual settings in real-time in the editor
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                if (glitchSprite != null)
                {
                    spriteRenderer.sprite = glitchSprite;
                }
                spriteRenderer.color = glitchColor;
                transform.localScale = new Vector3(glitchSize.x, glitchSize.y, 1f);
            }
        }
    }
}