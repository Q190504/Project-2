using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnDuration
{
    [Header("Time Window (Minutes)")]
    public float startMinute;
    public float endMinute;

    [Header("Enemies In This Duration")]
    public List<EnemySpawnEntry> enemies = new();
}
