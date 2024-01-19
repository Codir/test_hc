using System;
using CoreGameplay;
using UnityEngine;

namespace Utils
{
    public class CollisionProvider : MonoBehaviour, IHittable
    {
        public event Action<Collision> OnCollisionEnterEvent;
        public event Action<Collision> OnCollisionStayEvent;
        public event Action<Collision> OnCollisionExitEvent;
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerStayEvent;
        public event Action<Collider> OnTriggerExitEvent;
        public event Action<float> OnHitEvent;

        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionEnterEvent?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            OnCollisionStayEvent?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnCollisionExitEvent?.Invoke(collision);
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            OnTriggerEnterEvent?.Invoke(otherCollider);
        }

        private void OnTriggerStay(Collider otherCollider)
        {
            OnTriggerStayEvent?.Invoke(otherCollider);
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            OnTriggerExitEvent?.Invoke(otherCollider);
        }

        public void OnHit(float value)
        {
            OnHitEvent?.Invoke(value);
        }
    }
}