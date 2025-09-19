using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PawPrintPoisonerWeapon : BaseWeapon
{
    [SerializeField] private List<PawPrintPoisonerLevelDataSO> levelDatas;

    private bool canSpawnNewCloud;

    private float timer;
    [SerializeField] private float tick;
    [SerializeField] private float cooldown;
    [SerializeField] private int maximumClouds;
    [SerializeField] private float distanceToCreateACloud;
    private float distanceTraveled;

    [Header("Refs")]
    [SerializeField] private GameObject player;

    private PlayerMovement playerMovement;
    private AbilityHaste abilityHaste;
    private GenericDamageModifier genericDamageModifier;
    private FrenzySkill FrenzySkill;

    private List<PoisonCloud> activePoisonClouds;

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
        FrenzySkill = player.GetComponent<FrenzySkill>();

        activePoisonClouds = new List<PoisonCloud>();

        hasInitialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive) return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        PawPrintPoisonerLevelDataSO levelData = GetCurrentLevelData();

        int damagePerTick = levelData.damagePerTick;
        int finalDamagePerTick = (int)(damagePerTick * (1 + genericDamageModifier.GetValue() + FrenzySkill.GetFrenzyBonusPercent()));

        float cloudRadius = levelData.cloudRadius;
        float maximumCloudDuration = levelData.maximumCloudDuration;
        float bonusMoveSpeedPerTargetInTheCloudModifier = levelData.bonusMoveSpeedPerTargetInTheCloudModifier;

        // Update distance traveled
        float playerCurrentSpeed = playerMovement.GetCurrentSpeed();
        float distanceThisFrame = playerCurrentSpeed * Time.deltaTime;
        distanceTraveled += distanceThisFrame;

        // If the player moved at least distanceToCreateACloud & is not in any existing cloud, create a new one
        if (distanceTraveled >= distanceToCreateACloud)
        {
            // If the player isn't in any existing cloud, create a new one
            if (canSpawnNewCloud)
            {
                // If total number of clouds is greater than maximumClouds, remove the oldest cloud
                if (activePoisonClouds.Count >= maximumClouds)
                {
                    PoisonCloud oldestCloud = activePoisonClouds[0];
                    activePoisonClouds.Remove(oldestCloud);
                    ProjectilesManager.Instance.ReturnPoisonCloud(oldestCloud);
                }

                // Spawn the cloud
                PoisonCloud cloud = ProjectilesManager.Instance.TakePoisonCloud();

                // Set the cloud's stats
                SetCloudStats(cloud, tick, finalDamagePerTick, cloudRadius, maximumCloudDuration,
                    bonusMoveSpeedPerTargetInTheCloudModifier);

                // Add this cloud to the list of clouds
                if (!activePoisonClouds.Contains(cloud))
                    activePoisonClouds.Add(cloud);

                float finalCooldownTime = abilityHaste.GetCooldownTimeAfterReduction(cooldown);
                timer = finalCooldownTime; // Reset timer
                distanceTraveled = 0; // Reset distance traveled
            }
        }

        // Boost Speed
        if (bonusMoveSpeedPerTargetInTheCloudModifier > 0)
        {
            int totalEnemiesInCloud = 0;
            foreach (var cloud in activePoisonClouds)
                totalEnemiesInCloud += cloud.GetTotalEnemies();

            // Update player speed
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            float bonusMultiplier = 1f + (bonusMoveSpeedPerTargetInTheCloudModifier * totalEnemiesInCloud);
            playerMovement.SetCurrentSpeed(playerMovement.GetCurrentSpeed() * bonusMultiplier);
        }
    }

    public void SetCloudStats(PoisonCloud cloud, float tick, int damagePerTick, float cloudRadius,
    float maximumCloudDuration, float bonusMoveSpeedPerTargetInTheCloudModifier)
    {
        Vector2 playerPosition = player.transform.position;

        cloud.transform.position = playerPosition;
        cloud.transform.localScale = new Vector2(cloudRadius, cloudRadius);

        cloud.Initialize(tick, damagePerTick, cloudRadius, maximumCloudDuration, bonusMoveSpeedPerTargetInTheCloudModifier);
    }

    public void SetCanSpawnNewCloud(bool status)
    {
        canSpawnNewCloud = status;
    }

    protected override void Initialize()
    {
        timer = 0;
        distanceTraveled = 0;
        currentLevel = 0;
        canSpawnNewCloud = true;
        if (activePoisonClouds.Count > 0)
            activePoisonClouds.Clear();

        hasInitialized = true;
    }

    private PawPrintPoisonerLevelDataSO GetCurrentLevelData()
    {
        return levelDatas[math.min(currentLevel - 1, maxLevel - 1)];
    }
}
