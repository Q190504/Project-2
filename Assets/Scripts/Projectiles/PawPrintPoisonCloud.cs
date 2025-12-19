using UnityEngine;

public class PawPrintPoisonCloud : PoisonCloud
{
    int totalEnemiesCurrentlyInTheCloud;

    [SerializeField] PoisonCloudPublisherSO onCloudReturn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckExistTime();

        DealDamge();
    }

    protected override void CheckExistTime()
    {
        existDurationTimer -= Time.deltaTime;
        if (existDurationTimer <= 0)
        {
            onCloudReturn?.RaiseEvent(this);
            ReturnCloud();
        }
    }

    protected override void ReturnCloud()
    {
        ProjectilesManager.Instance.Return(this);
    }

    public int GetTotalEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, cloudRadius / 2);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out ObjectType objectType)
                && damageTargetObjectTypes.Contains(objectType.InGameObjectType))
                totalEnemiesCurrentlyInTheCloud++;
        }

        return totalEnemiesCurrentlyInTheCloud;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying()) return;

        // Set canSpawnNewCloud = false to Paw Print Poisoner Weapon
        if (collision.TryGetComponent(out ObjectType objectType)
            && objectType.InGameObjectType == InGameObjectType.Player)
        {
            if (collision.TryGetComponent(out PawPrintPoisonerWeapon pawPrintPoisonerWeapon))
                pawPrintPoisonerWeapon.SetCanSpawnNewCloud(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying()) return;

        // Set canSpawnNewCloud = true to Paw Print Poisoner Weapon
        if (collision.TryGetComponent(out ObjectType objectType)
            && objectType.InGameObjectType == InGameObjectType.Player)
        {
            if (collision.TryGetComponent(out PawPrintPoisonerWeapon pawPrintPoisonerWeapon))
                pawPrintPoisonerWeapon.SetCanSpawnNewCloud(true);
        }
    }

    public override void Initialize(float tick, int damagePerTick, float cloudRadius, float maximumCloudDuration)
    {
        base.Initialize(tick, damagePerTick, cloudRadius, maximumCloudDuration);
        totalEnemiesCurrentlyInTheCloud = 0;
    }
}
