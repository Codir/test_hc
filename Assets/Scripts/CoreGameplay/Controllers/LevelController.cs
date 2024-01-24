using System.Collections.Generic;
using Configs;
using CoreGameplay.Models;
using CoreGameplay.Views;
using UnityEngine;

namespace CoreGameplay.Controllers
{
    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance { get; private set; }

        [SerializeField] private PlayerController PlayerPrefab;
        [SerializeField] private PlayerPathController PlayerPathPrefab;
        [SerializeField] private TargetView ExitPrefab;
        [SerializeField] private GameConfig GameConfig;
        [SerializeField] private float PaddingValue;

        [SerializeField] private Transform PlayerSpawnPosition;
        [SerializeField] private Transform ExitSpawnPosition;

        public Transform LevelContainer;

        private CoreGameplayController _coreGameplayController;
        private PlayerController _player;
        private PlayerPathController _pathController;
        private TargetView _exit;

        private List<BaseLevelObject> _levelObjects = new();
        private Vector3 _pathPosition;
        private List<ExplosionController> _explosionsList = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new System.Exception("An instance of LevelController already exists");
            }

            Instance = this;
            _levelObjects = new List<BaseLevelObject>();
            _explosionsList = new List<ExplosionController>();

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _coreGameplayController = new CoreGameplayController(GameConfig);
            LoadLevel();
        }

        public void LoadLevel()
        {
            var levelNumber = AppController.Instance.GameStateModel.LevelNumber;

            var playerSpawnPosition = PlayerSpawnPosition.position;
            _player = Instantiate(PlayerPrefab, playerSpawnPosition, Quaternion.identity);
            var exitSpawnPosition = ExitSpawnPosition.position;
            _exit = Instantiate(ExitPrefab, exitSpawnPosition, Quaternion.identity);

            _pathPosition = (playerSpawnPosition + exitSpawnPosition) / 2f;
            _pathController = Instantiate(PlayerPathPrefab, _pathPosition, Quaternion.identity);

            _player.OnCurrentSizeChangedEvent += _pathController.OnCurrentSizeChanged;
            _pathController.OnPathFreeAction += _coreGameplayController.OnLevelCompleted;
            _pathController.Clear();

            var coreGameplayModel = new CoreGameplayModel
            {
                Player = _player,
                PathController = _pathController,
                Exit = _exit
            };

            _coreGameplayController.OnLoadLevel(coreGameplayModel);
            GenerateLevel(levelNumber);
        }

        private void GenerateLevel(int levelNumber)
        {
            var levelConfig = GetLevelConfigByNumber(levelNumber);

            if (levelConfig == null) return;

            var segmentsCount = levelConfig.Segments.Length;
            var pathLength = (PlayerSpawnPosition.position - ExitSpawnPosition.position).magnitude - PaddingValue;
            var segmentLength = segmentsCount > 0 ? pathLength / levelConfig.Segments.Length : pathLength;

            for (var segmentId = 0; segmentId < segmentsCount; segmentId++)
            {
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
            var sizeX = segmentConfig.BaseLevelObject.Size.x;
            var sizeZ = segmentConfig.BaseLevelObject.Size.z;
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
                    var levelObject = ObjectsPoolController.GetLevelObject(segmentConfig.BaseLevelObject,
                        levelObjectPosition,
                        Quaternion.identity);
                    //var baseLevelObject = Instantiate(segmentConfig.BaseLevelObject, LevelContainer);
                    //baseLevelObject.transform.position = levelObjectPosition;
                    _levelObjects.Add(levelObject);
                }
            }
        }

        public void UnloadLevel()
        {
            _player.OnCurrentSizeChangedEvent -= _pathController.OnCurrentSizeChanged;
            _pathController.OnPathFreeAction -= _coreGameplayController.OnLevelCompleted;
            _coreGameplayController.OnUnloadLevel();

            foreach (var levelObject in _levelObjects)
            {
                ObjectsPoolController.ReturnLevelObjectToPool(levelObject);
            }

            foreach (var explosion in _explosionsList)
            {
                Destroy(explosion.gameObject);
            }

            _levelObjects.Clear();

            if (_player == null) return;

            Destroy(_player.gameObject);
            Destroy(_pathController.gameObject);
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

        public void OnRemoveObstacle(BaseLevelObject baseLevelObject)
        {
            _coreGameplayController.OnRemoveObstacle(baseLevelObject);
        }

        public void AddExplosion(ExplosionController explosion)
        {
            _explosionsList.Add(explosion);
        }

        public void RemoveExplosion(ExplosionController explosion)
        {
            _explosionsList.Remove(explosion);
        }
    }
}