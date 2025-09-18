using UnityEngine;

public class SlimeBeam : MonoBehaviour
{
    private int damage;
    private bool canDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDamage && collision == null)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && !collision.CompareTag("Player"))
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    public void Initialize(int damage)
    {
        this.damage = damage;
        canDamage = false;
    }

    public void CanDamage()
    {
        this.canDamage = true;
    }

    public void Return()
    {
        canDamage = false;
        ProjectilesManager.Instance.ReturnSlimeBeam(this);
    }
}
