using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SlimeBullet : MonoBehaviour
{
    [SerializeField] private float flySmoothTime = 0.1f;
    [SerializeField] SlimeBulletSlowArea slowArea;
    [SerializeField] private List<InGameObjectType> damageTargetObjectTypes;

    private bool isAbleToMove;
    private bool isBeingSummoned;
    private Vector2 moveDirection;
    private float moveSpeed;
    private float distanceTraveled;
    private float maxDistance;
    private int remainingDamage;
    private float passthroughDamageModifier;
    private float existDuration;

    private Rigidbody2D rb;
    private GameObject player;


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
                StopMoving();
            }
        }
        else if (!isAbleToMove && isBeingSummoned) // Summoned
        {
            player = GameManager.Instance.GetPlayerGO();
            Vector3 playerPos = GameManager.Instance.GetPlayerGO().transform.position;
            Vector3 bulletPosition = transform.position;
            if (player.TryGetComponent<SlimeReclaimSkill>(out SlimeReclaimSkill slimeReclaimSkill))
            {
                Vector3 directionToPlayer = playerPos - bulletPosition;
                float distanceToPlayer = math.length(directionToPlayer);

                if (distanceToPlayer > 0.0001f)
                {
                    Vector3 moveDirection = directionToPlayer / distanceToPlayer;
                    transform.position += slimeReclaimSkill.GetBulletSpeedWhenSummoned() * Time.deltaTime * moveDirection;
                }
            }
        }
        else // Out of life time
        {
            rb.linearVelocity = Vector2.zero;

            if (existDuration <= 0)
                ProjectilesManager.Instance.ReturnSlimeBullet(this);
            else
                existDuration -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameState.Playing)
            return;

        if (collision.TryGetComponent<IDamageable>(out IDamageable damageable)
            && collision.TryGetComponent<ObjectType>(out ObjectType objectType)
            && damageTargetObjectTypes.Contains(objectType.InGameObjectType))
        {
            // Skip if (stopped moving and not being summoned)
            if ((!isAbleToMove && !isBeingSummoned))
                return;

            // Deal damage
            int damage = remainingDamage;

            if (damage <= 0)
                return;

            damageable.TakeDamage(damage);

            // Reduce damage for future hits if the bullet is not being summoned
            if (!isBeingSummoned)
            {
                remainingDamage = (int)(damage * passthroughDamageModifier);
            }

            AudioManager.Instance.PlaySlimeBulletHitSoundSFX();
        }
        else if (collision.TryGetComponent<ObjectType>(out ObjectType objectType1)
            && objectType1.InGameObjectType == InGameObjectType.Player)
        {
            if (!isAbleToMove && isBeingSummoned)
            {
                rb.linearVelocity = Vector2.zero;
                isBeingSummoned = false;
                ProjectilesManager.Instance.ReturnSlimeBullet(this);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Impassible") && !isAbleToMove)
        {
            isAbleToMove = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Initialize()
    {
        isAbleToMove = true;
        moveDirection = Vector2.zero;
        moveSpeed = 0;
        distanceTraveled = 0;
        maxDistance = 0;
        remainingDamage = 0;
        passthroughDamageModifier = 0;
        existDuration = 0;
        isBeingSummoned = false;

        slowArea.Initialize(0, 0);
        slowArea.gameObject.SetActive(false);
    }

    public void Initialize(Vector2 moveDirection, float moveSpeed, float maxDistance, int remainingDamage,
        float passthroughDamageModifier, int healPlayerAmount, float existDuration, float slowModifier, float slowRadius)
    {
        this.isAbleToMove = true;
        this.moveDirection = moveDirection;
        this.moveSpeed = moveSpeed;
        this.distanceTraveled = 0;
        this.maxDistance = maxDistance;
        this.remainingDamage = remainingDamage;
        this.passthroughDamageModifier = passthroughDamageModifier;
        this.existDuration = existDuration;
        this.isBeingSummoned = false;
        slowArea.Initialize(slowRadius, slowModifier);
        slowArea.gameObject.SetActive(false);
    }

    public void StopMoving()
    {
        isAbleToMove = false;
        slowArea.gameObject.SetActive(true);
        ProjectilesManager.Instance.RegisterSlimeBulletsToReclaim(this);
    }

    public void Summon(int damage)
    {
        isBeingSummoned = true;
        slowArea.gameObject.SetActive(false);
        remainingDamage = damage;
        ProjectilesManager.Instance.UnregisterSlimeBulletsToReclaim(this);
    }

    public List<InGameObjectType> GetDamageTargetObjectTypes()
    {
        return damageTargetObjectTypes;
    }
}
