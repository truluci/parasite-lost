using UnityEngine;
using System;

namespace ParasiteLost.Rhythm.System
{
    public class RhythmManager : MonoBehaviour
    {
        [Header("Rhythm Settings")]
        public float beatInterval = 1f;
        public float tolerance = 0.2f;
        
        [Header("Audio")]
        public AudioSource audioSource;
        
        private float nextBeatTime;
        private bool isRhythmActive = false;
        
        public Action OnBeatHit;
        public Action OnBeatMiss;
        
        private void Start()
        {
            nextBeatTime = Time.time + beatInterval;
        }
        
        private void Update()
        {
            if (isRhythmActive)
            {
                CheckForBeat();
            }
        }
        
        public void StartRhythmBattle()
        {
            isRhythmActive = true;
            nextBeatTime = Time.time + beatInterval;
            Debug.Log("Rhythm battle started!");
        }
        
        public void StopRhythmBattle()
        {
            isRhythmActive = false;
            Debug.Log("Rhythm battle stopped!");
        }
        
        private void CheckForBeat()
        {
            if (Time.time >= nextBeatTime - tolerance)
            {
                // Beat window is open
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    HitBeat();
                }
            }
            else if (Time.time >= nextBeatTime + tolerance)
            {
                // Missed beat
                MissBeat();
            }
        }
        
        private void HitBeat()
        {
            OnBeatHit?.Invoke();
            nextBeatTime += beatInterval;
            Debug.Log("Beat hit!");
        }
        
        private void MissBeat()
        {
            OnBeatMiss?.Invoke();
            nextBeatTime += beatInterval;
            Debug.LogError("Beat missed!");
        }
    }
}
