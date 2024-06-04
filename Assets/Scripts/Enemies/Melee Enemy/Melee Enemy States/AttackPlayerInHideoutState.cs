using UnityEngine;

namespace Enemies.MeleeEnemy.States
{
    using Enemies.States;

    public class AttackPlayerInHideoutState : MeleeEnemyState
    {
        private PlayerManager _playerManager;
        private bool _attackedPlayer;

        public AttackPlayerInHideoutState(MeleeEnemyManager meleeEnemyManager, PlayerManager playerManager) : base(meleeEnemyManager)
        {
            _playerManager = playerManager;
        }

        public override void EnterState()
        {
            EnemyManager.Agent.SetDestination(_playerManager.transform.position);

            EnemyManager.IsForcedToSeePlayer = true;

            EnemyManager.Agent.OnPathCompleted += AttackPlayerOnPathFinishedHandler;
        }

        public override void ExitState()
        {
            EnemyManager.Agent.ResetPath();

            EnemyManager.IsForcedToSeePlayer = false;

            EnemyManager.Agent.OnPathCompleted -= AttackPlayerOnPathFinishedHandler;
        }

        protected override EnemyState ChangeState()
        {
            if (_attackedPlayer)
            {
                return new IdleState(EnemyManager);
            }

            return this;
        }

        protected override void UpdateState()
        {
            /*if (_playerManager == null) return;

            float distanceToPlayer = (_playerManager.transform.position - EnemyManager.transform.position).magnitude;

            if (distanceToPlayer <= EnemyManager.AttackDistance)
            {
                _playerManager.GetAttacked();
            }*/
        }

        private void AttackPlayerOnPathFinishedHandler()
        {
            _playerManager.GetAttacked();
        }
    }
}
