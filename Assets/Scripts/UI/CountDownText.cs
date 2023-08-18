using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

namespace UI
{
    public class CountDownText : SerializedMonoBehaviour
    {
        public bool m_Completed = false;
        [SerializeField] private GameObject m_TextObject3;
        private TextMeshProUGUI m_Text3;

        [SerializeField] private GameObject m_TextObject2;
        private TextMeshProUGUI m_Text2;

        [SerializeField] private GameObject m_TextObject1;
        private TextMeshProUGUI m_Text1;

        [SerializeField] private GameObject m_TextObjectGo;
        private TextMeshProUGUI m_TextGo;

        [SerializeField] private RectTransform m_CenterPosition;

        private void Awake() {
            // Debug.Log("Test it !");
            m_Text3 = m_TextObject3.GetComponent<TextMeshProUGUI>();
            m_Text2 = m_TextObject2.GetComponent<TextMeshProUGUI>();
            m_Text1 = m_TextObject1.GetComponent<TextMeshProUGUI>();
            m_TextGo = m_TextObjectGo.GetComponent<TextMeshProUGUI>();

            // onAllCoroutinesCompleted = new UnityEvent();
        }

        private IEnumerator Start() {
            Vector3 centerPosition = m_CenterPosition.transform.position;

            // yield return new WaitForSeconds(1);
            m_TextObject3.SetActive(true);
            m_TextObject3.transform.DOMove(centerPosition, 1)
                .OnComplete(() =>
                {
                    // 创建一个新的协程来处理后续的动画
                    StartCoroutine(ChangeColorAndFadeOut(m_Text3));
                });


            yield return new WaitForSeconds(1);
            m_TextObject2.SetActive(true);
            m_TextObject2.transform.DOMove(centerPosition, 1)
                .OnComplete(() =>
                {
                    // 创建一个新的协程来处理后续的动画
                    StartCoroutine(ChangeColorAndFadeOut(m_Text2));
                });

            yield return new WaitForSeconds(1);
            m_TextObject1.SetActive(true);
            m_TextObject1.transform.DOMove(centerPosition, 1)
                .OnComplete(() =>
                {
                    // 创建一个新的协程来处理后续的动画
                    StartCoroutine(ChangeColorAndFadeOut(m_Text1));
                });

            yield return new WaitForSeconds(1);
            m_TextObjectGo.SetActive(true);
            m_TextObjectGo.transform.DOMove(centerPosition, 1)
                .OnComplete(() =>
                {
                    // 创建一个新的协程来处理后续的动画
                    StartCoroutine(ChangeColorAndFadeOutGo(m_TextGo));
                });
        }

        private IEnumerator ChangeColorAndFadeOut(TextMeshProUGUI text) {
            // 等待一秒
            // yield return new WaitForSeconds(1);

            // 开始变大的动画
            var scaleTween = text.transform.DOScale(2.0f, 0.5f); // 这里的2.0f是新的大小，1.0f是动画的持续时间

            // 等待变大动画结束
            yield return scaleTween.WaitForCompletion();

            // 开始改变颜色的动画
            var colorTween = text.DOColor(new Color(0, 0, 0, 0), 0.5f);

            // 等待颜色改变动画结束
            yield return colorTween.WaitForCompletion();
        }

        private IEnumerator ChangeColorAndFadeOutGo(TextMeshProUGUI text) {
            // 等待一秒
            // yield return new WaitForSeconds(1);

            // 开始变大的动画
            var scaleTween = text.transform.DOScale(2.0f, 0.5f); // 这里的2.0f是新的大小，1.0f是动画的持续时间

            // 等待变大动画结束
            yield return scaleTween.WaitForCompletion();

            // 开始改变颜色的动画
            var colorTween = text.DOColor(new Color(0, 0, 0, 0), 0.5f);

            m_Completed = true;

            // 等待颜色改变动画结束
            yield return colorTween.WaitForCompletion();
        }
    }
}