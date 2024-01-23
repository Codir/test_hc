using System;
using System.Collections.Generic;
using System.Linq;
using CoreGameplay;
using UnityEngine;

[Serializable]
public class ObjectsPoolConfig
{
    public LevelObjectView Prefab;
    public int Count;
}

public class ObjectsPoolController : MonoBehaviour
{
    [SerializeField] private Transform LevelContainer;
    [SerializeField] private ObjectsPoolConfig[] ObjectsPoolConfigs;

    private static ObjectsPoolController _instance;

    public static ObjectsPoolController Instance => _instance;

    private static Dictionary<string, LevelObjectView[]> _objectsPools;
    private static Dictionary<string, Transform> _objectsContainers;

    private void Awake()
    {
        //TODO: remove this
        if (_instance == null)
        {
            _instance = this;
        }

        AppController.Instance.ObjectsPoolController = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _objectsPools = new Dictionary<string, LevelObjectView[]>();
        _objectsContainers = new Dictionary<string, Transform>();

        foreach (var objectsPoolConfig in ObjectsPoolConfigs)
        {
            CreateObjectsPool(objectsPoolConfig);
        }
    }

    private void CreateObjectsPool(ObjectsPoolConfig objectsPoolConfig)
    {
        var objectName = objectsPoolConfig.Prefab.Name;
        var parent = GetParent(objectsPoolConfig.Prefab.Name);

        var levelObjects = new LevelObjectView[objectsPoolConfig.Count];
        for (var i = 0; i < objectsPoolConfig.Count; i++)
        {
            levelObjects[i] = Instantiate(objectsPoolConfig.Prefab, parent.transform);
            levelObjects[i].gameObject.SetActive(false);
        }

        _objectsPools.Add(objectName, levelObjects);
    }

    public static void CreateObjectsPool<T>(T prefab, int count)
        where T : LevelObjectView
    {
        var key = typeof(T).ToString();
        if (_objectsPools.ContainsKey(key))
        {
            ClearObjectsPool<T>();
        }

        var objectsPool = new LevelObjectView[count];
        var parent = GetParent(prefab.Name);

        for (var i = 0; i < count; i++)
        {
            objectsPool[i] = Instantiate(prefab, parent);
            objectsPool[i].gameObject.SetActive(false);
        }

        _objectsPools.Add(key, objectsPool);
    }

    public static void ClearAllObjectsPool()
    {
        if (_objectsPools.Count <= 0) return;

        foreach (var (key, value) in _objectsPools)
        {
            foreach (var levelObject in value)
            {
                Destroy(levelObject.gameObject);
            }

            Destroy(_objectsContainers[key].gameObject);
        }

        _objectsPools = new Dictionary<string, LevelObjectView[]>();
        _objectsContainers = new Dictionary<string, Transform>();
    }

    public static void ClearObjectsPool<T>()
        where T : LevelObjectView
    {
        var key = typeof(T).ToString();
        if (!_objectsPools.ContainsKey(key)) return;

        foreach (var levelObject in _objectsPools[key])
        {
            Destroy(levelObject.gameObject);
        }

        _objectsPools.Remove(key);

        Destroy(_objectsContainers[key].gameObject);
        _objectsContainers.Remove(key);
    }

    public static T GetLevelObject<T>(T prefab, Vector3 position, Quaternion rotation)
        where T : LevelObjectView
    {
        var key = typeof(T).ToString();
        var parent = GetParent(prefab.Name);
        var levelObject = !_objectsPools.ContainsKey(key)
            ? GetLevelObjectFromPool<T>(prefab)
            : Instantiate(prefab, parent);

        var gameObject = levelObject.gameObject;
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.SetActive(true);

        return levelObject;
    }

    private static Transform GetParent(string name)
    {
        if (_objectsContainers.ContainsKey(name)) return _objectsContainers[name];

        var parent = new GameObject(name).transform;
        parent.SetParent(_instance.LevelContainer);
        _objectsContainers.Add(name, parent);

        return _objectsContainers[name];
    }

    private static T GetLevelObjectFromPool<T>(LevelObjectView prefab)
        where T : LevelObjectView
    {
        var objectsPool = _objectsPools[prefab.Name];

        foreach (var levelObject in objectsPool)
        {
            if (!levelObject.gameObject.activeSelf)
            {
                return levelObject as T;
            }
        }

        return objectsPool.First() as T;
    }

    public static void ReturnLevelObjectToPool(LevelObjectView levelObject)
    {
        var go = levelObject.gameObject;
        go.SetActive(false);
    }
}