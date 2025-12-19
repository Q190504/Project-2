using UnityEngine;

[CreateAssetMenu(fileName = "Paw Print Poisoner Level ", menuName = "Scriptable Objects/Weapon Data/Paw Print Poisoner Level Data")]
public class PawPrintPoisonerLevelDataSO : ScriptableObject
{
    public int damagePerTick;
    public float cloudRadius;
    public float maximumCloudDuration;
    public float bonusMoveSpeedPerTargetInTheCloudModifier;
}
