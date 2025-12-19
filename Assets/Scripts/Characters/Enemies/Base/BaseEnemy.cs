using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IEnemyMoveable, ITriggerCheckable
{
    [SerializeField] protected InGameObjectType objectTypeCanDamage;
    [SerializeField] protected MoveDirectionType moveDirection;
    [SerializeField] protected int baseSpike;
    protected int spike;

    [SerializeField] protected int baseMaxHealth;
    protected int maxHealth;
    protected int currentHealth;

    [SerializeField] protected BaseVFX hitEffectPrefab;

    protected float strikingDistance;
    public float StrikingDistance { get; set; }
    public bool CanAttack { get; set; }
    public bool IsWithinStrikingDistance { get; set; } = true;


    public int BaseMaxHealth => baseMaxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public Rigidbody2D RB { get; set; }
    public Animator Animator { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }
    public EffectManager EffectManager { get; set; }
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

    [SerializeField] private EnemyIdleSOBase EnemyIdleBase;
    [SerializeField] private EnemyChaseSOBase EnemyChaseBase;
    [SerializeField] private EnemyAttackSOBase EnemyAttackBase;
    [SerializeField] private EnemySkill1SOBase EnemySkill1Base;
    [SerializeField] private EnemySkill2SOBase EnemySkill2Base;

    public EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
    public EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }
    public EnemySkill1SOBase EnemySkill1BaseInstance { get; set; }
    public EnemySkill2SOBase EnemySkill2BaseInstance { get; set; }

    #endregion

    private void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
        EnemyChaseBaseInstance = Instantiate(EnemyChaseBase);
        EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);
        EnemySkill1BaseInstance = Instantiate(EnemySkill1Base);
        EnemySkill2BaseInstance = Instantiate(EnemySkill2Base);

        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        Skill1State = new EnemySkill1State(this, StateMachine);
        Skill2State = new EnemySkill2State(this, StateMachine);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        if (SpriteRenderer == null)
        {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (SpriteRenderer == null)
                Debug.LogError($"Cant find SpriteRenderer of {gameObject.name};");
        }

        EffectManager = GetComponent<EffectManager>();

        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this);
        EnemyAttackBaseInstance.Initialize(gameObject, this);
        EnemySkill1BaseInstance.Initialize(gameObject, this);
        EnemySkill2BaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
        SetAnimation();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (collision.TryGetComponent(out ObjectType objectType)
            && objectType.InGameObjectType == objectTypeCanDamage
            && collision.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(spike);
    }

    public abstract void Initialize(float difficultyMultiplier);

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

    public abstract void Die();

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
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
        else if (!IsFacingRight && velovity.x > 0f)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
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
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
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
