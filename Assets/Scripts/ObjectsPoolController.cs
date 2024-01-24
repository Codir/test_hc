using System;
using System.Collections.Generic;
using System.Linq;
using CoreGameplay.Controllers;
using UnityEngine;

[Serializable]
public class ObjectsPoolConfig
{
    public BaseLevelObject Prefab;
    public int Count;
}

public class ObjectsPoolController : MonoBehaviour
{
    [SerializeField] private Transform LevelContainer;
    [SerializeField] private ObjectsPoolConfig[] ObjectsPoolConfigs;

    private static Transform _levelContainer;

    private static Dictionary<string, BaseLevelObject[]> _objectsPools;
    private static Dictionary<string, Transform> _objectsContainers;

    private void Awake()
    {
        _levelContainer = LevelContainer;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _objectsPools = new Dictionary<string, BaseLevelObject[]>();
        _objectsContainers = new Dictionary<string, Transform>();

        foreach (var objectsPoolConfig in ObjectsPoolConfigs) CreateObjectsPool(objectsPoolConfig);
    }

    private void CreateObjectsPool(ObjectsPoolConfig objectsPoolConfig)
    {
        var objectName = objectsPoolConfig.Prefab.Name;
        var parent = GetParent(objectsPoolConfig.Prefab.Name);

        var levelObjects = new BaseLevelObject[objectsPoolConfig.Count];
        for (var i = 0; i < objectsPoolConfig.Count; i++)
        {
            levelObjects[i] = Instantiate(objectsPoolConfig.Prefab, parent.transform);
            levelObjects[i].gameObject.SetActive(false);
        }

        _objectsPools.Add(objectName, levelObjects);
    }

    public static void CreateObjectsPool<T>(T prefab, int count)
        where T : BaseLevelObject
    {
        var key = typeof(T).ToString();
        if (_objectsPools.ContainsKey(key)) ClearObjectsPool<T>();

        var objectsPool = new BaseLevelObject[count];
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
            foreach (var levelObject in value) Destroy(levelObject.gameObject);

            Destroy(_objectsContainers[key].gameObject);
        }

        _objectsPools = new Dictionary<string, BaseLevelObject[]>();
        _objectsContainers = new Dictionary<string, Transform>();
    }

    public static void ClearObjectsPool<T>()
        where T : BaseLevelObject
    {
        var key = typeof(T).ToString();
        if (!_objectsPools.ContainsKey(key)) return;

        foreach (var levelObject in _objectsPools[key]) Destroy(levelObject.gameObject);

        _objectsPools.Remove(key);

        Destroy(_objectsContainers[key].gameObject);
        _objectsContainers.Remove(key);
    }

    public static T GetLevelObject<T>(T prefab, Vector3 position, Quaternion rotation)
        where T : BaseLevelObject
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
        parent.SetParent(_levelContainer);
        _objectsContainers.Add(name, parent);

        return _objectsContainers[name];
    }

    private static T GetLevelObjectFromPool<T>(BaseLevelObject prefab)
        where T : BaseLevelObject
    {
        var objectsPool = _objectsPools[prefab.Name];

        foreach (var levelObject in objectsPool)
            if (!levelObject.gameObject.activeSelf)
                return levelObject as T;

        return objectsPool.First() as T;
    }

    public static void ReturnLevelObjectToPool(BaseLevelObject baseLevelObject)
    {
        var go = baseLevelObject.gameObject;
        go.SetActive(false);
    }
}