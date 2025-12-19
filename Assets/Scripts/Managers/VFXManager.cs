using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    private static VFXManager _instance;
    public static VFXManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<VFXManager>();
            return _instance;
        }
    }

    // Active effects tracked per prefab
    private readonly Dictionary<BaseVFX, HashSet<BaseVFX>> activeVFXs = new();

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ---------------- SPAWN ----------------

    public BaseVFX SpawnVFX(
        BaseVFX prefab,
        Vector3 position,
        Quaternion rotation,
        Vector3 scale
    )
    {
        if (prefab == null)
            return null;

        BaseVFX vfx =
            PoolingManager.Instance.Spawn(prefab, position, rotation);

        vfx.transform.localScale = scale;

        RegisterActiveEffect(prefab, vfx);
        return vfx;
    }

    // ---------------- RETURN ----------------

    public void ReturnEffect(BaseVFX effect)
    {
        if (effect == null)
            return;

        UnregisterActiveEffect(effect);
        PoolingManager.Instance.Despawn(effect);
    }

    // ---------------- TRACKING ----------------

    private void RegisterActiveEffect(BaseVFX prefab, BaseVFX instance)
    {
        if (!activeVFXs.TryGetValue(prefab, out var set))
        {
            set = new HashSet<BaseVFX>();
            activeVFXs[prefab] = set;
        }

        set.Add(instance);
    }

    private void UnregisterActiveEffect(BaseVFX instance)
    {
        foreach (var kvp in activeVFXs)
        {
            if (kvp.Value.Remove(instance))
                return;
        }
    }

    // ---------------- SYSTEM ----------------

    public void Initialize()
    {
        ClearAllEffects();
        GameInitializationManager.Instance.animationsPrepared = true;
    }

    private void ClearAllEffects()
    {
        foreach (var kvp in activeVFXs)
        {
            foreach (var effect in kvp.Value)
                PoolingManager.Instance.Despawn(effect);

            kvp.Value.Clear();
        }

        activeVFXs.Clear();
    }
}
