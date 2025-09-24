using System.Collections.Generic;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    private static PassiveManager _instance;

    [SerializeField] private List<BasePassive> passives;

    public static PassiveManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<PassiveManager>();
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
        passives = new List<BasePassive>();
    }

    public void StartInitialize()
    {
        if (passives.Count > 0)
            foreach (BasePassive passive in passives)
                if (passive.IsInitialized())
                    passive.Initialize();

        GameInitializationManager.Instance.passivesInitialized = true;
    }

    public List<BasePassive> GetPassives()
    {
        return passives;
    }
}
