using UnityEngine;

public class ExplodeSlime : BaseEnemy
{
    private ExplodeSlimeHealth health;
    private ExplodeSlimeMovement movement;
    private ExplodeSlimeExplosionLogic explodeLogic;

    private void OnEnable()
    {
        movement = GetComponent<ExplodeSlimeMovement>();
        health = GetComponent<ExplodeSlimeHealth>();
        explodeLogic = GetComponent<ExplodeSlimeExplosionLogic>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

        explodeLogic.Initialize();
    }
}
