using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ParasiteLost.UI.MainMenu
{
    /// <summary>
    /// Simple frame-by-frame sprite animation player intended for displaying an imported GIF
    /// (exported as a sequence of PNG sprites) on a UI Image before loading the game.
    /// Place all frames in a folder and assign them to the frames array in order, or
    /// call LoadFromResourcesFolder to auto-populate by folder name under a Resources path.
    /// </summary>
    public class AnimatedImagePlayer : MonoBehaviour
    {
        [Header("Target")]
        public Image targetImage;               // UI Image to update (required)

        [Header("Frames")] 
        public Sprite[] frames;                 // Ordered frames of the intro animation
        public float framesPerSecond = 12f;      // Playback speed
        public bool autoPlayOnEnable = false;    // Auto start when enabled
        public bool disableRaycastDuringPlay = true; // Optional: disable raycast target while animating

        [Header("Behaviour")] 
        public bool hideImageBeforePlay = true; // Hide (alpha 0) until first frame set
        public bool disableGameObjectOnComplete = false; // Disable gameObject when finished
        public bool destroyOnComplete = false;  // Destroy component when finished

        [Header("Events")] 
        public UnityEvent onAnimationFinished;  // Invoked when animation finishes naturally

        private Coroutine playRoutine;
        private bool isPlaying;
        public bool IsPlaying => isPlaying;

        private void Reset()
        {
            targetImage = GetComponent<Image>();
        }

        private void Awake()
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }

            if (hideImageBeforePlay && targetImage != null)
            {
                var c = targetImage.color;
                c.a = 0f;
                targetImage.color = c;
            }
        }

        private void OnEnable()
        {
            if (autoPlayOnEnable)
            {
                Play();
            }
        }

        /// <summary>
        /// Start playing the animation from the beginning.
        /// </summary>
        public void Play()
        {
            if (frames == null || frames.Length == 0 || targetImage == null)
            {
                Debug.LogWarning("[AnimatedImagePlayer] Cannot play - frames or targetImage missing");
                return;
            }

            Stop();
            playRoutine = StartCoroutine(PlayRoutine());
        }

        /// <summary>
        /// Stops the animation if it is playing.
        /// </summary>
        public void Stop()
        {
            if (playRoutine != null)
            {
                StopCoroutine(playRoutine);
                playRoutine = null;
            }
            isPlaying = false;
        }

        /// <summary>
        /// Total duration in seconds based on framesPerSecond and frame count.
        /// </summary>
        public float GetTotalDuration()
        {
            if (frames == null || frames.Length == 0 || framesPerSecond <= 0f) return 0f;
            return frames.Length / framesPerSecond;
        }

        private IEnumerator PlayRoutine()
        {
            isPlaying = true;
            if (disableRaycastDuringPlay && targetImage != null)
            {
                targetImage.raycastTarget = false;
            }

            float frameTime = 1f / Mathf.Max(1f, framesPerSecond);
            int index = 0;

            // Fade in if hidden
            if (hideImageBeforePlay && targetImage != null)
            {
                var c = targetImage.color;
                c.a = 1f;
                targetImage.color = c;
            }

            while (index < frames.Length)
            {
                targetImage.sprite = frames[index];
                index++;
                yield return new WaitForSeconds(frameTime);
            }

            isPlaying = false;
            onAnimationFinished?.Invoke();

            if (disableRaycastDuringPlay && targetImage != null)
            {
                targetImage.raycastTarget = true;
            }

            if (disableGameObjectOnComplete)
            {
                gameObject.SetActive(false);
            }

            if (destroyOnComplete)
            {
                Destroy(this);
            }
        }
    }
}
