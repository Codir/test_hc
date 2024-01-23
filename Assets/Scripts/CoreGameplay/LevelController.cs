using System.Collections.Generic;
using Configs;
using CoreGameplay.Models;
using UnityEngine;

namespace CoreGameplay
{
    public class LevelController : MonoBehaviour
    {
        public CoreGameplayController CoreGameplayController;

        [SerializeField] private PlayerController PlayerPrefab;
        [SerializeField] private PlayerPathView PlayerPathPrefab;
        [SerializeField] private TargetView ExitPrefab;
        [SerializeField] private GameConfig GameConfig;

        //TODO: remove
        public static LevelController Instance;

        [SerializeField] private Transform PlayerSpawnPosition;
        [SerializeField] private Transform ExitSpawnPosition;
        [SerializeField] private Transform LevelContainer;

        private PlayerController _player;
        private PlayerPathView _pathView;
        private TargetView _exit;

        private readonly List<LevelObjectView> _levelObjects = new();
        private Vector3 _pathPosition;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            CoreGameplayController = new CoreGameplayController(GameConfig);
            LoadLevel();
        }

        public void LoadLevel()
        {
            var levelNumber = AppController.Instance.GameStateModel.LevelNumber;

            _player = Instantiate(PlayerPrefab, PlayerSpawnPosition.position, Quaternion.identity);
            _exit = Instantiate(ExitPrefab, ExitSpawnPosition.position, Quaternion.identity);

            _pathPosition = (PlayerSpawnPosition.position + ExitSpawnPosition.position) / 2f;
            _pathView = Instantiate(PlayerPathPrefab, _pathPosition, Quaternion.identity);

            _player.OnCurrentSizeChangedEvent += _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction += CoreGameplayController.OnLevelCompleted;
            _pathView.Clear();

            var coreGameplayModel = new CoreGameplayModel
            {
                Player = _player,
                PathView = _pathView,
                Exit = _exit
            };

            CoreGameplayController.OnLoadLevel(coreGameplayModel);
            GenerateLevel(levelNumber);
        }

        private void GenerateLevel(int levelNumber)
        {
            var levelConfig = GetLevelConfigByNumber(levelNumber);

            if (levelConfig == null) return;

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
        }

        private LevelConfig GetLevelConfigByNumber(int levelNumber)
        {
            if (GameConfig == null) return null;

            return GameConfig.Levels.Length < levelNumber
                ? GameConfig.Levels[levelNumber]
                : GameConfig.Levels[levelNumber % GameConfig.Levels.Length];
        }

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
                    var levelObject = ObjectsPoolController.GetLevelObject(segmentConfig.LevelObject,
                        levelObjectPosition,
                        Quaternion.identity);
                    //var levelObject = Instantiate(segmentConfig.LevelObject, LevelContainer);
                    //levelObject.transform.position = levelObjectPosition;
                    _levelObjects.Add(levelObject);
                }
            }
        }

        public void UnloadLevel()
        {
            _player.OnCurrentSizeChangedEvent -= _pathView.OnCurrentSizeChanged;
            _pathView.OnPathFreeAction -= CoreGameplayController.OnLevelCompleted;
            CoreGameplayController.OnUnloadLevel();

            foreach (var levelObject in _levelObjects)
            {
                ObjectsPoolController.ReturnLevelObjectToPool(levelObject);
                //Destroy(levelObject.gameObject);
            }

            _levelObjects.Clear();

            Destroy(_player.gameObject);
            Destroy(_pathView.gameObject);
            Destroy(_exit.gameObject);
        }

        public void NextLevel()
        {
            AppController.Instance.GameStateModel.LevelNumber++;
        }

        public void ReloadLevel()
        {
            UnloadLevel();
            LoadLevel();
        }
    }
}