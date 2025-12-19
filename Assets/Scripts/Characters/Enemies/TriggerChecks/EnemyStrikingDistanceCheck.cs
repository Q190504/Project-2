using UnityEngine;

public class EnemyStrikingDistanceCheck : MonoBehaviour
{
    public GameObject PlayerTarget { get; set; }
    private BaseEnemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<BaseEnemy>();
        _enemy.StrikingDistance = this.GetComponent<CircleCollider2D>().radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerTarget)
            _enemy.SetStrikingDistanceBool(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerTarget)
            _enemy.SetStrikingDistanceBool(false);
    }
}
