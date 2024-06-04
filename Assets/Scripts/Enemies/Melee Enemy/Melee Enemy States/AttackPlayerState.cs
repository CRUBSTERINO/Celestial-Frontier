namespace Enemies.MeleeEnemy.States
{
    using Enemies.States;


    public class AttackPlayerState : MeleeEnemyState
    {
        PlayerManager _playerManager;

        public AttackPlayerState(MeleeEnemyManager meleeEnemyManager, PlayerManager playerManager) : base(meleeEnemyManager)
        {
            _playerManager = playerManager;
        }

        public override void EnterState()
        {
            _playerManager.GetAttacked();
        }

        public override void ExitState()
        {
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
