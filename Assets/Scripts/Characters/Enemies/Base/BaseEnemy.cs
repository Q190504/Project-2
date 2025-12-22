using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IEnemyMoveable, ITriggerCheckable
{
    [SerializeField] protected InGameObjectType objectTypeCanDamage;
    [SerializeField] protected MoveDirectionType moveDirection;

    [Header("Spike Damage")]
    [SerializeField] protected int baseSpike;
    protected int spike;

    [Header("Health")]
    [SerializeField] protected int baseMaxHealth;
    protected int maxHealth;
    protected int currentHealth;

    [Header("Move Variables")]
    [HideInInspector]
    public float nodeSize;
    [HideInInspector]
    public int mapWidth;

    [SerializeField] protected BaseVFX hitEffectPrefab;

    protected float strikingDistance;
    public float StrikingDistance { get; set; }
    public bool CanAttack { get; set; }
    public bool IsWithinStrikingDistance { get; set; } = true;

    public int BaseMaxHealth => baseMaxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    // Refs
    public Rigidbody2D RB { get; set; }
    public Animator Animator { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }
    public EffectManager EffectManager { get; set; }
    public FlowFieldManager FlowFieldManager { get; set; }
    public Transform PlayerTransform { get; set; }
    public bool IsFacingRight { get; set; } = true;

    #region Striking Distance Debug Variables

    private LineRenderer rangeLineRenderer;
    [SerializeField] private bool showRangeDebug = false;
    [SerializeField] private int debugSegments = 50;

    #endregion

    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemySkill1State Skill1State { get; set; }
    public EnemySkill2State Skill2State { get; set; }

    #endregion

    #region Scriptable Object Variables

    public EnemyIdleSOBase EnemyIdleBase;
    public EnemyChaseSOBase EnemyChaseBase;
    public EnemyAttackSOBase EnemyAttackBase;
    public EnemySkill1SOBase EnemySkill1Base;
    public EnemySkill2SOBase EnemySkill2Base;

    #endregion

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState();
        ChaseState = new EnemyChaseState();
        AttackState = new EnemyAttackState();
        Skill1State = new EnemySkill1State();
        Skill2State = new EnemySkill2State();
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate(this);
        SetAnimation();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate(this);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (collision.TryGetComponent(out ObjectType objectType)
            && objectType.InGameObjectType == objectTypeCanDamage
            && collision.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(spike);
    }

    public virtual void Initialize(Transform playerTransform, float difficultyMultiplier)
    {
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        if (Animator == null)
        {
            Animator = GetComponentInChildren<Animator>();
            if (Animator == null)
                Debug.LogError($"Can't find Animator in {gameObject.name}");
        }

        SpriteRenderer = GetComponent<SpriteRenderer>();
        if (SpriteRenderer == null)
        {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (SpriteRenderer == null)
                Debug.LogError($"Cant find SpriteRenderer of {gameObject.name};");
        }

        EffectManager = GetComponent<EffectManager>();

        PlayerTransform = playerTransform;

        int enemyHP = (int)(BaseMaxHealth + difficultyMultiplier);
        currentHealth = enemyHP;

        int enemySpike = (int)(baseSpike + difficultyMultiplier);
        spike = enemySpike;

        StateMachine.Initialize(this, ChaseState);
    }

    public InGameObjectType GetObjectTypeCanDamage()
    {
        return objectTypeCanDamage;
    }

    #region Health / Die Fuctions

    public void ClearEffects()
    {
        if (TryGetComponent(out EffectManager effectManager))
            effectManager.ClearAllEffects();
    }

    public virtual void TakeDamage(int amount)
    {
        // Create Hit effect
        VFXManager.Instance.SpawnVFX(hitEffectPrefab, transform.position, Quaternion.identity, Vector3.one);

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            GameManager.Instance.AddEnemyKilled();

            // Try to spawn XP orb
            ExperienceOrbManager.Instance.TrySpawnExperienceOrb(transform.position);

            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public virtual void Die()
    {
        EnemyManager.Instance.ReturnEnemy(this);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    #endregion

    #region Movement Fuctions

    public void MoveEnemy(Vector2 velovity)
    {
        RB.linearVelocity = velovity;
        CheckForLeftOrRightFacing(velovity);
    }

    public void CheckForLeftOrRightFacing(Vector2 velovity)
    {
        if (IsFacingRight && velovity.x < 0f)
        {
            //Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            //transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
        else if (!IsFacingRight && velovity.x > 0f)
        {
            //Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            //transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
    }

    #endregion

    #region Animation Triggers

    private void SetAnimation()
    {
        Vector2 moveDir = Utility.GetDirection(moveDirection, RB.linearVelocity);
        Animator.SetFloat("x", moveDir.x);
        Animator.SetFloat("y", moveDir.y);
        SpriteRenderer.flipX = !IsFacingRight;
    }

    public enum AnimationTriggerType
    {
        StopMove,
        Move,
        Shooting,
    }

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(this, triggerType);
    }

    #endregion

    #region Distance Checks

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }

    #endregion

    #region Circle for game mode

    protected LineRenderer CreateCircleRenderer(Color color)
    {
        if (!gameObject.TryGetComponent<LineRenderer>(out LineRenderer lr))
            lr = new GameObject("Circle").AddComponent<LineRenderer>();

        lr.transform.SetParent(transform);
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = debugSegments;

        // Visual settings
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;

        return lr;
    }

    protected void DrawCircle(LineRenderer lr, float radius)
    {
        for (int i = 0; i < debugSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / debugSegments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    #endregion
}
