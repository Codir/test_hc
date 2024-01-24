using CoreGameplay.Components;
using UnityEngine;
using Utils;

namespace CoreGameplay.Views
{
    public class PlayerProjectileView : MonoBehaviour
    {
        [SerializeField] private float ShootForce;
        [SerializeField] private CollisionProvider Collider;
        [SerializeField] private ChargingComponent ChargingComponent;

        private Rigidbody _rigidbody;

        public void Show()
        {
            gameObject.SetActive(true);

            Collider.OnCollisionEnterEvent -= OnCollisionEnterCallback;
            Collider.OnCollisionEnterEvent += OnCollisionEnterCallback;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            var currentTransform = transform;
            currentTransform.position = Vector3.zero;
            currentTransform.rotation = Quaternion.identity;
            _rigidbody.velocity = Vector3.zero;

            Collider.OnCollisionEnterEvent -= OnCollisionEnterCallback;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnterCallback(Collision collision)
        {
            var hittable = collision.gameObject.GetComponent<IHittable>();

            if (hittable == null) return;
            hittable.OnHit(ChargingComponent.CurrentCharge);

            Hide();
        }

        public void AddCharge(float value)
        {
            ChargingComponent.Add(value);
        }

        public void Shoot(Vector3 direction)
        {
            SoundsController.PlaySound(Sfx.Shoot);

            var force = direction * ShootForce;
            _rigidbody.AddForce(force);
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            ChargingComponent.CurrentCharge = 0;
        }
    }
}