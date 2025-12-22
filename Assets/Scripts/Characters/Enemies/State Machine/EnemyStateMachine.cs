using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState CurrentEnemyState { get; set; }

    public void Initialize(BaseEnemy enemy, EnemyState startingState)
    {
        CurrentEnemyState = startingState;
        CurrentEnemyState.EnterState(enemy);
    }

    public void ChangeState(BaseEnemy enemy, EnemyState newState)
    {
        CurrentEnemyState.ExitState(enemy);
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState(enemy);

    }
}
