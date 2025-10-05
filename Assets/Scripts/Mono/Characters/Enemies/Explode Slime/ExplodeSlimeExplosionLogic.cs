using UnityEngine;

public class ExplodeSlimeExplosionLogic : MonoBehaviour
{
    [SerializeField] private float detectRadius;
    [SerializeField] private float explodeRadius;
    [SerializeField] private float timeToExplode;
    [SerializeField] private int explodeDamage;
    [SerializeField] private string explodeAnimationClipName;
    [SerializeField] private int segments = 50;

    private bool isExploding;
    private float explodeTimer;
    private InGameObjectType damageTargetObjectType;

    private Animator animator;
    private ExplodeSlimeHealth health;
    private LineRenderer explodeLine;

    private void OnEnable()
    {
        if (gameObject.TryGetComponent<ExplodeSlime>(out ExplodeSlime explodeSlime))
            damageTargetObjectType = explodeSlime.GetObjectTypeCanDamage();

        animator = GetComponent<Animator>();
        health = GetComponent<ExplodeSlimeHealth>();

        explodeLine = CreateCircleRenderer(Color.red);
        DrawCircle(explodeLine, explodeRadius);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (!isExploding)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(gameObject.transform.position, detectRadius);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<ObjectType>(out ObjectType objectType)
                    && damageTargetObjectType == objectType.InGameObjectType)
                {
                    isExploding = true;
                    explodeTimer = timeToExplode;
                    PlayExplodeAnimation();
                    return;
                }
            }
        }
        else if (isExploding && explodeTimer > 0)
        {
            explodeTimer -= Time.deltaTime;
            if (explodeTimer <= 0 && health.IsAlive())
                Explode();
        }
    }

    public void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(gameObject.transform.position, explodeRadius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<ObjectType>(out ObjectType objectType)
                && damageTargetObjectType == objectType.InGameObjectType
                && hit.TryGetComponent<IDamageable>(out IDamageable iDamageable))
            {
                iDamageable.TakeDamage(explodeDamage);
            }
        }

        AnimationManager.Instance.TakeExplodeSlimeExplodeEffect(transform.position, explodeRadius);

        health.Die();
    }

    #region Circle for debug

    void OnDrawGizmos()
    {
        DrawCircle(transform.position, explodeRadius, Color.red);
        DrawCircle(transform.position, detectRadius, Color.blue);
    }

    void DrawCircle(Vector3 center, float radius, Color color)
    {
        Gizmos.color = color;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    #endregion


    #region Circle for game mode

    LineRenderer CreateCircleRenderer(Color color)
    {
        if (!gameObject.TryGetComponent<LineRenderer>(out LineRenderer lr))
            lr = new GameObject("Circle").AddComponent<LineRenderer>();

        lr.transform.SetParent(transform);
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = segments;

        // Visual settings
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;

        return lr;
    }

    void DrawCircle(LineRenderer lr, float radius)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    #endregion

    public void Initialize()
    {
        isExploding = false;
        explodeTimer = 0;
    }

    #region Explode Animation

    private void PlayExplodeAnimation()
    {
        AnimationClip clip = GetClip(explodeAnimationClipName);
        if (clip != null)
        {
            float speed = clip.length / timeToExplode;
            animator.SetFloat("SpeedMultiplier", speed);
            animator.SetTrigger("explode");
        }
    }

    private AnimationClip GetClip(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip;
        }

        return null;
    }

    #endregion

    public bool IsExploding()
    {
        return isExploding;
    }
}
