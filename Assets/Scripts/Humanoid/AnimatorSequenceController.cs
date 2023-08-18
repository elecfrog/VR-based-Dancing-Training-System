using Sirenix.OdinInspector;
using UnityEngine;

namespace Humanoid
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorSequenceController : SerializedMonoBehaviour
    {
        private Animator m_Animator;
        private AnimatorControllerParameter[] m_AnimtorParameters;

        // Hashes for animator states
        private int hashStartDance;
        // private int hashWalking;
        private int hashIdle;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_AnimtorParameters = m_Animator.parameters;

            // get all parameter data
            foreach (AnimatorControllerParameter parameter in m_AnimtorParameters)
            {
                Debug.Log("Parameter name: " + parameter.name);
                Debug.Log("Parameter type: " + parameter.type);
            }

            // Convert strings to hashes
            // idle, ready, or stop
            hashIdle = Animator.StringToHash("Idle");

            hashStartDance = Animator.StringToHash("01");
        }

        // from first animation clip to play
        public void PlayStart()
        {
            m_Animator.speed = 1;
            m_Animator.Play(hashStartDance, -1, 0);
        }

        public void Pause() => m_Animator.speed = 0;

        public void Continue() => m_Animator.speed = 1;

        public void Stop() => m_Animator.Play(hashIdle);

        public void Resume()
        {
            m_Animator.speed = 1;
            m_Animator.Play(hashStartDance, -1, 0);
        }
    }
}