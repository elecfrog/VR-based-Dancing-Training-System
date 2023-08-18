using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class ProgressBarController : SerializedMonoBehaviour
    {
        [AssetsOnly]
        public GameObject progressBarPrefab;
        private ReplayProgressBar instanceScript;
        [SceneObjectsOnly]
        [InfoBox("Parent Object of progressBar")]
        public Canvas canvas;

        public void DisplayProgressBar()
        {
            GameObject instance = Instantiate(progressBarPrefab, canvas.transform, false);

            instanceScript = instance.GetComponent<ReplayProgressBar>();
        }
    }
}