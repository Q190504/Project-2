using System.Collections.Generic;
using UnityEngine;

public class ProjectilesManager : MonoBehaviour
{
    public static ProjectilesManager Instance;

    private readonly HashSet<BaseProjectile> activeProjectiles = new();

    // Slime Bullets waiting to be reclaimed
    private readonly List<SlimeBullet> slimeBulletsToReclaim = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // -------- SPAWN --------
    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation)
        where T : BaseProjectile
    {
        T projectile = PoolingManager.Instance.Spawn(prefab, position, rotation);
        activeProjectiles.Add(projectile);
        return projectile;
    }

    // -------- RETURN --------
    public void Return(BaseProjectile projectile)
    {
        if (!activeProjectiles.Remove(projectile))
            return;

        PoolingManager.Instance.Despawn(projectile);
    }

    // -------- CLEANUP --------
    public void Initialize()
    {
        foreach (var proj in activeProjectiles)
            PoolingManager.Instance.Despawn(proj);

        activeProjectiles.Clear();
        GameInitializationManager.Instance.hasCleanProjectiles = true;
    }

    // ---------------- SLIME BULLET RECLAIM ----------------

    /// <summary>
    /// Mark Slime Bullet to be reclaimed later
    /// </summary>
    public void RegisterSlimeBulletToReclaim(SlimeBullet slimeBullet)
    {
        if (activeProjectiles.Contains(slimeBullet))
            slimeBulletsToReclaim.Add(slimeBullet);
    }

    /// <summary>
    /// Remove Slime Bullet from reclaim queue
    /// </summary>
    public void UnregisterSlimeBulletToReclaim(SlimeBullet slimeBullet)
    {
        slimeBulletsToReclaim.Remove(slimeBullet);
    }

    /// <summary>
    /// Are there Slime Bullet waiting to be reclaimed?
    /// </summary>
    public bool HasSlimeBulletsToReclaim()
    {
        return slimeBulletsToReclaim.Count > 0;
    }

    public List<SlimeBullet> GetSlimeBulletsWaitToReclaim()
    {
        return slimeBulletsToReclaim;
    }

    /// <summary>
    /// Snapshot for safe iteration
    /// </summary>
    public IReadOnlyCollection<SlimeBullet> GetSlimeBulletsToReclaim()
    {
        return slimeBulletsToReclaim;
    }
}
