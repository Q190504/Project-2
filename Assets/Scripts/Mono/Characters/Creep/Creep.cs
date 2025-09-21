using UnityEngine;

public class Creep : MonoBehaviour
{
    [SerializeField] private InGameObjectType objectTypeCanDamage;
    [SerializeField] private int baseSpike;
    private int spike;

    private CreepHealth health;
    private CreepMovement movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        health = GetComponent<CreepHealth>();
        movement = GetComponent<CreepMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (collision.TryGetComponent<ObjectType>(out ObjectType objectType)
            && objectType.InGameObjectType == objectTypeCanDamage
            && collision.TryGetComponent<IDamageable>(out IDamageable damageable))
            damageable.TakeDamage(spike);
    }

    public void Initialize(Vector2 pos, GameObject target, float difficultyMultiplier)
    {
        transform.position = pos;

        movement.Initialize(target);

        int enemyHP = (int)(health.BaseMaxHealth + difficultyMultiplier);
        health.Initialize(enemyHP);

        int enemySpike = (int)(baseSpike + difficultyMultiplier);
        spike = enemySpike;
    }
}
