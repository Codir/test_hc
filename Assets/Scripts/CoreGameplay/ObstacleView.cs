using System.Collections;
using UnityEngine;
using Utils;

namespace CoreGameplay
{
    public class ObstacleView : LevelObjectView
    {
        [SerializeField] private Material BaseMaterial;
        [SerializeField] private Material OnHitMaterial;
        [SerializeField] private Renderer Renderer;
        [SerializeField] private CollisionProvider Collider;
        [SerializeField] private int ExplosionDelay;
        [SerializeField] private ExplosionView ExplosionPrefab;

        private float _charge;
        private IEnumerator _explosion;

        private void OnEnable()
        {
            Collider.OnHitEvent += OnHit;

            Init();
        }

        private void OnDisable()
        {
            Collider.OnHitEvent -= OnHit;

            if (_explosion == null) return;

            StopCoroutine(_explosion);
            _explosion = null;
        }

        private void Init()
        {
            Renderer.material = BaseMaterial;
        }

        private void OnHit(float value)
        {
            Renderer.material = OnHitMaterial;

            _charge = value;
            _explosion = Explosion();
            StartCoroutine(_explosion);
        }

        private IEnumerator Explosion()
        {
            yield return new WaitForSeconds(ExplosionDelay);

            CreateExplosion();

            ObjectsPoolController.ReturnLevelObjectToPool(this);
            CoreGameplayController.Instance.OnRemoveObstacle(this);
        }

        private void CreateExplosion()
        {
            var explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            explosion.Init(_charge);
        }
    }
}