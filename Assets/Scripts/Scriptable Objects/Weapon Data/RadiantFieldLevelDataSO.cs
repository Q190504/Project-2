using UnityEngine;

[CreateAssetMenu(fileName = "Radiant Field Level ", menuName = "Scriptable Objects/Weapon Data/Radiant Field Level Data")]

public class RadiantFieldLevelDataSO : ScriptableObject
{
    public int damagePerTick;
    public float cooldown;
    public float radius;
    public float slowModifier;
}
