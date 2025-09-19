using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SlimeBulletShooterWeapon : BaseWeapon
{
    [SerializeField] private List<SlimeBulletShooterLevelDataSO> levelDatas;

    private float timer;

    [Header("Refs")]
    [SerializeField] GameObject player;
    private FrenzySkill frenzySkill;
    private AbilityHaste abilityHaste;
    private GenericDamageModifier genericDamageModifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Cant found player in SlimeBulletShooterWeapon");
            return;
        }

        frenzySkill = player.GetComponent<FrenzySkill>();
        abilityHaste = player.GetComponent<AbilityHaste>();
        genericDamageModifier = player.GetComponent<GenericDamageModifier>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
            return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        SlimeBulletShooterLevelDataSO levelData = null;
        if (currentLevel < levelDatas.Count)
        {
            levelData = levelDatas[currentLevel - 1];

            int baseDamage = levelData.damage;
            int finalDamage = (int)(baseDamage * (1 + genericDamageModifier.GetValue() + frenzySkill.GetFrenzyBonusPercent()));

            float baseCooldownTime = levelData.cooldown;
            float finalCooldownTime = abilityHaste.GetCooldownTimeAfterReduction(baseCooldownTime);

            int bulletCount = levelData.bulletCount;
            float minimumDistance = levelData.minimumDistance;
            float minDistBetweenBullets = levelData.minimumDistanceBetweenBullets;
            float maxDistBetweenBullets = levelData.maximumDistanceBetweenBullets;
            float passthroughDamageModifier = levelData.passthroughDamageModifier;
            float moveSpeed = levelData.moveSpeed;
            float existDuration = levelData.existDuration;
            float slowModifier = levelData.slowModifier;
            float slowRadius = levelData.slowRadius;
            float delayBetweenBullet = levelData.delayBetweenBullet;

            StartCoroutine(Shoot(finalDamage, finalCooldownTime, bulletCount,
                minimumDistance, minDistBetweenBullets, maxDistBetweenBullets,
                passthroughDamageModifier, moveSpeed, existDuration,
                slowModifier, slowRadius, delayBetweenBullet, finalCooldownTime));
        }
    }

    IEnumerator Shoot(
    int damage,
    float cooldown,
    int bulletCount,
    float minimumDistance,
    float minDistBetweenBullets,
    float maxDistBetweenBullets,
    float passthroughDamageModifier,
    float moveSpeed,
    float existDuration,
    float slowModifier,
    float slowRadius,
    float delayBetweenBullet,
    float finalCooldownTime)
    {
        // Precompute step size
        float bonusDistance = (maxDistBetweenBullets - minDistBetweenBullets) / Mathf.Max(1, bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            // Spawn the bullet
            SlimeBullet bullet = ProjectilesManager.Instance.TakeSlimeBullet();

            float distance = minimumDistance + i * bonusDistance;

            SetBulletStats(bullet, damage, passthroughDamageModifier, cooldown,
                distance, moveSpeed, existDuration, slowModifier, slowRadius);

            // Wait before spawning the next bullet
            if (delayBetweenBullet > 0f && i < bulletCount - 1)
                yield return new WaitForSeconds(delayBetweenBullet);
        }

        timer = finalCooldownTime; // Reset timer
    }

    private void SetBulletStats(SlimeBullet bullet, int damage, float passthroughDamageModifier,
        float cooldown, float maxDistance, float moveSpeed, float existDuration, float slowModifier,
        float slowRadius)
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mouseWorldPosition = MapManager.GetMouseWorldPosition();
        Vector2 moveDirection = math.normalize(mouseWorldPosition - playerPosition);

        bullet.transform.position = playerPosition;
        bullet.Initialize(moveDirection, moveSpeed, maxDistance, damage, passthroughDamageModifier, 0, existDuration, slowModifier, slowRadius);
    }

    protected override void Initialize()
    {
        currentLevel = 1;
        timer = 0;
    }
}
