using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static AnimationManager _instance;

    [Header("Visual Prefabs")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private int hitEffectPrepare;
    private int hitEffectCount;
    private List<GameObject> inactiveHitEffectGameObjects;

    public static AnimationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<AnimationManager>();
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
        inactiveHitEffectGameObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void PrepareHitEffect()
    {
        if (hitEffectPrefab == null) return;

        for (int i = 0; i < hitEffectPrepare; i++)
        {
            GameObject hitEffect = Object.Instantiate(hitEffectPrefab);
            hitEffect.gameObject.SetActive(false);
            inactiveHitEffectGameObjects.Add(hitEffect);
            hitEffectCount++;
        }
    }

    public GameObject TakeHitEffect()
    {
        if (hitEffectCount == 0)
            PrepareHitEffect();

        GameObject hitEffect = inactiveHitEffectGameObjects[0];
        inactiveHitEffectGameObjects.RemoveAt(0);
        hitEffectCount--;
        hitEffect.gameObject.SetActive(true);
        return hitEffect;
    }

    public void ReturnHitEffect(GameObject hitEffect)
    {
        hitEffect.gameObject.SetActive(false);
        inactiveHitEffectGameObjects.Add(hitEffect);
        hitEffectCount++;
    }

    public void Initialize()
    {
        if (hitEffectCount == 0)
            PrepareHitEffect();

        GameInitializationManager.Instance.animationsPrepared = true;
    }
}
