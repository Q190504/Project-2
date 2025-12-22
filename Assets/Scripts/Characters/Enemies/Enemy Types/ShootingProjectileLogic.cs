using UnityEngine;

public class ShootingProjectileLogic : MonoBehaviour
{
    [HideInInspector]
    public float difficultyMultiplier = 0;
    [HideInInspector]
    public bool isShooting = false;
    [HideInInspector]
    public float cooldownTimer = 0;

    private void Update()
    {
        if (cooldownTimer > 0 && !isShooting)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
