using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Shoot Slime Bullet", menuName = "Scriptable Objects/Enemy/Attack Logic/Shoot Slime Bullet")]

public class EnemyAttackShootSlimeBullet : EnemyAttackSOBase
{
    [Header("Stats")]
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private InGameObjectType targetObjectType = InGameObjectType.Player;
    [SerializeField] private float cooldownTime = 2;
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float delayBetweenBullet = 0.5f;

    public override void DoAnimationTriggerEventLogic(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void DoFrameUpdateLogic(BaseEnemy enemy)
    {
        base.DoFrameUpdateLogic(enemy);

        enemy.MoveEnemy(Vector2.zero);

        if (enemy.TryGetComponent(out ShootingProjectileLogic shootingProjectileLogic))
        {
            if (shootingProjectileLogic.cooldownTimer > 0)
                return;

            if (enemy.PlayerTransform == null || shootingProjectileLogic.isShooting) return;

            enemy.StartCoroutine(Shoot(enemy, bulletCount, delayBetweenBullet, shootingProjectileLogic.difficultyMultiplier));
        }
    }

    #region Shooting Functions

    IEnumerator Shoot(BaseEnemy enemy, int bulletCount, float delayBetweenBullet, float difficultyMultiplier)
    {
        if (enemy.TryGetComponent(out ShootingProjectileLogic shootingProjectileLogic))
        {
            shootingProjectileLogic.isShooting = true;

            for (int i = 0; i < bulletCount; i++)
            {
                // Spawn the bullet
                EnemyBullet bullet = ProjectilesManager.Instance.Spawn(bulletPrefab, enemy.transform.position, Quaternion.identity);

                SetBulletStats(bullet, enemy.PlayerTransform.position, difficultyMultiplier);

                // Wait before spawning the next bullet
                if (delayBetweenBullet > 0f && i < bulletCount - 1)
                    yield return new WaitForSeconds(delayBetweenBullet);
            }

            shootingProjectileLogic.cooldownTimer = cooldownTime; // Reset timer
            shootingProjectileLogic.isShooting = false;

            enemy.StateMachine.ChangeState(enemy, enemy.IdleState);
        }
    }

    private void SetBulletStats(EnemyBullet bullet, Vector2 playerPosition, float difficultyMultiplier)
    {
        Vector2 moveDirection = math.normalize(playerPosition - new Vector2(bullet.transform.position.x, bullet.transform.position.y));
        bullet.Initialize(moveDirection, difficultyMultiplier);
    }

    public void Initialize(BaseEnemy enemy, float difficultyMultiplier)
    {
        if (enemy.TryGetComponent(out ShootingProjectileLogic shootingProjectileLogic))
            shootingProjectileLogic.difficultyMultiplier = difficultyMultiplier;
    }

    #endregion
}
