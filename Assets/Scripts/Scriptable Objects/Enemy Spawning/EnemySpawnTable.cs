using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnTable", menuName = "Scriptable Objects/Spawning/Enemy Spawn Table")]
public class EnemySpawnTable : ScriptableObject
{
    public List<EnemySpawnDuration> durations = new();

    public BaseEnemy GetRandomEnemy(float minutes)
    {
        float totalWeight = 0f;

        // First pass: sum weights from ALL active durations
        foreach (var duration in durations)
        {
            if (minutes < duration.startMinute ||
                minutes > duration.endMinute)
                continue;

            foreach (var enemy in duration.enemies)
                totalWeight += enemy.weight;
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.value * totalWeight;

        // Second pass: pick enemy
        foreach (var duration in durations)
        {
            if (minutes < duration.startMinute ||
                minutes > duration.endMinute)
                continue;

            foreach (var enemy in duration.enemies)
            {
                roll -= enemy.weight;
                if (roll <= 0f)
                    return enemy.prefab;
            }
        }

        return null;
    }
}
