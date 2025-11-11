using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShootingSlimeMovement : MonoBehaviour
{
    [SerializeField] float baseMoveSpeed;
    float moveSpeed;

    float nodeSize;
    int mapWidth;

    // Refs
    private ShootingLogic shootingLogic;
    private Rigidbody2D rb;
    private EffectManager effectManager;
    private FlowFieldManager flowFieldManager;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    float targetSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        effectManager = GetComponent<EffectManager>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shootingLogic = GetComponentInChildren<ShootingLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        else
        {
            #region Apply effects

            targetSpeed = moveSpeed;
            float multiplier = 1f;
            if (effectManager.HasEffect(EffectType.Stun))
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (effectManager.HasEffect(EffectType.Slow))
            {
                List<BaseEffect> slowEffects = effectManager.GetEffectOfType(EffectType.Slow);

                foreach (var effect in slowEffects)
                {
                    multiplier *= 1f - math.clamp(effect.Value, 0f, 1f);
                }
            }

            targetSpeed *= multiplier;

            #endregion

            int x = (int)(transform.position.x / nodeSize);
            int y = (int)(transform.position.y / nodeSize);
            int index = x + y * mapWidth;

            Vector2 flowDirection = flowFieldManager.GetDirectionFromIndex(index);
            Vector2 movement = new Vector2(flowDirection.x, flowDirection.y) * targetSpeed;

            #region Animation

            animator.SetFloat("x", flowDirection.x);
            animator.SetFloat("y", flowDirection.y);

            spriteRenderer.flipX = flowDirection.x < 0;

            #endregion

            if (shootingLogic.IsShooting() || shootingLogic.IsTargetInRange())
                rb.linearVelocity = Vector2.zero;
            else
                rb.linearVelocity = movement;
        }
    }

    public void Initialize()
    {
        moveSpeed = baseMoveSpeed;
        flowFieldManager = FlowFieldManager.Instance;
        mapWidth = flowFieldManager.GetMapWidth();
        nodeSize = flowFieldManager.GetNodeSize();
    }
}
