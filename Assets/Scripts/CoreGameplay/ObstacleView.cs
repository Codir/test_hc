using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace CoreGameplay
{
    public class ObstacleView : MonoBehaviour
    {
        //TODO: add pooling of obstacles
        [SerializeField] private Material BaseMaterial;
        [SerializeField] private Material OnHitMaterial;
        [SerializeField] private Renderer Renderer;
        [SerializeField] private CollisionProvider Collider;

        [SerializeField] private int ExplosionDelay;
        //TODO: add pooling of explosions
        [SerializeField] private ExplosionView ExplosionPrefab;

        private float _charge;

        private void Start()
        {
            Collider.OnHitEvent += OnHit;

            Init();
        }

        private void OnDestroy()
        {
            //TODO: make for using same object instead obstacleView and IHittable
            CoreGameplayController.Instance.OnRemoveObstacle(this);
            
            Collider.OnHitEvent -= OnHit;
        }

        public void Init()
        {
            Renderer.material = BaseMaterial;
        }

        public void OnHit(float value)
        {
            Renderer.material = OnHitMaterial;

            _charge = value;
            Explosion();
        }

        private async void Explosion()
        {
            await Task.Delay(ExplosionDelay);

            var explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            explosion.Init(_charge);

            //TODO: move to pooling manager
            Destroy(gameObject);
        }
    }
}