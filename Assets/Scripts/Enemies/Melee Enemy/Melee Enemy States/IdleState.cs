using UnityEngine;

namespace Enemies.MeleeEnemy.States
{
    using Enemies.States;

    public class IdleState : MeleeEnemyState
    {
        public IdleState(MeleeEnemyManager manager) : base(manager)
        {

        }

        public override void EnterState()
        {
            EnemyManager.StartLookingForPlayer();
        }

        public override void ExitState()
        {
            EnemyManager.StopLookingForPlayer();
        }

        protected override EnemyState ChangeState()
        {
            return this;
        }

        protected override void UpdateState()
        {

        }
    }
}
