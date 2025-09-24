using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UpgradeSlot : MonoBehaviour
{
    private UpgradeType upgradeType;
    private WeaponType weaponType;
    private PassiveType passiveType;

    [SerializeField] private Image image;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject levelContainer;

    void Start ()
    {
        ClearSlotInfo();
    }

    public void SetSlotInfo(UpgradeType upgradeType, WeaponType weaponType, PassiveType passiveType, 
        Sprite image, int level)
    {
        this.upgradeType = upgradeType;
        this.weaponType = weaponType;
        this.passiveType = passiveType;
        this.levelText.text = level.ToString();
        this.image.sprite = image;
        levelContainer.SetActive(true);
        this.image.enabled = true;
    }

    public void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }

    public void ClearSlotInfo()
    {
        image.enabled = false;
        levelContainer.SetActive(false);
        weaponType = WeaponType.None;
        passiveType = PassiveType.None;
        levelText.text = "0";
    }

    public UpgradeType GetUpgradeType()
    {
        return upgradeType;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public PassiveType GetPassiveType()
    { 
        return passiveType; 
    }
}
