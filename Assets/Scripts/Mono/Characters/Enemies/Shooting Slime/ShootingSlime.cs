using UnityEngine;

public class ShootingSlime : BaseEnemy
{
    private BaseEnemyHealth health;
    private ShootingSlimeMovement movement;
    private ShootingLogic shootingLogic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        health = GetComponent<BaseEnemyHealth>();
        movement = GetComponent<ShootingSlimeMovement>();
        shootingLogic = GetComponentInChildren<ShootingLogic>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Initialize(Vector2 pos, float difficultyMultiplier)
    {
        transform.position = pos;

        movement.Initialize();

        int enemyHP = (int)(health.BaseMaxHealth + difficultyMultiplier);
        health.Initialize(enemyHP);

        int enemySpike = (int)(baseSpike + difficultyMultiplier);
        spike = enemySpike;

        shootingLogic.Initialize(difficultyMultiplier);
    }
}
