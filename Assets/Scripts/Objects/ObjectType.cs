using UnityEngine;

public enum InGameObjectType
{
    Unknown,
    Player,
    Enemy,
    RadiantField,
    SlimeBullet,
    FrenzySkill,
    PawPrintPoisonCloud,
    SlimeBeam,
    ExperienceOrb,
    EnemyBullet,
    EnemyPoisonCloud,
}

public class ObjectType : MonoBehaviour
{
    public InGameObjectType InGameObjectType;
}
