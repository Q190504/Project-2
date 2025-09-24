using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct OpenUpgradePanelSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        //if (!GameManager.Instance.IsPlaying())
        //    return;

        //var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        //var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        //foreach (var (playerSlots, entity) in
        //         SystemAPI.Query<RefRO<PlayerUpgradeSlotsComponent>>()
        //                  .WithAll<PlayerLevelUpEvent>().WithEntityAccess())
        //{
        //    // Pause game
        //    GameManager.Instance.TogglePauseGameForUpgrading();

        //    // Collect all valid upgrade options
        //    NativeList<UpgradeOptionStruct> offerings
        //        = UpgradeOfferingHelper.GenerateOfferings(playerSlots.ValueRO);

        //    // Open UI
        //    UpgradeManager.Instance.SetTimer();
        //    GamePlayUIManager.Instance.OpenUpgradePanel(offerings);

        //    offerings.Dispose();

        //    ecb.RemoveComponent<PlayerLevelUpEvent>(entity);
        //}
    }
}

