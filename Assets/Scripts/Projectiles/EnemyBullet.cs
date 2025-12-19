using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : BaseProjectile
{
    [SerializeField] private List<InGameObjectType> damageTargetObjectTypes;
    [SerializeField] private int baseDamage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxDistance;
    [SerializeField] private float flySmoothTime = 0.1f;

    private bool isAbleToMove = false;
    private Vector2 moveDirection;
    private int damage;
    private float distanceTraveled;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
        {
            isAbleToMove = false;
            return;
        }
        else if (GameManager.Instance.IsUpgrading())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isAbleToMove) // Moving
        {
            Vector2 targetVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, flySmoothTime);

            distanceTraveled += moveSpeed * Time.deltaTime;
            if (distanceTraveled >= maxDistance)
            {
                ProjectilesManager.Instance.Return(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameState.Playing)
            return;

        if (collision.TryGetComponent<ObjectType>(out ObjectType objectType)
            && damageTargetObjectTypes.Contains(objectType.InGameObjectType)
            && collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            ProjectilesManager.Instance.Return(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Impassible"))
        {
            ProjectilesManager.Instance.Return(this);
        }
    }

    public void Initialize()
    {
        isAbleToMove = true;
        moveDirection = Vector2.zero;
        moveSpeed = 0;
        distanceTraveled = 0;
        maxDistance = 0;
        damage = 0;
    }

    public void Initialize(Vector2 moveDirection, float difficultyMultiplier)
    {
        this.isAbleToMove = true;
        this.moveDirection = moveDirection;
        this.distanceTraveled = 0;
        this.damage = Mathf.RoundToInt(baseDamage * difficultyMultiplier);
    }
}
