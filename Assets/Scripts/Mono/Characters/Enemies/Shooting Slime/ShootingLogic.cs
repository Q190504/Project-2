using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ShootingLogic : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private InGameObjectType targetObjectType = InGameObjectType.Player;
    [SerializeField] private float cooldownTime = 2;
    [SerializeField] private float shootingRange = 10;
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float delayBetweenBullet = 0.5f;
    [SerializeField] private bool showRangeDebug = false;
    [SerializeField] private int debugSegments = 50;

    [Header("Refs")]
    [SerializeField] private Animator animator;

    private float difficultyMultiplier = 0;
    private bool isShooting = false;
    private float cooldownTimer = 0;

    private GameObject target;

    private LineRenderer rangeLineRenderer;
    private CircleCollider2D circleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        if (showRangeDebug && rangeLineRenderer != null)
        {
            rangeLineRenderer = CreateCircleRenderer(Color.magenta);
            DrawCircle(rangeLineRenderer, shootingRange);
        }

        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
            circleCollider.radius = shootingRange;
        else
            Debug.LogError("Cant find Range's Circle Collider in Shooting Slime!");
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (target == null || isShooting) return;

        StartCoroutine(Shoot(bulletCount, delayBetweenBullet, difficultyMultiplier));
    }

    IEnumerator Shoot(int bulletCount, float delayBetweenBullet, float difficultyMultiplier)
    {
        isShooting = true;

        for (int i = 0; i < bulletCount; i++)
        {
            animator.SetTrigger("shooting");

            // Spawn the bullet
            EnemyBullet bullet = ProjectilesManager.Instance.TakeEnemyBullet();

            SetBulletStats(bullet, difficultyMultiplier);

            // Wait before spawning the next bullet
            if (delayBetweenBullet > 0f && i < bulletCount - 1)
                yield return new WaitForSeconds(delayBetweenBullet);
        }

        cooldownTimer = cooldownTime; // Reset timer
        isShooting = false;
    }

    private void SetBulletStats(EnemyBullet bullet, float difficultyMultiplier)
    {
        Vector2 playerPosition = target.transform.position;
        bullet.transform.position = transform.position;
        Vector2 moveDirection = math.normalize(playerPosition - new Vector2(bullet.transform.position.x, bullet.transform.position.y));
        bullet.Initialize(moveDirection, difficultyMultiplier);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (target == null && collision.TryGetComponent(out ObjectType objectType)
            && objectType.InGameObjectType == targetObjectType)
        {
            target = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (target != null && collision.TryGetComponent(out ObjectType objectType)
            && objectType.InGameObjectType == targetObjectType)
        {
            target = null;
        }
    }

    #region Circle for game mode

    LineRenderer CreateCircleRenderer(Color color)
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

    void DrawCircle(LineRenderer lr, float radius)
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

    public void Initialize(float difficultyMultiplier)
    {
        this.difficultyMultiplier = difficultyMultiplier;
    }

    public bool IsShooting()
    {
        return isShooting;
    }

    public bool IsTargetInRange()
    {
        return target != null;
    }
}
