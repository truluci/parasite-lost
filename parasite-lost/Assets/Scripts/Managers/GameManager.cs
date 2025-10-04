using UnityEngine;
using ParasiteLost.Core.Player;
using ParasiteLost.Rhythm.System;

namespace ParasiteLost.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        public GameState currentState;
        
        [Header("References")]
        public ParasiteController parasiteController;
        public RhythmManager rhythmManager;
        
        public enum GameState
        {
            MainMenu,
            Playing,
            RhythmBattle,
            Paused,
            GameOver
        }
        
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
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
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            currentState = GameState.MainMenu;
            Debug.Log("Game initialized");
        }
        
        public void StartGame()
        {
            currentState = GameState.Playing;
            Debug.Log("Game started");
        }
        
        public void StartRhythmBattle()
        {
            currentState = GameState.RhythmBattle;
            rhythmManager?.StartRhythmBattle();
            Debug.Log("Rhythm battle started");
        }
        
        public void EndRhythmBattle()
        {
            currentState = GameState.Playing;
            rhythmManager?.StopRhythmBattle();
            Debug.Log("Rhythm battle ended");
        }
        
        public void PauseGame()
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;
            Debug.Log("Game paused");
        }
        
        public void ResumeGame()
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;
            Debug.Log("Game resumed");
        }
        
        public void GameOver()
        {
            currentState = GameState.GameOver;
            Debug.LogError("Game Over!");
        }
    }
}
