using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class UpgradeOfferingHelper
{
    public static List<UpgradeOption> GenerateOfferings(PlayerUpgradeSlots slots)
    {
        var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
        var offerings = new List<UpgradeOption>();

        #region Query valid weapons

        List<BaseWeapon> validWeapons = new List<BaseWeapon>();

        List<BaseWeapon> weapons = WeaponManager.Instance.GetWeapons();

        if (weapons.Count > 0)
        {
            foreach (BaseWeapon weapon in weapons)
            {
                bool existsInSlots = false;
                int currentLevel = 0;

                // Check if the weapon is already in the player's slots
                for (int i = 0; i < slots.GetWeaponList().Count; i++)
                {
                    if (slots.GetWeaponAtIndex(i).GetWeaponType() == weapon.GetWeaponType())
                    {
                        existsInSlots = true;
                        currentLevel = slots.GetWeaponAtIndex(i).GetCurrentLevel();
                        break;
                    }
                }

                // If weapon is in slots and level is below 5, or (not in slots at all && hasnt full slot)
                if ((existsInSlots && currentLevel < 5) || !existsInSlots && slots.GetWeaponList().Count < slots.GetMaxWeaponSlots())
                {
                    validWeapons.Add(weapon);
                }

            }
        }

        #endregion

        #region Query valid passives

        List<BasePassive> validPassives = new List<BasePassive>();

        List<BasePassive> passives = PassiveManager.Instance.GetPassives();

        if (passives.Count > 0)
        {
            foreach (BasePassive passive in passives)
            {
                bool existsInSlots = false;
                int currentLevel = 0;

                // Check if the passive is already in the player's slots
                for (int i = 0; i < slots.GetPassiveList().Count; i++)
                {
                    if (slots.GetPassvieAtIndex(i).GetPassiveType() == passive.GetPassiveType())
                    {
                        existsInSlots = true;
                        currentLevel = slots.GetPassvieAtIndex(i).GetCurrentLevel();
                        break;
                    }
                }

                // If passive is in slots and level is below 5, or (not in slots at all && hasnt full slot)
                if ((existsInSlots && currentLevel < 5) || !existsInSlots && slots.GetPassiveList().Count < slots.GetMaxPassvieSlots())
                {
                    validPassives.Add(passive);
                }
            }
        }

        #endregion

        #region combine valid weapons & passives into valid options

        List<UpgradeOption> combined = new List<UpgradeOption>();

        // Add valid weapons to the combined list
        foreach (var weapon in validWeapons)
        {
            WeaponUpgradeOption option = new WeaponUpgradeOption(weapon);
            combined.Add(option);
        }

        // Add valid passives to the combined list
        foreach (var passive in validPassives)
        {
            PassiveUpgradeOption option = new PassiveUpgradeOption(passive);
            combined.Add(option);
        }

        #endregion

        // Randomly select 3 from combined list
        while (offerings.Count < 3 && combined.Count > 0)
        {
            int index = random.NextInt(combined.Count);
            offerings.Add(combined[index]);
            combined.RemoveAt(index);
        }

        return offerings;
    }
}
