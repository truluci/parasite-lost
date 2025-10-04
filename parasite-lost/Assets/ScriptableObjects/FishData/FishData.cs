using UnityEngine;
using ParasiteLost.Core.Hosts;

namespace ParasiteLost.ScriptableObjects.FishData
{
    [CreateAssetMenu(fileName = "New Fish Data", menuName = "Parasite Lost/Fish Data")]
    public class FishData : ScriptableObject
    {
        [Header("Fish Properties")]
        public string fishName;
        public FishHost.FishSize fishSize;
        public float lifespanBonus;
        public float possessionDifficulty;
        
        [Header("Visual")]
        public Sprite fishSprite;
        public Color fishColor = Color.white;
        
        [Header("Rhythm Battle")]
        public AudioClip rhythmTrack;
        public float beatInterval = 1f;
        public int requiredHits = 5;
        
        [Header("Gameplay")]
        public float movementSpeed = 3f;
        public float detectionRange = 2f;
    }
}
