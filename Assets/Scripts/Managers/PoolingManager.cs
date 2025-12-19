using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;

    // prefab GameObject -> pooled instances
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    // instance -> prefab
    private readonly Dictionary<GameObject, GameObject> instanceToPrefab = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // -------- SPAWN --------
    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation)
        where T : Component
    {
        GameObject prefabGO = prefab.gameObject;

        if (!pools.TryGetValue(prefabGO, out var pool))
        {
            pool = new Queue<GameObject>();
            pools[prefabGO] = pool;
        }

        GameObject instanceGO;

        if (pool.Count > 0)
        {
            instanceGO = pool.Dequeue();
        }
        else
        {
            instanceGO = Instantiate(prefabGO);
            instanceToPrefab[instanceGO] = prefabGO;
        }

        instanceGO.transform.SetPositionAndRotation(position, rotation);
        instanceGO.SetActive(true);

        return instanceGO.GetComponent<T>();
    }

    // -------- DESPAWN --------
    public void Despawn<T>(T instance) where T : Component
    {
        GameObject instanceGO = instance.gameObject;

        if (!instanceToPrefab.TryGetValue(instanceGO, out var prefabGO))
        {
            Destroy(instanceGO);
            return;
        }

        instanceGO.SetActive(false);

        if (!pools.TryGetValue(prefabGO, out var pool))
        {
            pool = new Queue<GameObject>();
            pools[prefabGO] = pool;
        }

        pool.Enqueue(instanceGO);
    }
}
