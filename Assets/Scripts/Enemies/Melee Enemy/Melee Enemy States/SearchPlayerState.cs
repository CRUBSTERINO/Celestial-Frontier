using Cysharp.Threading.Tasks;
using Enemies.States;
using System.Threading;
using UnityEngine;

namespace Enemies.MeleeEnemy.States
{
    public class SearchPlayerState : MeleeEnemyState
    {
        private const float INSPECTING_SEARCH_POINT_TIME = 2f;

        private PathfindingService _pathfindingService;
        private PlayerManager _playerManager;
        private CancellationTokenSource _cancellationTokenSource;
        private Vector3 _playerLastPosition;
        private bool _isPlayerFound;
        private bool _inspectedSearchPoint;

        public SearchPlayerState(MeleeEnemyManager meleeEnemyManager, Vector3 playerLastPosition) : base(meleeEnemyManager)
        {
            _pathfindingService = ServiceLocator.Instance.GetService<PathfindingService>();
            _playerLastPosition = playerLastPosition;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void EnterState()
        {
            EnemyManager.StartLookingForPlayer();

            // Set path to search point
            EnemyManager.Agent.SetDestination(_pathfindingService.Surface.GetClosestSearchPoint(_playerLastPosition).transform.position);

            EnemyManager.PlayerFound += OnPlayerFoundHandler;
            EnemyManager.Agent.OnPathCompleted += OnPathToSearchPointFinishedHandler;
        }

        public override void ExitState()
        {
            EnemyManager.PlayerFound -= OnPlayerFoundHandler;
            EnemyManager.Agent.OnPathCompleted -= OnPathToSearchPointFinishedHandler;

            EnemyManager.StopLookingForPlayer();

            EnemyManager.Agent.ResetPath();
        }

        protected override EnemyState ChangeState()
        {
            if (_isPlayerFound)
            {
                return new PursuePlayerState(EnemyManager, _playerManager);
            }

            if (_inspectedSearchPoint)
            {
                if (EnemyManager.DefaultPath != null)
                {
                    return new PatrolState(EnemyManager, EnemyManager.DefaultPath);
                }
                else
                {
                    return new IdleState(EnemyManager);
                }
            }

            return this;
        }

        protected override void UpdateState()
        {
        }

        private void OnPlayerFoundHandler(PlayerManager playerManager)
        {
            _isPlayerFound = true;
            _playerManager = playerManager;
        }

        private void OnPathToSearchPointFinishedHandler()
        {
            InspectSearchPoint(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask InspectSearchPoint(CancellationToken cancellationToken)
        {
            await UniTask.WaitForSeconds(INSPECTING_SEARCH_POINT_TIME, cancellationToken: cancellationToken);

            _inspectedSearchPoint = true;
        }
    }
}
