using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField]
        [SceneObjectsOnly]
        private TextMeshProUGUI _minScore;

        [SerializeField]
        [SceneObjectsOnly]
        private TextMeshProUGUI _maxScore;

        [SerializeField]
        [SceneObjectsOnly]
        private TextMeshProUGUI _avgScore;
        
        [SerializeField]
        [SceneObjectsOnly]
        private JudgeTable _data;

        private void Awake()
        {
            if(_data == null || _minScore ==null || _maxScore == null || _avgScore == null)
                throw new Exception("Not Assigned!");
        }

        private void Update()
        {
            _maxScore.SetText($"Max: {_data._scores.maxScore:0.00}");
            _avgScore.SetText($"Avg: {_data._scores.avgScore * 100.0f :0.00}");
            _minScore.SetText($"Min: {_data._scores.minScore:0.00}");
        }
    }
}