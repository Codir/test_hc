using System;
using System.Linq;
using CoreGameplay;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action<float> OnCurrentSizeChangedEvent;

    [SerializeField] private PlayerProjectileView ProjectilePrefab;

    //[SerializeField] private Transform ProjectilesContainer;
    [SerializeField] private Transform ProjectilesSpawnPosition;
    [SerializeField] private Transform Body;
    [SerializeField] private Vector3 ProjectilesFlyDirection;
    [SerializeField] private float GrownProjectileSpeed;
    [SerializeField] private float StartSize;
    [SerializeField] private int ProjectilePoolSize;
    [SerializeField] private Animator PlayerAnimator;

    private PlayerProjectileView[] _projectiles;

    private PlayerProjectileView _currentProjectile;

    //TODO: change name of variable
    private float CurrentSize
    {
        get => _currentSize;
        set
        {
            _currentSize = value;
            OnCurrentSizeChanged();
        }
    }

    private float _currentSize;
    private bool _isTapped;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        InitProjectilesPool();

        //TODO: add load start value from level config
        CurrentSize = StartSize;
    }

    public void StartMoveAnimation(float speed)
    {
        //TODO: move to player view or controller of animator
        PlayerAnimator.SetBool("IsMoving", true);
        PlayerAnimator.SetFloat("MoveSpeed", speed);
    }

    public void EndMoveAnimation()
    {
        //TODO: move to player view or controller of animator
        PlayerAnimator.SetBool("IsMoving", false);
    }

    //TODO: Move to pooling extension
    private void InitProjectilesPool()
    {
        if (_projectiles != null && _projectiles.Length >= ProjectilePoolSize)
            return;

        _projectiles = new PlayerProjectileView[ProjectilePoolSize];

        var projectileRotation = Quaternion.LookRotation(ProjectilesFlyDirection);
        for (var i = 0; i < ProjectilePoolSize; i++)
        {
            _projectiles[i] = Instantiate(ProjectilePrefab, ProjectilesSpawnPosition.position, projectileRotation);
            _projectiles[i].Hide();
        }
    }

    private PlayerProjectileView GetProjectile()
    {
        foreach (var projectile in _projectiles)
        {
            //TODO: add checking ready in projectile controller
            if (!projectile.gameObject.activeSelf)
                return projectile;
        }

        return _projectiles.First();
    }
    //

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //TODO: add vibration feedback on
            _isTapped = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //TODO: add vibration feedback off
            _isTapped = false;

            OnFinishTap();
        }

        if (_isTapped && _currentProjectile == null)
        {
            _currentProjectile = GetProjectile();
            _currentProjectile.Show(ProjectilesSpawnPosition);
        }

        var value = GrownProjectileSpeed * Time.deltaTime;
        //Debug.Log($"_isTapped {_isTapped} _currentProjectile {_currentProjectile} CurrentSize {CurrentSize} value {value}");
        if (_isTapped && _currentProjectile != null && CurrentSize >= value)
        {
            _currentProjectile.GrownProjectile(value);
            CurrentSize -= value;
        }
        else if (_isTapped && _currentProjectile != null)
        {
            OnFinishTap();
        }
    }

    private void OnFinishTap()
    {
        ShootCurrentProjectile();
    }

    private void ShootCurrentProjectile()
    {
        _currentProjectile.Shoot(ProjectilesFlyDirection);
        _currentProjectile = null;
    }

    //TODO: move to separate class working with CurrentSize
    private void OnCurrentSizeChanged()
    {
        //Debug.Log($"OnCurrentSizeChanged CurrentSize {CurrentSize}");
        Body.localScale = Vector3.one * CurrentSize;
        OnCurrentSizeChangedEvent?.Invoke(CurrentSize);
    }
}