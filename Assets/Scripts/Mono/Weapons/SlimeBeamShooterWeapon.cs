using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SlimeBeamShooterWeapon : BaseWeapon
{
    [Header("Slime Beam Shooter Settings")]
    [SerializeField] private List<SlimeBeamShooterLevelDataSO> levelDatas;
    [SerializeField] int beamCount;
    [SerializeField] float timeBetween;
    [SerializeField] float spawnOffsetPositon;

    private float timer;
    private bool isShooting;

    [Header("Refs")]
    [SerializeField] private GameObject player;

    private AbilityHaste abilityHaste;
    private GenericDamageModifier genericDamageModifier;
    private FrenzySkill frenzySkill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Cant find player in SlimeBeamShooterWeapon");
            return;
        }

        abilityHaste = player.GetComponent<AbilityHaste>();
        genericDamageModifier = player.GetComponent<GenericDamageModifier>();
        frenzySkill = player.GetComponent<FrenzySkill>();

        isInitialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying() || !IsActive || isShooting)
            return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        SlimeBeamShooterLevelDataSO levelData = GetCurrentLevelData();

        int baseDamage = levelData.damage;
        int finalDamage = (int)(baseDamage * (1 + genericDamageModifier.GetValue()
            + frenzySkill.GetFrenzyBonusPercent()));

        float finalCooldownTime = abilityHaste.GetCooldownTimeAfterReduction(levelData.cooldown);

        if (currentLevel < 5) // Shoot all beams with a delay between 2 beams
            StartCoroutine(ShootBeam(spawnOffsetPositon, finalDamage, beamCount, timeBetween,
                finalCooldownTime));
        else // Shoot all beams at once
            StartCoroutine(ShootBeam(spawnOffsetPositon, finalDamage, beamCount, 0,
                finalCooldownTime));
    }

    IEnumerator ShootBeam(float spawnOffsetPositon, int damage, int beamCount, float timeBetweenBeams,
        float finalCooldownTime)
    {
        isShooting = true;
        for (int i = 0; i < beamCount; i++)
        {
            Vector2 playerPosition = player.transform.position;
            Vector2 position = playerPosition + GetAttackDirection(spawnOffsetPositon, i);
            Quaternion rotation = GetRotation(i);

            //spawn beam
            SlimeBeam slimeBeamInstance = ProjectilesManager.Instance.TakeSlimeBeam();
            SetStats(slimeBeamInstance, damage, position, rotation);

            if (i < beamCount - 1 && timeBetweenBeams > 0)
                yield return new WaitForSeconds(timeBetweenBeams);
        }

        timer = finalCooldownTime; // Reset timer
        isShooting = false;
    }

    public override void Initialize()
    {
        timer = 0f;
        currentLevel = 0;
        isInitialized = true;
        isShooting = false;
    }

    private SlimeBeamShooterLevelDataSO GetCurrentLevelData()
    {
        return levelDatas[math.min(currentLevel - 1, levelDatas.Count - 1)];
    }

    private void SetStats(SlimeBeam beam, int damage, Vector2 originPosition, quaternion rotation)
    {
        beam.transform.position = originPosition;
        beam.transform.rotation = rotation;

        beam.Initialize(damage);
    }

    private Vector2 GetAttackDirection(float spawnOffsetPositon, int count)
    {
        return (count % 4) switch
        {
            0 => new Vector2(0, spawnOffsetPositon),// Top 
            1 => new Vector2(spawnOffsetPositon, 0),// Right 
            2 => new Vector2(0, -spawnOffsetPositon),// Bottom
            3 => new Vector2(-spawnOffsetPositon, 0),// Left 
            _ => Vector2.zero,
        };
    }

    private Quaternion GetRotation(int count)
    {
        return (count % 4) switch
        {
            0 => Quaternion.identity,// Top 0 degrees
            1 => Quaternion.Euler(0, 0, 270),// Right 270 degrees
            2 => Quaternion.Euler(0, 0, 180),// Bottom 180 degrees
            3 => Quaternion.Euler(0, 0, 90),// Left 90 degrees
            _ => Quaternion.identity,
        };
    }
}
