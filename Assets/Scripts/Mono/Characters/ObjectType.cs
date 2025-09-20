using UnityEngine;

public enum InGameObjectType
{
    Unknown,
    Player,
    Creep,
    RadiantField,
    SlimeBullet,
    FrenzySkill,
    PoisonCloud,
    SlimeBeam,
    ExperienceOrb,
}

public class ObjectType : MonoBehaviour
{
    public InGameObjectType InGameObjectType;
}
