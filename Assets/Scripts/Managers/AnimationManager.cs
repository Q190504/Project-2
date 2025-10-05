using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static AnimationManager _instance;

    [Header("Visual Prefabs")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private int hitEffectPrepare;

    [SerializeField] private GameObject explodeSlimeExplodeEffectPrefab;
    [SerializeField] private int explodeSlimeExplodeEffectPrepare;

    private int inactiveHitEffectCount;
    private int activeHitEffectCount;

    private int inactiveExplodeSlimeExplodeEffectCount;
    private int activeExplodeSlimeExplodeEffectCount;

    private Transform hitEffectPool;
    private Transform explodeSlimeExplodeEffectPool;

    private List<GameObject> inactiveHitEffectGameObjects;
    private List<GameObject> activeHitEffectGameObjects;
    private List<GameObject> inactiveExplodeSlimeExplodeEffectGameObjects;
    private List<GameObject> activeExplodeSlimeExplodeEffectGameObjects;

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

        inactiveHitEffectGameObjects = new List<GameObject>();
        activeHitEffectGameObjects = new List<GameObject>();

        inactiveExplodeSlimeExplodeEffectGameObjects = new List<GameObject>();
        activeExplodeSlimeExplodeEffectGameObjects = new List<GameObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreatePools();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreatePools()
    {
        hitEffectPool = new GameObject("Hit Effect Pool").transform;
        hitEffectPool.SetParent(transform);

        explodeSlimeExplodeEffectPool = new GameObject("Explode Slime Explode Effect Pool").transform;
        explodeSlimeExplodeEffectPool.transform.SetParent(transform);
    }

    #region Hit Effect

    private void PrepareHitEffect()
    {
        if (hitEffectPrefab == null) return;

        for (int i = 0; i < hitEffectPrepare; i++)
        {
            GameObject hitEffect = Object.Instantiate(hitEffectPrefab, hitEffectPool);
            hitEffect.SetActive(false);
            inactiveHitEffectGameObjects.Add(hitEffect);
            inactiveHitEffectCount++;
        }
    }

    public GameObject TakeHitEffect()
    {
        if (inactiveHitEffectCount == 0)
            PrepareHitEffect();

        GameObject hitEffect = inactiveHitEffectGameObjects[0];
        inactiveHitEffectGameObjects.RemoveAt(0);
        inactiveHitEffectCount--;
        hitEffect.transform.SetParent(null);
        hitEffect.SetActive(true);
        return hitEffect;
    }

    public void ReturnHitEffect(GameObject hitEffect)
    {
        hitEffect.gameObject.SetActive(false);
        hitEffect.transform.SetParent(hitEffectPool);
        inactiveHitEffectGameObjects.Add(hitEffect);
        inactiveHitEffectCount++;
    }

    #endregion


    #region Explode Slime's Explode Effect

    private void PrepareExplodeSlimeExplodeEffect()
    {
        if (explodeSlimeExplodeEffectPrefab == null) return;

        for (int i = 0; i < explodeSlimeExplodeEffectPrepare; i++)
        {
            GameObject effect = Object.Instantiate(explodeSlimeExplodeEffectPrefab, explodeSlimeExplodeEffectPool);
            effect.SetActive(false);
            inactiveExplodeSlimeExplodeEffectGameObjects.Add(effect);
            inactiveExplodeSlimeExplodeEffectCount++;
        }
    }
    
    public GameObject TakeExplodeSlimeExplodeEffect(Vector2 pos, float scale)
    {
        if (inactiveExplodeSlimeExplodeEffectCount == 0)
            PrepareExplodeSlimeExplodeEffect();

        GameObject effect = inactiveExplodeSlimeExplodeEffectGameObjects[0];
        inactiveExplodeSlimeExplodeEffectGameObjects.RemoveAt(0);
        inactiveExplodeSlimeExplodeEffectCount--;

        activeExplodeSlimeExplodeEffectGameObjects.Add(effect);
        activeExplodeSlimeExplodeEffectCount++;

        effect.transform.SetParent(null);

        effect.transform.position = pos;
        effect.transform.localScale = new Vector2(scale, scale);
        effect.SetActive(true);
        return effect;
    }

    public void ReturnExplodeSlimeExplodeEffect(GameObject effect)
    {
        effect.SetActive(false);

        effect.transform.SetParent(explodeSlimeExplodeEffectPool);

        activeExplodeSlimeExplodeEffectGameObjects.Remove(effect);
        activeExplodeSlimeExplodeEffectCount--;

        inactiveExplodeSlimeExplodeEffectGameObjects.Add(effect);
        inactiveExplodeSlimeExplodeEffectCount++;
    }

    #endregion

    void PrepareEffects()
    {
        if (inactiveHitEffectCount == 0)
            PrepareHitEffect();

        if (inactiveExplodeSlimeExplodeEffectCount == 0)
            PrepareExplodeSlimeExplodeEffect();
    }

    public void Initialize()
    {
        PrepareEffects();

        ClearAllEffects();

        GameInitializationManager.Instance.animationsPrepared = true;
    }

    private void ClearAllEffects()
    {
        ClearAllHitEffect();
        ClearAllExplodeSlimeExplodeEffect();
    }

    private void ClearAllHitEffect()
    {
        if (activeHitEffectGameObjects != null && activeHitEffectCount > 0)
        {
            foreach (var effect in activeHitEffectGameObjects)
                ReturnHitEffect(effect);

            activeHitEffectGameObjects.Clear();
        }
    }

    private void ClearAllExplodeSlimeExplodeEffect()
    {
        if (activeExplodeSlimeExplodeEffectGameObjects != null && activeExplodeSlimeExplodeEffectCount > 0)
        {
            foreach (var effect in activeExplodeSlimeExplodeEffectGameObjects)
                ReturnExplodeSlimeExplodeEffect(effect);

            activeExplodeSlimeExplodeEffectGameObjects.Clear();
        }
    }
}
