using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager _instance;

    [SerializeField] private float totalTime;
    private float timer;
    private List<UpgradeCard> upgradeOptions;
    private PlayerUpgradeSlots playerUpgradeSlots;

    //private List<UpgradeOptionClass> selectedPassiveUpgrades;
    //private List<UpgradeOptionClass> selectedWeaponUpgrades;

    [Header("Refs")]
    [SerializeField] private GameObject player;
    [SerializeField] private TwoFloatPublisherSO updateCountdownSO;
    [SerializeField] private VoidPublisherSO togglePauseSO;

    public static UpgradeManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<UpgradeManager>();
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetTimer();
        upgradeOptions = new List<UpgradeCard>();
        if (player != null)
            playerUpgradeSlots = player.GetComponent<PlayerUpgradeSlots>();
        else
            Debug.LogError("Player isn't assign in UpgradeManager");
        //selectedPassiveUpgrades = new List<UpgradeOptionClass>();
        //selectedWeaponUpgrades = new List<UpgradeOptionClass>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsUpgrading())
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // select the first upgrade option
                ChooseFirstOption();

                timer = totalTime;
            }

            updateCountdownSO.RaiseEvent(timer, totalTime);
        }
    }

    private void ChooseFirstOption()
    {
        if (upgradeOptions != null && upgradeOptions.Count > 0)
        {
            // Select the first upgrade option
            UpgradeCard firstOption = upgradeOptions[0];
            firstOption.Select();
        }
    }

    public void ClearOptions()
    {
        upgradeOptions.Clear();
    }

    public void SetTimer()
    {
        timer = totalTime;
    }

    public void AddUpgradeOption(GameObject card)
    {
        UpgradeCard option = card.GetComponent<UpgradeCard>();

        if (option != null && !upgradeOptions.Contains(option))
        {
            upgradeOptions.Add(option);
        }
    }

    public void ResetTimer()
    {
        timer = totalTime;
    }

    public void OpenUpgradePanel()
    {
        // Pause game
        GameManager.Instance.TogglePauseGameForUpgrading();

        if (playerUpgradeSlots != null)
        {
            // Collect all valid upgrade options
            List<UpgradeOption> offerings
                = UpgradeOfferingHelper.GenerateOfferings(playerUpgradeSlots);

            // Open UI
            SetTimer();
            GamePlayUIManager.Instance.OpenUpgradePanel(offerings);
        }
        else
            Debug.LogError("Can't find PlayerUpgradeSlots in UpgradeManager");
    }

    public PlayerUpgradeSlots GetPlayerUpgradeSlots()
    {
        return playerUpgradeSlots;
    }
}
