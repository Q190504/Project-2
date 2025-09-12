using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(ResetWeaponSystem))]
[UpdateAfter(typeof(PlayerMovementSystem))]
public partial struct PawPrintPoisonerSystem : ISystem
{
    private EntityManager entityManager;
    private Entity player;
    private Entity pawPrintPoisonerEntity;
    private NativeList<Entity> clouds;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PawPrintPoisonerComponent>();
        clouds = new NativeList<Entity>(24, Allocator.Persistent); // Initialize the list with a capacity of the maximum number of clouds
        entityManager = state.EntityManager;
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out player))
        {
            Debug.Log($"Cant Found Player Entity in PawPrintPoisonerSystem!");
            return;
        }

        if (!SystemAPI.TryGetSingletonEntity<PawPrintPoisonerComponent>(out pawPrintPoisonerEntity))
        {
            Debug.Log($"Cant Found Paw Print Poisoner Entity in PawPrintPoisonerSystem!");
            return;
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Clean up all old clouds from the previous game
        if (SystemAPI.TryGetSingleton<InitializationTrackerComponent>(out var tracker)
            && !tracker.hasCleanCloudList 
            && GameManager.Instance.IsInitializing())
        {
            if (!clouds.IsEmpty)
                clouds.Clear();

            // Update tracker
            tracker.hasCleanCloudList = true;
            ecb.SetComponent(SystemAPI.GetSingletonEntity<InitializationTrackerComponent>(), tracker);
        }

        //Game has not started yet
        if(!GameManager.Instance.IsPlaying())
        {
            return;
        }

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Get Ability Haste
        float abilityHaste = 0;
        if (SystemAPI.TryGetSingleton<AbilityHasteComponent>(out AbilityHasteComponent abilityHasteComponent))
        {
            abilityHaste = abilityHasteComponent.abilityHasteValue;
        }
        else
        {
            Debug.Log($"Cant Found Ability Haste Component in PawPrintPoisonerSystem!");
        }

        // Get Generic Damage Modifier
        float genericDamageModifier = 0;
        if (SystemAPI.TryGetSingleton<GenericDamageModifierComponent>(out GenericDamageModifierComponent genericDamageModifierComponent))
        {
            genericDamageModifier = genericDamageModifierComponent.genericDamageModifierValue;
        }
        else
        {
            Debug.Log($"Cant find Generic Damage Modifier Component in PawPrintPoisonerSystem!");
        }

        // Get Frenzy data
        SlimeFrenzyComponent slimeFrenzyComponent;
        float bonusDamagePercent = 0;
        if (SystemAPI.HasComponent<SlimeFrenzyComponent>(player))
        {
            slimeFrenzyComponent = entityManager.GetComponentData<SlimeFrenzyComponent>(player);
            if(slimeFrenzyComponent.isActive)
                bonusDamagePercent = slimeFrenzyComponent.bonusDamagePercent;
        }
        else
        {
            Debug.Log($"Cant find Slime Frenzy Component in PawPrintPoisonCloudDamageSystem!");
        }


        float deltaTime = SystemAPI.Time.DeltaTime;

        PawPrintPoisonerComponent pawPrintPoisoner = 
            entityManager.GetComponentData<PawPrintPoisonerComponent>(pawPrintPoisonerEntity);

        WeaponComponent weaponComponent = 
            entityManager.GetComponentData<WeaponComponent>(pawPrintPoisonerEntity);

        pawPrintPoisoner.timer -= deltaTime;
        if (pawPrintPoisoner.timer > 0) return;

        ref var blobData = ref pawPrintPoisoner.Data;
        if (!blobData.IsCreated || blobData.Value.Levels.Length == 0) return;

        // Determine pawPrintPoisonerComponent level
        int level = weaponComponent.Level;

        if (level <= 0) // is inactive
        {
            return;
        }

        // Take Paw Print Poisoner's data
        float baseCooldownTime = pawPrintPoisoner.cooldown;
        float finalCooldownTime = baseCooldownTime * (100 / (100 + abilityHaste));

        float tick = pawPrintPoisoner.tick;
        float distanceToCreateACloud = pawPrintPoisoner.distanceToCreateACloud;
        float distanceTraveled = pawPrintPoisoner.distanceTraveled;

        int maximumClouds = pawPrintPoisoner.maximumClouds;

        // Take cloud's level data
        ref var levelData = ref blobData.Value.Levels[level];

        int damagePerTick = levelData.damagePerTick;
        int finalDamagePerTick = (int)(damagePerTick * (1 + genericDamageModifier + bonusDamagePercent));

        float cloudRadius = levelData.cloudRadius;
        float maximumCloudDuration = levelData.maximumCloudDuration;
        float bonusMoveSpeedPerTargetInTheCloudModifier = levelData.bonusMoveSpeedPerTargetInTheCloudModifier;

        // Update distance traveled
        PlayerMovementSpeedComponent playerMovementComponent = entityManager.GetComponentData<PlayerMovementSpeedComponent>(player);
        float playerCurrentSpeed = playerMovementComponent.totalSpeed;
        float distanceThisFrame = playerCurrentSpeed * deltaTime;
        distanceTraveled += distanceThisFrame;

        // If the player moved at least distanceToCreateACloud & is not in any existing cloud, create a new one
        if (distanceTraveled >= distanceToCreateACloud)
        {
            // Check if the player is in any existing cloud
            bool isNotInCloud = true;
            float3 playerPos = entityManager.GetComponentData<LocalTransform>(player).Position;
            foreach (var (pawPrintPoisonCloudComponent, cloudTransform, poisonCloudEntity) in SystemAPI.Query<RefRO<PawPrintPoisonCloudComponent>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                float3 cloudPos = cloudTransform.ValueRO.Position;
                float distance = math.distance(playerPos, cloudPos);
                if (distance < pawPrintPoisonCloudComponent.ValueRO.cloudRadius)
                {
                    isNotInCloud = false;
                    break;
                }
            }

            // If the player is in any existing cloud, do not create a new one
            if (!isNotInCloud)
            {
                return;
            }

            // If total number of clouds is greater than maximumClouds, remove the oldest cloud
            if (clouds.Length >= maximumClouds)
            {
                Entity oldestCloud = clouds[0];

                clouds.RemoveAt(0);

                ProjectilesManager.Instance.ReturnPoisonCloud(oldestCloud, ecb);
            }

            // Spawn the cloud
            Entity cloudEntity = ProjectilesManager.Instance.TakePoisonCloud(ecb);

            // Set the cloud's stats
            SetCloudStats(ecb, cloudEntity, tick, finalDamagePerTick, cloudRadius, maximumCloudDuration,
                bonusMoveSpeedPerTargetInTheCloudModifier);

            // Add this cloud to the list of clouds
            if (!clouds.Contains(cloudEntity))
                clouds.Add(cloudEntity);

            pawPrintPoisoner.timer = finalCooldownTime; // Reset timer
            distanceTraveled = 0; // Reset distance traveled
        }

        pawPrintPoisoner.distanceTraveled = distanceTraveled;

        ecb.SetComponent(pawPrintPoisonerEntity, pawPrintPoisoner);
    }

    public void OnDestroy(ref SystemState state)
    {
        clouds.Dispose();
    }

    public void SetCloudStats(EntityCommandBuffer ecb, Entity cloud, float tick, int damagePerTick, float cloudRadius,
        float maximumCloudDuration, float bonusMoveSpeedPerTargetInTheCloudModifier)
    {
        float3 playerPosition = entityManager.GetComponentData<LocalTransform>(player).Position;

        ecb.SetComponent(cloud, new LocalTransform
        {
            Position = playerPosition,
            Rotation = Quaternion.identity,
            Scale = cloudRadius
        });

        if (!entityManager.HasComponent<PawPrintPoisonCloudComponent>(cloud))
        {
            ecb.AddComponent(cloud, new PawPrintPoisonCloudComponent
            {
                tick = tick,
                tickTimer = tick,
                damagePerTick = damagePerTick,
                cloudRadius = cloudRadius,
                maximumCloudDuration = maximumCloudDuration,
                existDurationTimer = maximumCloudDuration,
                bonusMoveSpeedPerTargetInTheCloudModifier = bonusMoveSpeedPerTargetInTheCloudModifier,
                totalEnemiesCurrentlyInTheCloud = 0,
            });
        }
        else
        {
            ecb.SetComponent(cloud, new PawPrintPoisonCloudComponent
            {
                tick = tick,
                tickTimer = tick,
                damagePerTick = damagePerTick,
                cloudRadius = cloudRadius,
                maximumCloudDuration = maximumCloudDuration,
                existDurationTimer = maximumCloudDuration,
                bonusMoveSpeedPerTargetInTheCloudModifier = bonusMoveSpeedPerTargetInTheCloudModifier,
                totalEnemiesCurrentlyInTheCloud = 0,
            });
        }
    }
}
