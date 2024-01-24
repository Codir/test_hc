using Configs;
using CoreGameplay.Models;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UI;
using UnityEngine;

namespace CoreGameplay.Controllers
{
    public class CoreGameplayController
    {
        private CoreGameplayModel _model;
        private TweenerCore<Vector3, Vector3, VectorOptions> _playerMoveTween;
        private readonly GameConfig _gameConfig;

        public CoreGameplayController(GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
        }

        public void OnRemoveObstacle(BaseLevelObject obstacle)
        {
            _model.PathController.OnRemoveObstacle(obstacle);
        }

        public void OnLoadLevel(CoreGameplayModel model)
        {
            _model = model;

            _model.Player.OnCurrentSizeChangedEvent += _model.PathController.OnCurrentSizeChanged;
            _model.PathController.OnPathFreeAction += OnLevelCompleted;
            _model.PathController.Clear();
        }

        public void OnUnloadLevel()
        {
            _model.Player.OnCurrentSizeChangedEvent -= _model.PathController.OnCurrentSizeChanged;
            _model.PathController.OnPathFreeAction -= OnLevelCompleted;
            _playerMoveTween?.Kill();
        }

        public void OnLevelCompleted()
        {
            var position = _model.Exit.transform.position;
            var distanceToTarget = Vector3.Distance(_model.Player.transform.position, position);
            var delay = distanceToTarget / _gameConfig.PlayerMoveSpeed;
            _model.Player.StartMoveAnimation(_gameConfig.PlayerMoveSpeed);
            _playerMoveTween = _model.Player.transform
                .DOMove(position - Vector3.forward * 0.5f, delay)
                .OnComplete(OnWinAnimationCompleted);
        }

        private void OnWinAnimationCompleted()
        {
            _model.Player.EndMoveAnimation();

            LevelController.Instance.UnloadLevel();
            ScreensManager.ChangeScreen(ScreensType.WinLevelScreen);
        }
    }
}