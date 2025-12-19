using UnityEngine;

public class EnemyPoisonCloud : PoisonCloud
{
    [SerializeField] int baseDamagePerTick = 1;

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
            ReturnCloud();
        }
    }


    protected override void ReturnCloud()
    {
        ProjectilesManager.Instance.Return(this);
    }

    public void Initialize(float difficultyMultiplier)
    {
        this.damagePerTick = Mathf.FloorToInt(baseDamagePerTick * difficultyMultiplier);
    }
}
