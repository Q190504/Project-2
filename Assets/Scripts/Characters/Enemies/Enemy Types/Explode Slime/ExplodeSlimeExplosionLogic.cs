using UnityEngine;

public class ExplodeSlimeExplosionLogic : MonoBehaviour
{
    [SerializeField] private BaseVFX effectPrefab;
    [SerializeField] private float detectRadius;
    [SerializeField] private float explodeRadius;
    [SerializeField] private float timeToExplode;
    [SerializeField] private int explodeDamage;
    [SerializeField] private string explodeAnimationClipName;
    [SerializeField] private int segments = 50;

    private bool isExploding;
    private float explodeTimer;
    private InGameObjectType damageTargetObjectType;

    [Header("Explode Warning Flash")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float minFlashInterval = 0.05f;
    [SerializeField] private float maxFlashInterval = 0.3f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float flashTimer;
    private bool flashState;

    private ExplodeSlime explodeSlime;
    private LineRenderer explodeLine;

    private void OnEnable()
    {
        if (gameObject.TryGetComponent(out explodeSlime))
            damageTargetObjectType = explodeSlime.GetObjectTypeCanDamage();

        if (explodeLine == null)
            explodeLine = CreateCircleRenderer(Color.red);

        DrawCircle(explodeLine, explodeRadius);
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
                if (hit.TryGetComponent(out ObjectType objectType)
                    && damageTargetObjectType == objectType.InGameObjectType)
                {
                    isExploding = true;
                    explodeTimer = timeToExplode;
                    return;
                }
            }
        }
        else if (isExploding && explodeTimer > 0)
        {
            explodeTimer -= Time.deltaTime;
            UpdateFlashEffect();
            if (explodeTimer <= 0 && explodeSlime.IsAlive())
                Explode();
        }
    }

    public void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(gameObject.transform.position, explodeRadius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out ObjectType objectType)
                && damageTargetObjectType == objectType.InGameObjectType
                && hit.TryGetComponent(out IDamageable iDamageable))
            {
                iDamageable.TakeDamage(explodeDamage);
            }
        }

        VFXManager.Instance.SpawnVFX(effectPrefab, transform.position, Quaternion.identity, Vector3.one * explodeRadius);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        explodeSlime.Die();
    }

    #region Circle for debug

    //void OnDrawGizmos()
    //{
    //    DrawCircle(transform.position, explodeRadius, Color.red);
    //    DrawCircle(transform.position, detectRadius, Color.blue);
    //}

    //void DrawCircle(Vector3 center, float radius, Color color)
    //{
    //    Gizmos.color = color;
    //    Vector3 lastPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * radius;

    //    for (int i = 1; i <= segments; i++)
    //    {
    //        float angle = i * Mathf.PI * 2f / segments;
    //        Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
    //        Gizmos.DrawLine(lastPoint, nextPoint);
    //        lastPoint = nextPoint;
    //    }
    //}

    #endregion


    #region Circle for game mode

    LineRenderer CreateCircleRenderer(Color color)
    {
        if (!gameObject.TryGetComponent(out LineRenderer lr))
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
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            lr.SetPosition(i, pos);
        }
    }


    #endregion

    private void UpdateFlashEffect()
    {
        if (spriteRenderer == null) return;
        // Flash speed increases as timer goes down
        float t = 1f - (explodeTimer / timeToExplode);

        // Flash the sprite
        float flashInterval = Mathf.Lerp(maxFlashInterval, minFlashInterval, t);

        flashTimer += Time.deltaTime;
        if (flashTimer >= flashInterval)
        {
            flashTimer = 0f;
            flashState = !flashState;
            spriteRenderer.color = flashState ? flashColor : originalColor;
        }
    }


    public void Initialize()
    {
        isExploding = false;
        explodeTimer = 0;
        flashState = false;
        flashTimer = 0;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        spriteRenderer = explodeSlime.SpriteRenderer;
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public bool IsExploding()
    {
        return isExploding;
    }
}
