using System;
using System.Linq;
using CoreGameplay.Components;
using CoreGameplay.Views;
using UI;
using UnityEngine;

namespace CoreGameplay.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public event Action<float> OnCurrentSizeChangedEvent;

        [SerializeField] private PlayerProjectileView ProjectilePrefab;
        [SerializeField] private Transform ProjectilesSpawnPosition;
        [SerializeField] private Vector3 ProjectilesDirection;
        [SerializeField] private float GrownProjectileSpeed;
        [SerializeField] private float StartSize;
        [SerializeField] private float MinSize;
        [SerializeField] private int ProjectilePoolSize;
        [SerializeField] private Animator PlayerAnimator;
        [SerializeField] private ChargingComponent ChargingComponent;

        private PlayerProjectileView[] _projectilesPool;

        private PlayerProjectileView _currentProjectile;

        private Quaternion _projectileRotation;
        private bool _isTapped;

        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            InitProjectilesPool();

            _projectileRotation = Quaternion.LookRotation(ProjectilesDirection);
            ChargingComponent.CurrentCharge = StartSize;
        }

        private void OnEnable()
        {
            ChargingComponent.OnCurrentSizeChangedEvent -= OnCurrentSizeChanged;
            ChargingComponent.OnCurrentSizeChangedEvent += OnCurrentSizeChanged;
        }

        private void OnDisable()
        {
            ChargingComponent.OnCurrentSizeChangedEvent -= OnCurrentSizeChanged;
        }

        private void OnCurrentSizeChanged(float value)
        {
            OnCurrentSizeChangedEvent?.Invoke(value);
        }

        public void StartMoveAnimation(float speed)
        {
            PlayerAnimator.SetBool(IsMoving, true);
            PlayerAnimator.SetFloat(MoveSpeed, speed);
        }

        public void EndMoveAnimation()
        {
            PlayerAnimator.SetBool(IsMoving, false);
        }

        private void InitProjectilesPool()
        {
            if (_projectilesPool != null && _projectilesPool.Length >= ProjectilePoolSize)
                return;

            _projectilesPool = new PlayerProjectileView[ProjectilePoolSize];

            for (var i = 0; i < ProjectilePoolSize; i++)
            {
                _projectilesPool[i] = Instantiate(ProjectilePrefab, LevelController.Instance.LevelContainer);
                _projectilesPool[i].Hide();
            }
        }

        private PlayerProjectileView GetProjectileFromPool()
        {
            foreach (var projectile in _projectilesPool)
                if (!projectile.gameObject.activeSelf)
                    return projectile;

            return _projectilesPool.First();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) _isTapped = true;

            if (Input.GetMouseButtonUp(0))
            {
                _isTapped = false;

                OnFinishTap();
            }

            if (_isTapped && _currentProjectile == null) CreateProjectile();

            var value = GrownProjectileSpeed * Time.deltaTime;
            if (_isTapped && _currentProjectile != null && ChargingComponent.CurrentCharge >= value)
            {
                _currentProjectile.AddCharge(value);
                ChargingComponent.CurrentCharge -= value;
            }
            else if (_isTapped && _currentProjectile != null)
            {
                OnFinishTap();
            }

            if (ChargingComponent.CurrentCharge <= MinSize)
            {
                LevelController.Instance.UnloadLevel();
                ScreensManager.ChangeScreen(ScreensType.FailLevelScreen);
                _isTapped = false;
            }
        }

        private void CreateProjectile()
        {
            _currentProjectile = GetProjectileFromPool();
            var currentProjectileTransform = _currentProjectile.transform;
            currentProjectileTransform.position = ProjectilesSpawnPosition.position;
            currentProjectileTransform.rotation = _projectileRotation;
            _currentProjectile.Show();
        }

        private void OnFinishTap()
        {
            ShootCurrentProjectile();
        }

        private void ShootCurrentProjectile()
        {
            _currentProjectile.Shoot(ProjectilesDirection);
            _currentProjectile = null;
        }
    }
}