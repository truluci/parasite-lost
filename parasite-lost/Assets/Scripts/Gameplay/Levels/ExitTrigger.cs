using UnityEngine;
using UnityEngine.SceneManagement;
using ParasiteLost.Core.Player;

namespace ParasiteLost.Gameplay.Levels
{
    [RequireComponent(typeof(Collider2D))]
    public class ExitTrigger : MonoBehaviour
    {
        [Header("Scene")]
        [Tooltip("Name of the scene to load when the player reaches the exit.")]
        public string nextSceneName = "";

        void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            bool triggered = false;
            if (other.TryGetComponent<ParasiteController>(out var parasite)) triggered = true;
            if (!triggered && other.CompareTag("Player")) triggered = true;
            if (triggered)
            {
                Debug.Log("level completed");
                TryLoadNextScene();
            }
        }

        void TryLoadNextScene()
        {
            string target = nextSceneName;
            if (string.IsNullOrEmpty(target))
            {
                var current = SceneManager.GetActiveScene();
                var currentName = current.name;
                int numberStart = currentName.Length - 1;
                while (numberStart >= 0 && char.IsDigit(currentName[numberStart])) numberStart--;
                numberStart++;
                if (numberStart < currentName.Length)
                {
                    string prefix = currentName.Substring(0, numberStart);
                    string numberStr = currentName.Substring(numberStart);
                    if (int.TryParse(numberStr, out int n)) target = prefix + (n + 1).ToString();
                }
            }

            if (string.IsNullOrEmpty(target))
            {
                Debug.LogWarning("ExitTrigger: could not infer next scene name from current scene and no nextSceneName provided.");
                return;
            }

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (name == target)
                {
                    SceneManager.LoadScene(target);
                    return;
                }
            }

            Debug.LogWarning($"ExitTrigger: Scene '{target}' not found in Build Settings. Add it or set a valid scene name.");
        }
    }
}
