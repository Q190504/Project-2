using UnityEngine;

public class RedPig : BaseEnemy
{
    private RedPigHealth health;
    private RedPigMovement movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        health = GetComponent<RedPigHealth>();
        movement = GetComponent<RedPigMovement>();
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
    }
}
