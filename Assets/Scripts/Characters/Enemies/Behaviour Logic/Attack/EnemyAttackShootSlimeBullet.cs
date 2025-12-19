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


    private float difficultyMultiplier = 0;
    private bool isShooting = false;
    private float cooldownTimer = 0;

    public override void DoAnimationTriggerEventLogic(BaseEnemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        enemy.MoveEnemy(Vector2.zero);

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (playerTransform == null || isShooting) return;

        enemy.StartCoroutine(Shoot(bulletCount, delayBetweenBullet, difficultyMultiplier));
    }

    public override void Initialize(GameObject gameObject, BaseEnemy enemy)
    {
        base.Initialize(gameObject, enemy);
        playerTransform = GameManager.Instance.GetPlayerGO().transform;
    }

    #region Shooting Functions

    IEnumerator Shoot(int bulletCount, float delayBetweenBullet, float difficultyMultiplier)
    {
        isShooting = true;

        for (int i = 0; i < bulletCount; i++)
        {
            // Spawn the bullet
            EnemyBullet bullet = ProjectilesManager.Instance.Spawn(bulletPrefab, enemy.transform.position, Quaternion.identity);

            SetBulletStats(bullet, difficultyMultiplier);

            // Wait before spawning the next bullet
            if (delayBetweenBullet > 0f && i < bulletCount - 1)
                yield return new WaitForSeconds(delayBetweenBullet);
        }

        cooldownTimer = cooldownTime; // Reset timer
        isShooting = false;

        enemy.StateMachine.ChangeState(enemy.IdleState);
    }

    private void SetBulletStats(EnemyBullet bullet, float difficultyMultiplier)
    {
        Vector2 playerPosition = playerTransform.position;
        Vector2 moveDirection = math.normalize(playerPosition - new Vector2(bullet.transform.position.x, bullet.transform.position.y));
        bullet.Initialize(moveDirection, difficultyMultiplier);
    }

    public void Initialize(float difficultyMultiplier)
    {
        this.difficultyMultiplier = difficultyMultiplier;
    }

    #endregion
}
