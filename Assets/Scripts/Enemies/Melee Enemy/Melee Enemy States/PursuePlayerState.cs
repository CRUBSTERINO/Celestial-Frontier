using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Enemies.MeleeEnemy.States
{
    using Enemies.States;
    using System.Threading;

    public class PursuePlayerState : MeleeEnemyState
    {
        private const float PATH_UPDATE_DELAY = 0.5f;

        private PlayerManager _playerManager;
        private CancellationTokenSource _updatePathToPlayerCancellationToken;
        // Should be set to true, if player hid during pursuit
        private bool _isPlayerHidden;
        // Shoud be set to true, if enemy saw player hiding during pursuit
        private bool _sawPlayerHiding;

        public PursuePlayerState(MeleeEnemyManager manager, PlayerManager playerManager) : base(manager)
        {
            _playerManager = playerManager;
            _updatePathToPlayerCancellationToken = new CancellationTokenSource();
        }

        public override void EnterState()
        {
            UpdatePathToPlayer(_updatePathToPlayerCancellationToken.Token).Forget();

            _isPlayerHidden = false;
            _sawPlayerHiding = false;

            EnemyManager.SetViewDistance(EnemyManager.InPursueViewDistance);

            _playerManager.OnHidden += OnPlayerHiddenHandler;
        }

        public override void ExitState()
        {
            _updatePathToPlayerCancellationToken.Cancel();

            EnemyManager.Agent.ResetPath();

            EnemyManager.SetViewDistance(EnemyManager.DefaultViewDistance);

            _playerManager.OnHidden -= OnPlayerHiddenHandler;
        }

        protected override EnemyState ChangeState()
        {
            float distanceToPlayer = (_playerManager.transform.position - EnemyManager.transform.position).magnitude;

            // Befindet sich der Spieler innerhalb des Angriffsradius, geht der Gegner in den Angriffszustand über.
            if (distanceToPlayer <= EnemyManager.AttackDistance)
            {
                return new AttackPlayerState(EnemyManager, _playerManager);
            }

            if (_isPlayerHidden)
            {
                if (_sawPlayerHiding)
                {
                    return new AttackPlayerInHideoutState(EnemyManager, _playerManager);
                }
                else
                {
                    return new SearchPlayerState(EnemyManager, _playerManager.transform.position);
                }
            }

            // Andernfalls bleibt der Feind in einem Zustand der Verfolgung
            return this;
        }

        protected override void UpdateState()
        {

        }

        private async UniTask UpdatePathToPlayer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_playerManager != null)
                {
                    EnemyManager.Agent.SetDestination(_playerManager.transform.position);

                    await UniTask.WaitForSeconds(PATH_UPDATE_DELAY);
                }
                else
                {
                    break;
                }
            }
        }

        private void OnPlayerHiddenHandler()
        {
            _isPlayerHidden = true;
            _sawPlayerHiding = EnemyManager.IsPlayerInView(_playerManager, _playerManager.SpriteRenderer.bounds, true);
        }
    }
}