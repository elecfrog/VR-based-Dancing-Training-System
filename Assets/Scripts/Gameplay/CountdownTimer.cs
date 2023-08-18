using System;
using Sirenix.OdinInspector;
using UI;
using UltimateReplay;
using UnityEngine;

namespace Gameplay
{
    public class CountdownTimer : SerializedMonoBehaviour
    {
        public float totalTime = 30.0f; // time scale in seconds

        [SerializeField] 
        private float remainingTime = 0.0f;


        [SceneObjectsOnly]
        public Humanoid.AnimatorSequenceController animController;

        [SceneObjectsOnly]
        public Canvas canvas;

        [SceneObjectsOnly]
        [SerializeField]
        private ReplayControls replayController;
        
        [AssetsOnly]
        public GameObject countDownTextPrefab;

        private CountDownText countDownText;


        private void Awake()
        {
            // init the count down time
            remainingTime = totalTime;
        }

        private void Start()
        {
            if (animController == null)
                throw new Exception("Animation Controller Does not Exist!");
        }

        // invoke by button click
        public void StartTimer()
        {
            // stop animation at first
            animController.Stop();

            // count down text
            GameObject instance = Instantiate(countDownTextPrefab, canvas.transform, false);

            countDownText = instance.GetComponent<CountDownText>();
        }

        private void Update()
        {
            if (countDownText == null)
                return;

            // Check if the coroutine has finished
            if (countDownText.m_Completed)
            {
                // when the text animated, start dancing
                animController.PlayStart();
                replayController.StartRecord();
                countDownText = null;
            }
        }

        // Start Countdown Coroutine
        private void StartCountdown()
        {
            GameObject instance = Instantiate(countDownTextPrefab);
            instance.transform.SetParent(canvas.transform, false);
        }
    }
}