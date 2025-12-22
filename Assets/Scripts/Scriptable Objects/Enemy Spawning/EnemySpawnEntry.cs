using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class EnemySpawnEntry
{
    public BaseEnemy prefab;

    [Range(0f, 1f)]
    public float weight = 1f;
}
