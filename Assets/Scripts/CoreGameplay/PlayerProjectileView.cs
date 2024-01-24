using UnityEngine;
using Utils;

namespace CoreGameplay
{
    public class PlayerProjectileView : MonoBehaviour
    {
        [SerializeField] private float SizeChangeMultiplier;
        [SerializeField] private float ModeSpeed;
        [SerializeField] private CollisionProvider Collider;
        [SerializeField] private Transform Body;

        //TODO: change to correct calling setter
        private float _chargeValue;
        private Rigidbody _rigidbody;

        private float ChargeValue
        {
            get => _chargeValue;
            set
            {
                _chargeValue = value;
                OnSizeChanged();
            }
        }

        public void Show(Transform spawnPosition)
        {
            gameObject.SetActive(true);
            _rigidbody.position = transform.position = spawnPosition.position;

            Collider.OnCollisionEnterEvent -= OnCollisionEnterCallback;
            Collider.OnCollisionEnterEvent += OnCollisionEnterCallback;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _rigidbody.velocity = Vector3.zero;

            Collider.OnCollisionEnterEvent -= OnCollisionEnterCallback;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnterCallback(Collision collision)
        {
            //TODO: move code to separate class
            var hittable = collision.gameObject.GetComponent<IHittable>();

            if (hittable == null) return;

            hittable.OnHit(_chargeValue);

            Hide();
        }

        public void GrownProjectile(float value)
        {
            ChargeValue += value * SizeChangeMultiplier;
        }

        public void Shoot(Vector3 direction)
        {
            SoundsController.PlaySound(Sfx.Shoot);

            _rigidbody.AddForce(direction * ModeSpeed * 1000f);
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            ChargeValue = 0;
        }

        private void OnSizeChanged()
        {
            Body.localScale = Vector3.one * ChargeValue;
        }
    }
}