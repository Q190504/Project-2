using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeCardType;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardDescription;

    [SerializeField] private VoidPublisherSO playSoundSO;

    private WeaponType weaponType;
    private PassiveType passiveType;

    private Dictionary<WeaponType, Action<PlayerUpgradeSlots>> weaponUpgradeHandlers;
    private Dictionary<PassiveType, Action<PlayerUpgradeSlots>> passiveUpgradeHandlers;

    private void Awake()
    {
        weaponUpgradeHandlers = new Dictionary<WeaponType, Action<PlayerUpgradeSlots>>
        {
            { WeaponType.SlimeBulletShooter, (slots) => slots.LevelUpWeapon(WeaponType.SlimeBulletShooter) },
            { WeaponType.SlimeBeamShooter,  (slots) => slots.LevelUpWeapon(WeaponType.SlimeBeamShooter) },
            { WeaponType.RadiantField,      (slots) => slots.LevelUpWeapon(WeaponType.RadiantField) },
            { WeaponType.PawPrintPoisoner,  (slots) => slots.LevelUpWeapon(WeaponType.PawPrintPoisoner) },
        };

        passiveUpgradeHandlers = new Dictionary<PassiveType, Action<PlayerUpgradeSlots>>
        {
            { PassiveType.Damage,      (slots) => slots.LevelUpPassive(PassiveType.Damage) },
            { PassiveType.Armor,       (slots) => slots.LevelUpPassive(PassiveType.Armor) },
            { PassiveType.MaxHealth,   (slots) => slots.LevelUpPassive(PassiveType.MaxHealth) },
            { PassiveType.MoveSpeed,   (slots) => slots.LevelUpPassive(PassiveType.MoveSpeed) },
            { PassiveType.HealthRegen, (slots) => slots.LevelUpPassive(PassiveType.HealthRegen) },
            { PassiveType.PickupRadius,(slots) => slots.LevelUpPassive(PassiveType.PickupRadius) },
            { PassiveType.AbilityHaste,(slots) => slots.LevelUpPassive(PassiveType.AbilityHaste) },
        };
    }

    public void SetCardInfo(UpgradeType type, WeaponType weaponType, PassiveType passiveType,
        string name, string description, Sprite image, int levelNumber)
    {
        upgradeCardType = type;
        level.text = (levelNumber == 1) ? "NEW" : "Lv: " + levelNumber;
        cardName.text = name;
        if (image != null)
            cardImage.sprite = image;
        cardDescription.text = description;

        this.weaponType = weaponType;
        this.passiveType = passiveType;
    }

    public void Select()
    {
        playSoundSO.RaiseEvent();

        var playerSlots = UpgradeManager.Instance.GetPlayerUpgradeSlots();
        if (playerSlots == null)
        {
            Debug.LogError("PlayerUpgradeSlots not found in UpgradeCard!");
            return;
        }

        if (upgradeCardType == UpgradeType.Weapon)
        {
            if (weaponUpgradeHandlers.TryGetValue(weaponType, out var handler))
            {
                handler(playerSlots);
            }
            else
            {
                Debug.LogWarning($"No weapon handler for {weaponType}");
                return;
            }
        }
        else
        {
            if (passiveUpgradeHandlers.TryGetValue(passiveType, out var handler))
            {
                handler(playerSlots);
            }
            else
            {
                Debug.LogWarning($"No passive handler for {passiveType}");
                return;
            }
        }

    }
}
