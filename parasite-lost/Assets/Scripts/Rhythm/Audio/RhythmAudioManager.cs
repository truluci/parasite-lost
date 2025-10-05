using UnityEngine;

namespace ParasiteLost.Rhythm.Audio
{
    public class RhythmAudioManager : MonoBehaviour
    {
        [Header("Background Music")]
        public AudioClip battleTheme;
        public float musicVolume = 0.7f;
        
        [Header("Sound Effects")]
        public AudioClip[] hitSuccessSounds; // Multiple sounds for variety
        public AudioClip[] hitMissSounds;
        public AudioClip battleStartSound;
        public AudioClip battleEndSound;
        public AudioClip heartLostSound;
        
        [Header("Audio Sources")]
        public AudioSource musicAudioSource;
        public AudioSource sfxAudioSource;
        
        [Header("Audio Settings")]
        public float sfxVolume = 0.8f;
        public bool randomizePitch = true;
        public float pitchVariation = 0.1f;
        
        private void Start()
        {
            SetupAudioSources();
        }
        
        private void SetupAudioSources()
        {
            // Setup music audio source
            if (musicAudioSource != null)
            {
                musicAudioSource.volume = musicVolume;
                musicAudioSource.loop = true;
                
                if (battleTheme != null)
                {
                    musicAudioSource.clip = battleTheme;
                }
            }
            
            // Setup SFX audio source
            if (sfxAudioSource != null)
            {
                sfxAudioSource.volume = sfxVolume;
                sfxAudioSource.loop = false;
            }
        }
        
        public void StartBattleMusic()
        {
            if (musicAudioSource != null && battleTheme != null)
            {
                musicAudioSource.clip = battleTheme;
                musicAudioSource.Play();
                Debug.Log("Battle music started");
            }
        }
        
        public void StopBattleMusic()
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.Stop();
                Debug.Log("Battle music stopped");
            }
        }
        
        public void PlayHitSuccessSound()
        {
            if (hitSuccessSounds != null && hitSuccessSounds.Length > 0)
            {
                AudioClip soundToPlay = hitSuccessSounds[Random.Range(0, hitSuccessSounds.Length)];
                PlaySFX(soundToPlay);
            }
        }
        
        public void PlayHitMissSound()
        {
            if (hitMissSounds != null && hitMissSounds.Length > 0)
            {
                AudioClip soundToPlay = hitMissSounds[Random.Range(0, hitMissSounds.Length)];
                PlaySFX(soundToPlay);
            }
        }
        
        public void PlayBattleStartSound()
        {
            if (battleStartSound != null)
            {
                PlaySFX(battleStartSound);
            }
        }
        
        public void PlayBattleEndSound()
        {
            if (battleEndSound != null)
            {
                PlaySFX(battleEndSound);
            }
        }
        
        public void PlayHeartLostSound()
        {
            if (heartLostSound != null)
            {
                PlaySFX(heartLostSound);
            }
        }
        
        private void PlaySFX(AudioClip clip)
        {
            if (sfxAudioSource != null && clip != null)
            {
                // Randomize pitch for variety
                if (randomizePitch)
                {
                    float pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
                    sfxAudioSource.pitch = pitch;
                }
                else
                {
                    sfxAudioSource.pitch = 1f;
                }
                
                sfxAudioSource.PlayOneShot(clip);
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicAudioSource != null)
            {
                musicAudioSource.volume = musicVolume;
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxAudioSource != null)
            {
                sfxAudioSource.volume = sfxVolume;
            }
        }
        
        // Method to sync sound effects with the beat of the music
        public void PlaySyncedHitSound(float beatProgress)
        {
            // You can use beatProgress (0-1) to modify the sound based on music timing
            // For example, play different variations based on where we are in the beat
            
            if (beatProgress < 0.25f)
            {
                // On the beat - play the primary hit sound
                PlayHitSuccessSound();
            }
            else if (beatProgress < 0.75f)
            {
                // Off beat - maybe play a slightly different variation
                PlayHitSuccessSound();
            }
            else
            {
                // Near next beat - play standard sound
                PlayHitSuccessSound();
            }
        }
        
        public bool IsMusicPlaying()
        {
            return musicAudioSource != null && musicAudioSource.isPlaying;
        }
        
        public float GetMusicTime()
        {
            return musicAudioSource != null ? musicAudioSource.time : 0f;
        }
    }
}