using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerUpgradeSlots : MonoBehaviour
{
    [SerializeField] WeaponType defaultWeaponType = WeaponType.SlimeBulletShooter;
    [SerializeField] int maxWeaponSlots = 3;
    [SerializeField] int maxPassvieSlots = 5;

    private List<BaseWeapon> weapons;
    private List<BasePassive> passives;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize()
    {
        weapons.Clear();
        passives.Clear();

        // Update UI
        UpgradeEventArgs upgradeEventArgs = new WeaponUpgradeEventArgs(defaultWeaponType, 1);
        GamePlayUIManager.Instance.UpdateSlots(upgradeEventArgs);

        GameInitializationManager.Instance.playerUpgradeSlotsInitialized = true;
    }

    public List<BaseWeapon> GetWeaponList()
    {
        return weapons;
    }

    public BaseWeapon GetWeaponAtIndex(int index)
    {
        if (index >= 0 && index < weapons.Count)
            return weapons[index];
        else
            return null;
    }


    public int GetMaxWeaponSlots()
    {
        return maxWeaponSlots;
    }

    public int GetWeaponLevel(WeaponType weaponType)
    {
        foreach (BaseWeapon weapon in weapons)
        {
            if (weapon.GetWeaponType().Equals(weaponType))
            {
                return weapon.GetCurrentLevel();
            }
        }

        return -1;
    }

    public void LevelUpWeapon(WeaponType weaponType)
    {
        foreach (BaseWeapon weapon in weapons)
        {
            if (weapon.GetWeaponType().Equals(weaponType))
            {
                weapon.LevelUp();
                return;
            }
        }
    }

    public List<BasePassive> GetPassiveList()
    {
        return passives;
    }

    public BasePassive GetPassvieAtIndex(int index)
    {
        if (index >= 0 && index < passives.Count)
            return passives[index];
        else
            return null;
    }

    public int GetMaxPassvieSlots()
    {
        return maxPassvieSlots;
    }

    public int GetPassiveLevel(PassiveType passiveType)
    {
        foreach (BasePassive passive in passives)
        {
            if (passive.GetPassiveType().Equals(passiveType))
            {
                return passive.GetCurrentLevel();
            }
        }

        return -1;
    }

    public void LevelUpPassive(PassiveType passiveType)
    {
        foreach (BasePassive passive in passives)
        {
            if (passive.GetPassiveType().Equals(passiveType))
            {
                passive.LevelUp();
                return;
            }
        }
    }
}
