using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected InGameObjectType objectTypeCanDamage;
    [SerializeField] protected int baseSpike;
    protected int spike;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (collision.TryGetComponent<ObjectType>(out ObjectType objectType)
            && objectType.InGameObjectType == objectTypeCanDamage
            && collision.TryGetComponent<IDamageable>(out IDamageable damageable))
            damageable.TakeDamage(spike);
    }

    public abstract void Initialize(Vector2 pos, float difficultyMultiplier);

    public InGameObjectType GetObjectTypeCanDamage()
    {
        return objectTypeCanDamage;
    }
}
