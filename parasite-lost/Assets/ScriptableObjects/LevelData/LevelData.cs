using UnityEngine;
using ParasiteLost.Core.Hosts;

namespace ParasiteLost.ScriptableObjects.LevelData
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "Parasite Lost/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Info")]
        public string levelName;
        public int levelNumber;
        public string description;
        
        [Header("Level Layout")]
        public Vector2 startPosition;
        public Vector2 endPosition;
        public float levelWidth = 20f;
        public float levelHeight = 15f;
        
        [Header("Gameplay")]
        public float timeLimit = 60f;
        public int requiredFishPossessions = 3;
        public FishHost.FishSize[] availableFishSizes;
        
        [Header("Visual")]
        public Sprite backgroundSprite;
        public Color backgroundColor = Color.blue;
        
        [Header("Audio")]
        public AudioClip backgroundMusic;
        public AudioClip ambientSound;
    }
}
