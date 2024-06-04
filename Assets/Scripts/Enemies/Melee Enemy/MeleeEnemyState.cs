namespace Enemies.MeleeEnemy.States
{
    using Enemies.States;

    public abstract class MeleeEnemyState : EnemyState
    {
        // EnemyManager is always (should be) MeleeEnemyManager (parameter in constructor), so we can use hiding
        protected new MeleeEnemyManager EnemyManager => base.EnemyManager as MeleeEnemyManager;
        
        public MeleeEnemyState(MeleeEnemyManager meleeEnemyManager) : base(meleeEnemyManager)
        {

        }
    }
}
