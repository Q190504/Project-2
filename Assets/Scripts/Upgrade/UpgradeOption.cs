using Unity.Collections;
using UnityEngine;

public class UpgradeOption
{
    public UpgradeType cardType;
    public string displayName;
    public string description;
    public int currentLevel;        // Current level of this upgrade for the player
    public int maxLevel;            // Max level (typically 5)
    public WeaponType weaponType;   // Valid only if cardType == Weapon
    public PassiveType passiveType; // Valid only if cardType == Passive
}
public class WeaponUpgradeOption : UpgradeOption
{
    public WeaponUpgradeOption(BaseWeapon weapon)
    {
        cardType = UpgradeType.Weapon;
        this.weaponType = weapon.GetWeaponType();
        this.displayName = weapon.GetDisplayName();
        this.description = weapon.GetDescription();
        this.currentLevel = weapon.GetCurrentLevel();
        this.maxLevel = weapon.GetMaxLevel();
    }

    public WeaponUpgradeOption(WeaponType weaponType, string displayName, string description, int currentLevel, int maxLevel)
    {
        cardType = UpgradeType.Weapon;
        this.weaponType = weaponType;
        this.displayName = displayName;
        this.description = description;
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
    }
}

public class PassiveUpgradeOption : UpgradeOption
{
    public PassiveUpgradeOption(BasePassive passive)
    {
        cardType = UpgradeType.Passive;
        this.passiveType = passive.GetPassiveType();
        this.displayName = passive.GetDisplayName();
        this.description = passive.GetDescription();
        this.currentLevel = passive.GetCurrentLevel();
        this.maxLevel = passive.GetMaxLevel();
    }

    public PassiveUpgradeOption(PassiveType passiveType, string displayName, string description, int currentLevel, int maxLevel)
    {
        cardType = UpgradeType.Passive;
        this.passiveType = passiveType;
        this.displayName = displayName;
        this.description = description;
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
    }
}

// ==================================================

public class UpgradeEventArgs
{
    public UpgradeType upgradeType;
    public WeaponType weaponType;
    public PassiveType passiveType;
    public int level;
}

public class WeaponUpgradeEventArgs : UpgradeEventArgs
{
    public WeaponUpgradeEventArgs(WeaponType weaponType, int level)
    {
        upgradeType = UpgradeType.Weapon;
        this.weaponType = weaponType;
        this.level = level;
    }
}

public class PassiveUpgradeEventArgs : UpgradeEventArgs
{
    public PassiveUpgradeEventArgs(PassiveType passiveType, int level)
    {
        upgradeType = UpgradeType.Passive;
        this.passiveType = passiveType;
        this.level = level;
    }
}
