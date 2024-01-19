using System.Collections.Generic;
using UnityEngine;

namespace CoreGameplay
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] public CoreGameplayController CoreGameplayController;

        [SerializeField] private PlayerController PlayerPrefab;
        [SerializeField] private PlayerPathView PlayerPathPrefab;
        [SerializeField] private TargetView ExitPrefab;
        [SerializeField] private ObstacleView ObstaclePrefab;

        //TODO: remove
        public static LevelController Instance;
        
        //TODO: add spawn player and target, and create PathView by this two params
        [SerializeField] private Transform PlayerSpawnPosition;
        [SerializeField] private Transform ExitSpawnPosition;

        private PlayerController _player;
        private PlayerPathView _pathView;
        private TargetView _exit;

        private List<ObstacleView> _obstacle = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            _player = Instantiate(PlayerPrefab, PlayerSpawnPosition.position, Quaternion.identity);
            _exit = Instantiate(ExitPrefab, ExitSpawnPosition.position, Quaternion.identity);

            var pathPosition = (PlayerSpawnPosition.position + ExitSpawnPosition.position) / 2f;
            _pathView = Instantiate(PlayerPathPrefab, pathPosition, Quaternion.identity);
            
            _player.OnCurrentSizeChangedEvent += _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction += CoreGameplayController.OnLevelCompleted;
            _pathView.Clear();
            
            CoreGameplayController.Init(_player, _pathView, _exit);

            CreateObstacles();
        }

        private void CreateObstacles()
        {
            var pathPosition = (PlayerSpawnPosition.position + ExitSpawnPosition.position) / 2f;
            var obstacle = Instantiate(ObstaclePrefab, pathPosition, Quaternion.identity);
            _obstacle.Add(obstacle);
        }

        public void UnloadLevel()
        {
            _player.OnCurrentSizeChangedEvent -= _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction -= CoreGameplayController.OnLevelCompleted;
            
            _obstacle.Clear();
            
            Destroy(_player.gameObject);
            Destroy(_pathView.gameObject);
            Destroy(_exit.gameObject);
        }
    }
}