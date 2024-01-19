using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace CoreGameplay
{
    public class CoreGameplayController : MonoBehaviour
    {
        //TODO: move it to config
        [SerializeField] private float PlayerMoveSpeed;

        //TODO: remove
        public static CoreGameplayController Instance;

        private PlayerController _player;
        private PlayerPathView _pathView;
        private TargetView _exit;
        private TweenerCore<Vector3, Vector3, VectorOptions> _playerMoveTween;

        public void OnRemoveObstacle(ObstacleView obstacle)
        {
            _pathView.OnRemoveObstacle(obstacle);
        }

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            LoadLevel();
        }

        //TODO: move it to game model
        public void Init(PlayerController player, PlayerPathView pathView, TargetView exit)
        {
            _player = player;
            _pathView = pathView;
            _exit = exit;
        }

        private void OnDestroy()
        {
            _playerMoveTween?.Kill();

            UnloadLevel();
        }

        //TODO: move LoadLevel and UnloadLevel to levels manager
        public void LoadLevel()
        {
            _player.OnCurrentSizeChangedEvent += _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction += OnLevelCompleted;
            _pathView.Clear();
        }

        public void UnloadLevel()
        {
            _player.OnCurrentSizeChangedEvent -= _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction -= OnLevelCompleted;
        }

        public void OnLevelCompleted()
        {
            Debug.Log($"OnLevelCompleted");

            //TODO: add SFX

            var distanceToTarget = Vector3.Distance(_player.transform.position, _exit.transform.position);
            var delay = distanceToTarget / PlayerMoveSpeed;
            _player.StartMoveAnimation(PlayerMoveSpeed);
            _playerMoveTween = _player.transform.DOMove(_exit.transform.position, delay)
                .OnComplete(OnWinAnimationCompleted);
        }

        private void OnWinAnimationCompleted()
        {
            //TODO: add SFX and vibration feedback
            _player.EndMoveAnimation();
            Debug.Log($"OnWinAnimationCompleted");
            
            LevelController.Instance.UnloadLevel();
            LevelController.Instance.LoadLevel();
        }
    }
}