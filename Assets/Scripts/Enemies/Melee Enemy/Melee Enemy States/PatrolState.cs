using Cysharp.Threading.Tasks;
using Enemies.States;
using System.Threading;
using UnityEngine;

namespace Enemies.MeleeEnemy.States
{
    public class PatrolState : MeleeEnemyState
    {
        private ConfigurablePath _path;
        private PathCheckpoint _previousCheckPoint;
        private PathCheckpoint _currentCheckPoint;
        private bool _hasInspectedCheckpoint;
        private bool _isInspectingCheckpoint;
        private bool _isPlayerFound;
        private CancellationTokenSource _inspectingCancellationTokenSource;
        private PlayerManager _playerManager;

        public ConfigurablePath Path { get { return _path; } }

        public PatrolState(MeleeEnemyManager manager, ConfigurablePath path) : base(manager)
        {
            _inspectingCancellationTokenSource = new CancellationTokenSource();
            _path = path;
            _previousCheckPoint = null;
            _currentCheckPoint = null;
        }

        public override void EnterState()
        {
            SetSpecificCheckpointToAgent(_path.GetClosestCheckpoint(EnemyManager.transform.position));

            EnemyManager.StartLookingForPlayer();

            _hasInspectedCheckpoint = true;
            _isInspectingCheckpoint = false;

            EnemyManager.PlayerFound += OnPlayerFoundHandler;
        }

        public override void ExitState()
        {
            EnemyManager.PlayerFound -= OnPlayerFoundHandler;

            EnemyManager.StopLookingForPlayer();

            EnemyManager.Agent.ResetPath();

            if (!_inspectingCancellationTokenSource.IsCancellationRequested)
            {
                _inspectingCancellationTokenSource?.Cancel();
            }
        }

        protected override EnemyState ChangeState()
        {
            if (_isPlayerFound)
            {
                return new PursuePlayerState(EnemyManager, _playerManager);
            }

            return this;
        }

        protected override void UpdateState()
        {
            if (!EnemyManager.Agent.HasPath)
            {
                if (_hasInspectedCheckpoint)
                {
                    SetNextCheckpointToAgent();
                }
                else
                {
                    if (!_isInspectingCheckpoint)
                    {
                        if (_currentCheckPoint.InspectingTime > 0)
                        {
                            _inspectingCancellationTokenSource = new CancellationTokenSource();

                            InspectCurrentCheckpoint(_inspectingCancellationTokenSource.Token).Forget();
                        }
                        else
                        {
                            _hasInspectedCheckpoint = true;
                        }
                    }
                }
            }
        }

        private void SetNextCheckpointToAgent()
        {
            PathCheckpoint nextCheckpoint;
            nextCheckpoint = _path.GetNextCheckpoint(_previousCheckPoint, _currentCheckPoint);
            EnemyManager.Agent.SetDestination(nextCheckpoint.Position);

            _previousCheckPoint = _currentCheckPoint;
            _currentCheckPoint = nextCheckpoint;
            _hasInspectedCheckpoint = false;
        }

        private void SetSpecificCheckpointToAgent(PathCheckpoint checkpoint)
        {
            EnemyManager.Agent.SetDestination(checkpoint.Position);
            _previousCheckPoint = null;
            _currentCheckPoint = checkpoint;
        }

        private async UniTask InspectCurrentCheckpoint(CancellationToken cancellationToken)
        {
            _isInspectingCheckpoint = true;

            await UniTask.WaitForSeconds(_currentCheckPoint.InspectingTime, cancellationToken: cancellationToken);

            _hasInspectedCheckpoint = true;
            _isInspectingCheckpoint = false;
        }

        private void OnPlayerFoundHandler(PlayerManager playerManager)
        {
            _isPlayerFound = true;
            _playerManager = playerManager;
        }
    } 
}
