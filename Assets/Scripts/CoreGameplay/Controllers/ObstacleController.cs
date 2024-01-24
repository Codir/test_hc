using System.Collections;
using UnityEngine;
using Utils;

namespace CoreGameplay.Controllers
{
    public class ObstacleController : BaseLevelObject
    {
        [SerializeField] private Material BaseMaterial;
        [SerializeField] private Material OnHitMaterial;
        [SerializeField] private Renderer Renderer;
        [SerializeField] private CollisionProvider Collider;
        [SerializeField] private int ExplosionDelay;
        [SerializeField] private ExplosionController ExplosionPrefab;

        private float _charge;
        private IEnumerator _explosionCoroutine;

        private void OnEnable()
        {
            Collider.OnHitEvent += OnHit;

            Init();
        }

        private void OnDisable()
        {
            Collider.OnHitEvent -= OnHit;

            if (_explosionCoroutine == null) return;

            StopCoroutine(_explosionCoroutine);
            _explosionCoroutine = null;
        }

        private void Init()
        {
            Renderer.material = BaseMaterial;
        }

        private void OnHit(float value)
        {
            Renderer.material = OnHitMaterial;

            _charge = value;
            _explosionCoroutine = Explosion();
            StartCoroutine(_explosionCoroutine);
        }

        private IEnumerator Explosion()
        {
            yield return new WaitForSeconds(ExplosionDelay);

            CreateExplosion();

            ObjectsPoolController.ReturnLevelObjectToPool(this);
            LevelController.Instance.OnRemoveObstacle(this);
        }

        private void CreateExplosion()
        {
            var explosion = Instantiate(ExplosionPrefab, LevelController.Instance.LevelContainer);
            explosion.transform.position = transform.position;
            explosion.Init(_charge);
            LevelController.Instance.AddExplosion(explosion);
        }
    }
}