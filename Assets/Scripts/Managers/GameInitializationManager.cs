using System.Collections.Generic;
using UnityEngine;

public class GameInitializationManager : MonoBehaviour
{
    private static GameInitializationManager _instance;

    public bool hasFinishInitialization;
    public bool playerHealthInitialized;
    public bool playerSpeedInitialized;
    public bool playerPositionInitialized;
    public bool playerUpgradeSlotsInitialized;
    public bool playerLevelInitialized;
    public bool passivesInitialized;
    public bool weaponsInitialized;
    public bool hasCleanProjectiles;
    public bool enemySystemInitialized;
    public bool cleanedOrbs;
    public bool animationsPrepared;

    [SerializeField] private VoidPublisherSO startGameSO;

    public static GameInitializationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<GameInitializationManager>();
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
        ResetCheckers();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasFinishInitialization && HasInitializedAll())
        {
            Debug.Log("Finished Initialize All");
            hasFinishInitialization = true;
            startGameSO.RaiseEvent();
        }
    }

    public void ResetCheckers()
    {
        hasFinishInitialization = false;
        playerHealthInitialized = false;
        playerSpeedInitialized = false;
        playerPositionInitialized = false;
        playerUpgradeSlotsInitialized = false;
        playerLevelInitialized = false;
        passivesInitialized = false;
        weaponsInitialized = false;
        hasCleanProjectiles = false;
        enemySystemInitialized = false;
        cleanedOrbs = false;
        animationsPrepared = false;
    }

    private bool HasInitializedAll()
    {
        return playerHealthInitialized
            && playerSpeedInitialized
            && playerPositionInitialized
            && playerUpgradeSlotsInitialized
            && playerLevelInitialized
            && passivesInitialized
            && weaponsInitialized
            && hasCleanProjectiles
            && enemySystemInitialized
            && cleanedOrbs
            && animationsPrepared;
    }
}
