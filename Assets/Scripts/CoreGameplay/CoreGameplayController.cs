using Configs;
using CoreGameplay.Models;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UI;
using UnityEngine;

namespace CoreGameplay
{
    public class CoreGameplayController
    {
        //TODO: remove
        public static CoreGameplayController Instance;

        private CoreGameplayModel _model;
        private TweenerCore<Vector3, Vector3, VectorOptions> _playerMoveTween;

        public CoreGameplayController(GameConfig gameConfig)
        {
            _gameConfig = gameConfig;

            Instance = this;
        }

        private GameConfig _gameConfig;

        public void OnRemoveObstacle(ObstacleView obstacle)
        {
            _model.PathView.OnRemoveObstacle(obstacle);
        }

        //TODO: move LoadLevel and UnloadLevel to levels manager
        //TODO: move it to game model
        public void OnLoadLevel(CoreGameplayModel model)
        {
            _model = model;

            _model.Player.OnCurrentSizeChangedEvent += _model.PathView.OnCurrentSizeChanged;
            _model.PathView.OnPathFreeAction += OnLevelCompleted;
            _model.PathView.Clear();
        }

        public void OnUnloadLevel()
        {
            _model.Player.OnCurrentSizeChangedEvent -= _model.PathView.OnCurrentSizeChanged;
            _model.PathView.OnPathFreeAction -= OnLevelCompleted;
            _playerMoveTween?.Kill();
        }

        public void OnLevelCompleted()
        {
            Debug.Log($"OnLevelCompleted");

            var distanceToTarget = Vector3.Distance(_model.Player.transform.position, _model.Exit.transform.position);
            var delay = distanceToTarget / _gameConfig.PlayerMoveSpeed;
            _model.Player.StartMoveAnimation(_gameConfig.PlayerMoveSpeed);
            _playerMoveTween = _model.Player.transform
                .DOMove(_model.Exit.transform.position - Vector3.forward * 0.5f, delay)
                .OnComplete(OnWinAnimationCompleted);
        }

        private void OnWinAnimationCompleted()
        {
            _model.Player.EndMoveAnimation();

            ScreensManager.ChangeScreen(ScreensType.WinLevelScreen);
        }
    }
}