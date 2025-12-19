using UnityEngine;

[CreateAssetMenu(fileName = "Slime Bullet Shooter Level ", menuName = "Scriptable Objects/Weapon Data/Slime Bullet Shooter Level Data")]
public class SlimeBulletShooterLevelDataSO : ScriptableObject
{
    public int damage;
    public float cooldown;
    public int bulletCount;
    public float delayBetweenBullet;
    public float minimumDistance;
    public float minimumDistanceBetweenBullets;
    public float maximumDistanceBetweenBullets;
    public float passthroughDamageModifier;
    public float moveSpeed;
    public float existDuration;
    public float slowModifier;
    public float slowRadius;
}

