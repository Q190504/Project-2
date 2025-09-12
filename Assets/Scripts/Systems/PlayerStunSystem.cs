using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct PlayerStunSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (stunTimer, entity) in SystemAPI.Query<RefRW<StunTimerComponent>>().WithEntityAccess())
        {
            stunTimer.ValueRW.timeRemaining -= SystemAPI.Time.DeltaTime;

            if (stunTimer.ValueRO.timeRemaining <= 0)
            {
                ecb.RemoveComponent<StunTimerComponent>(entity);

                if (SystemAPI.HasComponent<StunComponent>(entity))
                {
                    var stunComponent = SystemAPI.GetComponent<StunComponent>(entity);
                    stunComponent.isStunned = false;
                    ecb.SetComponent(entity, stunComponent);

                    // Remove the stun effect UI when stun expires
                    GamePlayUIManager.Instance.RemoveEffectImage(ref GamePlayUIManager.Instance.stunEffectIndex);
                }
            }
            else
            {
                if (SystemAPI.HasComponent<StunComponent>(entity))
                {
                    var stunComponent = SystemAPI.GetComponent<StunComponent>(entity);
                    stunComponent.isStunned = false;
                    ecb.SetComponent(entity, stunComponent);

                    //if hasn't Stun Effect Image yet
                    if (GamePlayUIManager.Instance.stunEffectIndex == -1)
                        GamePlayUIManager.Instance.AddStunEffectImage();

                    // Update stun duration UI
                    GamePlayUIManager.Instance.UpdateEffectDurationUI(GamePlayUIManager.Instance.stunEffectIndex, stunTimer.ValueRO.timeRemaining, stunTimer.ValueRO.initialDuration);
                }
            }
        }
    }
}
