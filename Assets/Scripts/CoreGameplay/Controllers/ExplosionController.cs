using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace CoreGameplay.Controllers
{
    public class ExplosionController : MonoBehaviour
    {
        [SerializeField] private float SizeMultiplier;
        [SerializeField] private float ChargeMultiplier;
        [SerializeField] private float SizeChangeSpeed;
        [SerializeField] private Vector3 BaseSize;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tween;

        private float _chargeValue;

        public void Init(float value)
        {
            _chargeValue = value * ChargeMultiplier;
            transform.localScale = BaseSize;

            var scale = value * SizeMultiplier;
            var delay = scale / SizeChangeSpeed;
            _tween = transform.DOScale(scale, delay).OnComplete(OnAnimationCompleted);
        }

        private void OnDestroy()
        {
            LevelController.Instance.RemoveExplosion(this);
            StopAnimation();
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            var hittable = otherCollider.gameObject.GetComponent<IHittable>();
            hittable?.OnHit(_chargeValue);
        }

        private void StopAnimation()
        {
            _tween?.Kill();
        }

        private void OnAnimationCompleted()
        {
            SoundsController.PlaySound(Sfx.Splash);

            Destroy(gameObject);
        }
    }
}