using System.Collections.Generic;
using Configs;
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
        [SerializeField] private Transform LevelContainer;
        [SerializeField] private LevelConfig LevelConfig;

        private PlayerController _player;
        private PlayerPathView _pathView;
        private TargetView _exit;

        private List<LevelObjectView> _obstacles = new();
        private Vector3 _pathPosition;

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

            _pathPosition = (PlayerSpawnPosition.position + ExitSpawnPosition.position) / 2f;
            _pathView = Instantiate(PlayerPathPrefab, _pathPosition, Quaternion.identity);

            _player.OnCurrentSizeChangedEvent += _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction += CoreGameplayController.OnLevelCompleted;
            _pathView.Clear();

            CoreGameplayController.Init(_player, _pathView, _exit);

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            var levelConfig = LevelConfig;

            var segmentsCount = levelConfig.Segments.Length;
            //TODO: take 2f from padding value
            var pathLength = (PlayerSpawnPosition.position - ExitSpawnPosition.position).magnitude - 2f;
            var segmentLength = segmentsCount > 0 ? pathLength / levelConfig.Segments.Length : pathLength;

            for (var segmentId = 0; segmentId < segmentsCount; segmentId++)
            {
                //TODO: move all this data to model
                var segmentPosition =
                    _pathPosition - (pathLength / 2f - (segmentId + 0.5f) * segmentLength) * Vector3.forward;
                GenerateSegment(levelConfig.Segments[segmentId], segmentLength, segmentPosition);
            }

            /*var pathPosition = (PlayerSpawnPosition.position + ExitSpawnPosition.position) / 2f;
            var obstacle = Instantiate(ObstaclePrefab, pathPosition, Quaternion.identity);
            _obstacles.Add(obstacle);*/
        }

        //TODO: move it to level generator
        private void GenerateSegment(LevelSegmentConfig segmentConfig, float segmentLength, Vector3 segmentPosition)
        {
            var sizeX = segmentConfig.LevelObject.Size.x;
            var sizeZ = segmentConfig.LevelObject.Size.z;
            var rows = segmentConfig.Width / sizeX;
            var cols = segmentLength / sizeZ;

            Random.InitState(segmentConfig.Seed > 0 ? segmentConfig.Seed : Random.Range(0, int.MaxValue));

            for (var x = 0; x < rows; x++)
            {
                for (var z = 0; z < cols; z++)
                {
                    if (Random.value > segmentConfig.Density) continue;

                    var levelObjectPosition = segmentPosition;
                    levelObjectPosition.x += (-rows / 2f + x + 1) * sizeX;
                    levelObjectPosition.z += (-cols / 2f + z + 1) * sizeZ;
                    var obstacle = Instantiate(segmentConfig.LevelObject, LevelContainer);
                    obstacle.transform.position = levelObjectPosition;
                    _obstacles.Add(obstacle);
                }
            }
        }

        public void UnloadLevel()
        {
            _player.OnCurrentSizeChangedEvent -= _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction -= CoreGameplayController.OnLevelCompleted;

            _obstacles.Clear();

            Destroy(_player.gameObject);
            Destroy(_pathView.gameObject);
            Destroy(_exit.gameObject);
        }
    }
}