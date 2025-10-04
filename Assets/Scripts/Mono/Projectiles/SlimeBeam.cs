using System.Collections.Generic;
using UnityEngine;

public class SlimeBeam : MonoBehaviour
{
    [SerializeField] private List<InGameObjectType> damageTargetObjectTypes;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 baseAttackSize;

    private int damage;

    private Vector2 CurrentAttackSize => new Vector2(
    baseAttackSize.x * transform.localScale.x,
    baseAttackSize.y * transform.localScale.y);

    public void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, CurrentAttackSize, 0f);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<ObjectType>(out ObjectType objectType)
                && damageTargetObjectTypes.Contains(objectType.InGameObjectType)
                && hit.TryGetComponent<IDamageable>(out IDamageable iDamageable))
            {
                iDamageable.TakeDamage(damage);
            }
        }

        AudioManager.Instance.PlaySlimeBeamSoundSFX();
    }

    public void Initialize(int damage)
    {
        this.damage = damage;
    }

    public void Return()
    {
        ProjectilesManager.Instance.ReturnSlimeBeam(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, CurrentAttackSize);
    }
}
