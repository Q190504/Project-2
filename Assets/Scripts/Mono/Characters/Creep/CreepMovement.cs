using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class CreepMovement : MonoBehaviour
{
    [SerializeField] float baseMoveSpeed;
    float moveSpeed;

    float cellSize;
    int mapWidth;

    // Refs
    private Rigidbody2D rb;
    private EffectManager effectManager;
    private GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        effectManager = GetComponent<EffectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            rb.linearVelocity = Vector2.zero;
        else
        {


            #region Apply slow effects
            float targetSpeed = moveSpeed;
            float multiplier = 1f;
            if (effectManager.HasEffect(EffectType.Slow))
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            else if (effectManager.HasEffect(EffectType.Slow))
            {
                List<BaseEffect> slowEffects = effectManager.GetEffectOfType(EffectType.Slow);

                foreach (var effect in slowEffects)
                {
                    multiplier *= 1f - math.clamp(effect.Value, 0f, 1f);
                }
            }

            targetSpeed *= multiplier;

            #endregion

            int x = (int)(transform.position.x / cellSize);
            int y = (int)(transform.position.y / cellSize);
            int index = x + y * mapWidth;

            //if (index >= 0 && index < pathBuffer.Length)
            //{
            //    Vector2 flowDirection = pathBuffer[index].vector;

            //    Vector3 movement = new Vector3(flowDirection.x, flowDirection.y, 0) * targetSpeed;

            //    rb.linearVelocity = movement;
            //}
        }
    }

    public void Initialize(GameObject target)
    {
        this.target = target;

        //grid = gridQuery.GetSingletonEntity();
        //FlowFieldGridDataComponent flowFieldGridDataComponent = state.EntityManager.GetComponentData<FlowFieldGridDataComponent>(grid);
        //DynamicBuffer<GridNode> pathBuffer = state.EntityManager.GetBuffer<GridNode>(grid);
        //mapWidth = flowFieldGridDataComponent.width;
        //cellSize = flowFieldGridDataComponent.nodeSize;
    }
}
