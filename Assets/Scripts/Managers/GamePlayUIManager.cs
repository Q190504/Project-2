using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIManager : MonoBehaviour
{
    private static GamePlayUIManager _instance;

    [Header("Panels")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject comfirmExitPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text countdownSelectionText;
    [SerializeField] private TMP_Text inGameTimeText;
    [SerializeField] private TMP_Text inGameEnemyKilledText;
    [SerializeField] private TMP_Text endGamePanelTitleText;
    [SerializeField] private TMP_Text endGameTimeText;
    [SerializeField] private TMP_Text endGameEnemyKilledText;

    [Header("Bars")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TMP_Text xpText;

    [SerializeField] private Slider leftCountdownBar;
    [SerializeField] private Slider rightCountdownBar;

    [Header("Audio Setting")]
    public Slider sfxVolumeBar;
    [SerializeField] private Slider bgmVolumeBar;
    [SerializeField] private FloatPublisherSO setSFXSO;
    [SerializeField] private FloatPublisherSO setBGMSO;

    [Header("Skills")]
    public Image skill1Image;
    [SerializeField] private Image skill1CoodownImage;
    [SerializeField] private TMP_Text skill1CoodownText;
    [SerializeField] private Image skill2Image;
    [SerializeField] private Image skill2CoodownImage;
    [SerializeField] private TMP_Text skill2CoodownText;

    [Header("Weapons")]
    [SerializeField] private List<UpgradeSlot> weaponSlots;
    private int currentEmptyWeaponSlotIndex;

    [Header("Passives")]
    [SerializeField] private List<UpgradeSlot> passiveSlots;
    private int currentEmptyPassiveSlotIndex;

    [Header("Effects")]
    [SerializeField] private GameObject effectImagePrefab;
    [SerializeField] private Transform effectsLayout;
    [SerializeField] private Sprite stunEffectSprite;
    [SerializeField] private Sprite frenzyEffectSprite;

    private List<GameObject> effectImageList = new List<GameObject>();

    private Dictionary<EffectType, Sprite> effectSprites;
    private Dictionary<EffectType, int> effectIndexes;

    [Header("Cards")]
    [SerializeField] private Transform cardLayout;
    [SerializeField] private UpgradeCard passiveCardPrefab;
    [SerializeField] private UpgradeCard weaponCardPrefab;
    [SerializeField] private GameObjectPublisherSO addCardSO;

    [Header("Stats Icon")]
    [SerializeField] private Sprite damageIcon;
    [SerializeField] private Sprite maxHPIcon;
    [SerializeField] private Sprite moveSpeedIcon;
    [SerializeField] private Sprite healthRegenIcon;
    [SerializeField] private Sprite pickupRadiusIcon;
    [SerializeField] private Sprite armorIcon;
    [SerializeField] private Sprite abilityHasteIcon;

    [Header("Weapon Icon")]
    [SerializeField] private Sprite slimeBulletShooterIcon;
    [SerializeField] private Sprite slimeBeamShooterIcon;
    [SerializeField] private Sprite pawPrintPoisonerIcon;
    [SerializeField] private Sprite radiantFieldIcon;

    private Dictionary<PassiveType, Sprite> passiveIcons;
    private Dictionary<WeaponType, Sprite> weaponIcons;
    private Dictionary<PlayerSkillID, Image> skillImages;
    private Dictionary<PlayerSkillID, Image> skillCooldownImages;
    private Dictionary<PlayerSkillID, TMP_Text> skillCooldownTexts;

    public static GamePlayUIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<GamePlayUIManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        passiveIcons = new Dictionary<PassiveType, Sprite>
        {
            { PassiveType.Damage, damageIcon },
            { PassiveType.MaxHealth, maxHPIcon },
            { PassiveType.MoveSpeed, moveSpeedIcon },
            { PassiveType.HealthRegen, healthRegenIcon },
            { PassiveType.PickupRadius, pickupRadiusIcon },
            { PassiveType.Armor, armorIcon },
            { PassiveType.AbilityHaste, abilityHasteIcon }
        };

        weaponIcons = new Dictionary<WeaponType, Sprite>
        {
            { WeaponType.SlimeBulletShooter, slimeBulletShooterIcon },
            { WeaponType.SlimeBeamShooter, slimeBeamShooterIcon },
            { WeaponType.PawPrintPoisoner, pawPrintPoisonerIcon },
            { WeaponType.RadiantField, radiantFieldIcon }
        };

        skillImages = new Dictionary<PlayerSkillID, Image>
        {
            { PlayerSkillID.Frenzy, skill1Image },
            { PlayerSkillID.Reclaim, skill2Image },
        };

        skillCooldownImages = new Dictionary<PlayerSkillID, Image>
        {
            { PlayerSkillID.Frenzy, skill1CoodownImage },
            { PlayerSkillID.Reclaim, skill2CoodownImage },
        };

        skillCooldownTexts = new Dictionary<PlayerSkillID, TMP_Text>
        {
            { PlayerSkillID.Frenzy, skill1CoodownText },
            { PlayerSkillID.Reclaim, skill2CoodownText },
        };

        effectSprites = new Dictionary<EffectType, Sprite>
        {
            { EffectType.Stun, stunEffectSprite },
            { EffectType.Frenzy, frenzyEffectSprite }
        };

        effectIndexes = new Dictionary<EffectType, int>
        {
            { EffectType.Stun, -1 },
            { EffectType.Frenzy, -1 }
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenApplication();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPlaying() && Input.GetKeyDown(KeyCode.Escape))
        {
            SetSettingPanel(!settingPanel.activeSelf);
        }
    }

    public void UpdateHPBar(int currentHP, int maxHP)
    {
        hpBar.maxValue = maxHP;
        hpBar.value = currentHP;

        hpText.text = $"{hpBar.value}/{hpBar.maxValue}";
    }

    public void UpdateXPBar(int currentLevel, int experience, int experienceToNextLevel)
    {
        currentLevelText.text = currentLevel.ToString();
        xpBar.maxValue = experienceToNextLevel;
        xpBar.value = experience;

        xpText.text = $"{xpBar.value}/{xpBar.maxValue}";
    }

    // Set opacity (0 = fully transparent, 1 = fully opaque)
    public void SetImageOpacity(Image image, float alpha)
    {
        Color color = image.color;
        color.a = Mathf.Clamp01(alpha);
        image.color = color;
    }

    public void SetSkillImageOpacity(PlayerSkillID skillID, bool status)
    {
        if (skillImages.TryGetValue(skillID, out Image image))
        {
            SetImageOpacity(image, status ? 1f : 0.5f);
        }
        else
        {
            Debug.LogWarning($"No image found for skill: {skillID}");
        }
    }

    public void SetSkillCooldownUI(PlayerSkillID skillID, bool status)
    {
        if (skillCooldownImages.TryGetValue(skillID, out Image cooldownImage))
        {
            cooldownImage.gameObject.SetActive(status);
        }
        else
        {
            Debug.LogWarning($"No cooldown image found for skill: {skillID}");
        }

        if (skillCooldownTexts.TryGetValue(skillID, out TMP_Text cooldownText))
        {
            cooldownText.gameObject.SetActive(status);
        }
        else
        {
            Debug.LogWarning($"No cooldown text found for skill: {skillID}");
        }
    }

    public void UpdateSkillCooldownUI(PlayerSkillID skillID, float timeRemaining, float cooldownTime)
    {
        if (skillCooldownImages.TryGetValue(skillID, out Image cooldownImage))
        {
            cooldownImage.fillAmount = timeRemaining / cooldownTime;
        }
        else
        {
            Debug.LogWarning($"No cooldown image found for skill: {skillID}");
        }

        if (skillCooldownTexts.TryGetValue(skillID, out TMP_Text cooldownText))
        {
            cooldownText.text = ((int)timeRemaining).ToString();
        }
        else
        {
            Debug.LogWarning($"No cooldown text found for skill: {skillID}");
        }
    }

    public int GetEffectIndexes(EffectType type)
    {
        return effectIndexes[type];
    }

    public void AddEffectImage(EffectType type)
    {
        if (!effectSprites.ContainsKey(type)) return;

        // Create effect image
        GameObject effectImage = Instantiate(effectImagePrefab, effectsLayout);

        // Set sprite
        Image[] images = effectImage.GetComponentsInChildren<Image>();
        if (images.Length > 0)
            images[0].sprite = effectSprites[type];

        // Track object
        effectImageList.Add(effectImage);

        // Assign index if not set
        if (effectIndexes[type] == -1)
            effectIndexes[type] = effectImageList.Count - 1;
    }

    public void UpdateEffectDurationUI(EffectType type, float timeRemaining, float initialTime)
    {
        if (!effectIndexes.ContainsKey(type)) return;

        int index = effectIndexes[type];
        if (index == -1 || index >= effectImageList.Count) return;

        Image[] images = effectImageList[index].GetComponentsInChildren<Image>();
        if (images.Length > 1)
            images[1].fillAmount = timeRemaining / initialTime;
    }

    public void RemoveEffectImage(EffectType type)
    {
        if (!effectIndexes.ContainsKey(type)) return;

        int index = effectIndexes[type];
        if (index == -1 || index >= effectImageList.Count) return;

        Destroy(effectImageList[index]);
        effectImageList[index] = null;
        effectIndexes[type] = -1;
    }

    public void SetSettingPanel(bool status)
    {
        settingPanel.SetActive(status);
    }

    public void SetTitlePanel(bool status)
    {
        titlePanel.SetActive(status);
    }

    public void SetTime(double time)
    {
        inGameTimeText.text = $"{(int)time / 60:D2} : {(int)time % 60:D2}";
    }

    public void SetEnemyKilled(int enemyKilled)
    {
        inGameEnemyKilledText.text = enemyKilled.ToString();
    }

    public void SetCreditPanel(bool status)
    {
        creditPanel.SetActive(status);
    }

    public void OpenEndGamePanel(bool result)
    {
        if (result)
        {
            endGamePanelTitleText.text = "VICTORY";
        }
        else
        {
            endGamePanelTitleText.text = "DEFEATED";
        }

        endGameTimeText.text = inGameTimeText.text;
        endGameEnemyKilledText.text = inGameEnemyKilledText.text;

        endGamePanel.SetActive(true);
    }

    public void CloseEndGamePanel()
    {
        endGameTimeText.text = "00 : 00";
        endGameEnemyKilledText.text = "0";
        endGamePanel.SetActive(false);
    }

    public void OnStartGame()
    {
        SetSettingPanel(false);
        SetTitlePanel(false);
        CloseEndGamePanel();
        SetConfirmExitPanel(false);
        CloseUpgradePanel();
        ClearSlots();
    }

    public void SetConfirmExitPanel(bool status)
    {
        comfirmExitPanel.SetActive(status);
    }

    public void OpenUpgradePanel(List<UpgradeOption> upgradeOptions)
    {
        ClearCards();

        // Add cards
        foreach (UpgradeOption upgradeOption in upgradeOptions)
        {
            AddCard(upgradeOption.cardType, upgradeOption.weaponType, upgradeOption.passiveType,
                upgradeOption.currentLevel + 1, upgradeOption.displayName.ToString(),
                upgradeOption.description.ToString());
        }

        upgradePanel.SetActive(true);
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
        ClearCards();
    }

    public void AddCard(UpgradeType upgradeType, WeaponType weaponType, PassiveType passiveType, int level,
        string name, string description)
    {
        Sprite image = null;
        UpgradeCard upgradeCard = null;

        if (upgradeType == UpgradeType.Passive)
        {
            upgradeCard = Instantiate(passiveCardPrefab, cardLayout);
            passiveIcons.TryGetValue(passiveType, out image);
        }
        else if (upgradeType == UpgradeType.Weapon)
        {
            upgradeCard = Instantiate(weaponCardPrefab, cardLayout);
            weaponIcons.TryGetValue(weaponType, out image);
        }

        if (image == null)
        {
            if (upgradeType == UpgradeType.Passive)
                Debug.LogWarning($"[GamePlayUIManager] Unknown type for passive: {passiveType}");
            else if (upgradeType == UpgradeType.Weapon)
                Debug.LogWarning($"[GamePlayUIManager] Unknown type for weapon: {weaponType}");
            else
                Debug.LogWarning($"[GamePlayUIManager] Unknown type for image: {name}");
        }

        upgradeCard.SetCardInfo(upgradeType, weaponType, passiveType, name, description, image, level);
        addCardSO.RaiseEvent(upgradeCard.gameObject);
    }

    public void ClearCards()
    {
        if (cardLayout.childCount > 0)
            foreach (Transform child in cardLayout)
                Destroy(child.gameObject);
    }

    public void UpdateCountdown(float timeLeft, float totalTime)
    {
        leftCountdownBar.maxValue = totalTime;
        leftCountdownBar.value = timeLeft;

        rightCountdownBar.maxValue = totalTime;
        rightCountdownBar.value = timeLeft;

        countdownSelectionText.text = $"{(int)timeLeft}";
    }

    public void UpdateSlots(UpgradeEventArgs upgradeEventArgs)
    {
        if (upgradeEventArgs.upgradeType == UpgradeType.Weapon)
        {
            foreach (UpgradeSlot slot in weaponSlots)
            {
                if (slot.GetUpgradeType() == UpgradeType.Weapon 
                    && slot.GetWeaponType() == upgradeEventArgs.weaponType)
                {
                    slot.SetLevel(upgradeEventArgs.level);
                    return;
                }
            }

            // set the current empty one if not found
            if (weaponIcons.TryGetValue(upgradeEventArgs.weaponType, out Sprite iconSprite))
                weaponSlots[currentEmptyWeaponSlotIndex].SetSlotInfo(upgradeEventArgs.upgradeType, 
                    upgradeEventArgs.weaponType, PassiveType.None, iconSprite, upgradeEventArgs.level);
            else
            {
                Debug.LogWarning($"[GamePlayUIManager] Unknown weapon type: {upgradeEventArgs.weaponType}, using null sprite.");
                weaponSlots[currentEmptyWeaponSlotIndex].SetSlotInfo(upgradeEventArgs.upgradeType,
                    upgradeEventArgs.weaponType, PassiveType.None, iconSprite, upgradeEventArgs.level);
            }

            currentEmptyWeaponSlotIndex++;
        }
        else if (upgradeEventArgs.upgradeType == UpgradeType.Passive)
        {
            foreach (UpgradeSlot slot in passiveSlots)
            {
                if (slot.GetUpgradeType() == UpgradeType.Passive 
                    && slot.GetPassiveType() == upgradeEventArgs.passiveType)
                {
                    slot.SetLevel(upgradeEventArgs.level);
                    return;
                }
            }

            // set the current empty one if not found
            if (passiveIcons.TryGetValue(upgradeEventArgs.passiveType, out Sprite iconSprite))
                passiveSlots[currentEmptyPassiveSlotIndex].SetSlotInfo(upgradeEventArgs.upgradeType,
                    WeaponType.None, upgradeEventArgs.passiveType, iconSprite, upgradeEventArgs.level);
            else
            {
                Debug.LogWarning($"[GamePlayUIManager] Unknown passive type: {upgradeEventArgs.passiveType}, using null sprite.");
                passiveSlots[currentEmptyPassiveSlotIndex].SetSlotInfo(upgradeEventArgs.upgradeType,
                    WeaponType.None, upgradeEventArgs.passiveType, iconSprite, upgradeEventArgs.level);
            }

            currentEmptyPassiveSlotIndex++;
        }
        else
        {
            Debug.LogError($"[GamePlayUIManager] Unknown upgrade type: {upgradeEventArgs.upgradeType}");
        }
    }

    public void ClearSlots()
    {
        foreach (UpgradeSlot slot in weaponSlots)
        {
            slot.ClearSlotInfo();
        }

        currentEmptyWeaponSlotIndex = 0;

        foreach (UpgradeSlot slot in passiveSlots)
        {
            slot.ClearSlotInfo();
        }

        currentEmptyPassiveSlotIndex = 0;
    }

    public void OpenApplication()
    {
        SetSettingPanel(false);
        SetTitlePanel(true);
        CloseEndGamePanel();
        SetConfirmExitPanel(false);
        CloseUpgradePanel();
        ClearSlots();

        SetSkillCooldownUI(PlayerSkillID.Frenzy, false);
        SetSkillCooldownUI(PlayerSkillID.Reclaim, false);
    }

    public void SetBGMSlider(float value)
    {
        bgmVolumeBar.value = value;
    }

    public void SetSFXSlider(float value)
    {
        sfxVolumeBar.value = value;
    }

    public void SetSFX()
    {
        setSFXSO.RaiseEvent(sfxVolumeBar.value);
    }

    public void SetBGM()
    {
        setBGMSO.RaiseEvent(bgmVolumeBar.value);
    }

    public void ExitGame()
    {
        settingPanel.SetActive(false);
        titlePanel.SetActive(true);
    }
}
