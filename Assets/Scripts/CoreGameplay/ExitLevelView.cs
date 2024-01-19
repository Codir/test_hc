using UnityEngine;
using Utils;

namespace CoreGameplay
{
    public class TargetView : MonoBehaviour
    {
        [SerializeField] private CollisionProvider OpenGateTrigger;
        [SerializeField] private Animator GateAnimator;

        private bool _isOpenedGate;
        private static readonly int IsOpened = Animator.StringToHash("IsOpened");

        private void OnEnable()
        {
            OpenGateTrigger.OnTriggerEnterEvent -= OnOpenGateTriggerEnter;
            OpenGateTrigger.OnTriggerEnterEvent += OnOpenGateTriggerEnter;
        }

        private void OnDisable()
        {
            CloseGate();

            OpenGateTrigger.OnTriggerEnterEvent -= OnOpenGateTriggerEnter;
        }

        private void OnOpenGateTriggerEnter(Collider otherCollider)
        {
            var player = otherCollider.GetComponent<PlayerController>();

            if (player == null)
                return;

            OpenGate();

            Debug.Log("OnOpenGateTriggerEnter");
        }

        private void OpenGate()
        {
            if (_isOpenedGate) return;

            GateAnimator.SetBool(IsOpened, true);
        }

        private void CloseGate()
        {
            if (!_isOpenedGate) return;

            GateAnimator.SetBool(IsOpened, false);
        }
    }
}