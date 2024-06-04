using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Enemies.MeleeEnemy
{
    using States;
    using Enemies.States;
    using System;

    public class MeleeEnemyManager : EnemyManager
    {
        [SerializeField] private PathfindingAgent _agent;
        
        [SerializeField] private float _playerDetectionTime;
        [SerializeField] private ConfigurablePath _defaultPath;
        [Space]
        [SerializeField] private float _attackDistance;
        [Space] // FOV
        [SerializeField] private float _fov;
        [SerializeField] private LayerMask _raycastLayerMask;
        [SerializeField] private int _raysToPlayer;
        [SerializeField] private float _defaultViewDistance;
        [SerializeField] private float _inPursueViewDistance;
        [SerializeField] private Transform _eyeOrigin;

        private CancellationTokenSource _lookForPlayerCancellationTokenSource;
        private List<Vector3> _eyeRayDestinations;
        private float _playerInViewTime;
        private float _currentViewDistance;
        // If set to true, then always sees the player
        private bool _isForcedToSeePlayer;

        public PathfindingAgent Agent => _agent;
        public ConfigurablePath DefaultPath => _defaultPath;
        public float AttackDistance => _attackDistance;
        public float PlayerDetectionTime => _playerDetectionTime;
        public float PlayerInViewTime => _playerInViewTime;
        public float DefaultViewDistance => _defaultViewDistance;
        public float InPursueViewDistance => _inPursueViewDistance;
        public bool IsForcedToSeePlayer { get => _isForcedToSeePlayer; set => _isForcedToSeePlayer = value; }

        public event Action<PlayerManager> PlayerFound;
        public event Action PlayerLost;

        protected override void AwakeManager()
        {
            _lookForPlayerCancellationTokenSource = new CancellationTokenSource();
        }

        protected override void StartManager()
        {
            _currentViewDistance = _defaultViewDistance;
        }

        protected override void UpdateManager()
        {
        }

        private void OnDestroy()
        {
            _lookForPlayerCancellationTokenSource.Cancel();
        }

        private void OnDrawGizmosSelected()
        {
            if (_eyeRayDestinations == null) return;

            Gizmos.color = Color.blue;

            foreach (Vector3 destination in _eyeRayDestinations)
            {
                Gizmos.DrawLine(_eyeOrigin.position, destination);
            }
        }

        protected override EnemyState GetInitialState()
        {
            if (_defaultPath == null)
            {
                return new IdleState(this);

            }
            else
            {
                return new PatrolState(this, _defaultPath);
            }
        }

        private async UniTask LookForPlayer(CancellationToken cancellationToken)
        {
            _eyeRayDestinations = new List<Vector3>(_raysToPlayer);
            PlayerManager player = ServiceLocator.Instance.GetService<GameManager>().PlayerManager;
            SpriteRenderer spriteRenderer = player.SpriteRenderer;

            while (player == null && !cancellationToken.IsCancellationRequested)
            {
                player = ServiceLocator.Instance.GetService<GameManager>().PlayerManager;
                spriteRenderer = player.SpriteRenderer;

                await UniTask.Yield();
            }

            bool seePlayer;
            Bounds bounds;

            while (!cancellationToken.IsCancellationRequested && player != null)
            {
                bounds = spriteRenderer.bounds;
                seePlayer = false;
                _eyeRayDestinations.Clear();

                if (!player.IsHidden)
                {
                    seePlayer = IsPlayerInView(player, bounds);
                }

                if (seePlayer || _isForcedToSeePlayer)
                {
                    float lastPlayerInViewTime = _playerInViewTime;
                    _playerInViewTime += Time.fixedDeltaTime;

                    if (_playerInViewTime >= _playerDetectionTime)
                    {
                        PlayerFound?.Invoke(player);
                    }
                }
                else
                {
                    bool inViewTimeGreaterZero = _playerInViewTime > 0f;
                    _playerInViewTime -= Time.fixedDeltaTime;
                    _playerInViewTime = Mathf.Clamp(_playerInViewTime, 0.0f, _playerDetectionTime);

                    if (_playerInViewTime == 0f && inViewTimeGreaterZero)
                    {
                        PlayerLost?.Invoke();
                    }
                }

                await UniTask.WaitForFixedUpdate();
            }
        }

        public bool IsPlayerInView(PlayerManager player, Bounds bounds, bool ignoreFov = false)
        {
            if (!ignoreFov)
            {
                Vector3 playerDirection = (player.transform.position - _eyeOrigin.position).normalized;

                if (Vector3.Angle(_eyeOrigin.up, playerDirection) > _fov / 2f)
                {
                    return false;
                }
            }

            Vector3 playerPosition = bounds.center;
            playerPosition.y += bounds.extents.y;

            float heightStep = bounds.size.y / _raysToPlayer;

            Vector3 toPlayerVector = playerPosition - _eyeOrigin.position;
            Vector3 closestPointOnBounds = bounds.ClosestPoint(_eyeOrigin.position);
            float distanceToPlayer = (closestPointOnBounds - _eyeOrigin.position).magnitude;
            // If at least one point on player's border can possibly be detected, than raycast

            if (distanceToPlayer <= _currentViewDistance)
            {
                RaycastHit2D hit;

                for (int i = 0; i < _raysToPlayer; i++)
                {
                    _eyeRayDestinations.Add(playerPosition);

                    Ray ray = new Ray(_eyeOrigin.position, toPlayerVector.normalized);
                    bool intersectsPlayer = false;

                    if (bounds.IntersectRay(ray, out float distance))
                    {
                        intersectsPlayer = distance <= _currentViewDistance;
                    }

                    hit = Physics2D.Raycast(_eyeOrigin.position, toPlayerVector.normalized, distanceToPlayer, _raycastLayerMask);

                    if ((hit.transform == null || hit.transform.root.CompareTag("Player")) && intersectsPlayer)
                    {
                        return true;
                    }

                    playerPosition.y -= heightStep;
                    toPlayerVector = playerPosition - _eyeOrigin.position;
                }
            }

            return false;
        }

        public void StartLookingForPlayer()
        {
            if (!_lookForPlayerCancellationTokenSource.IsCancellationRequested)
            {
                _lookForPlayerCancellationTokenSource.Cancel();
                _lookForPlayerCancellationTokenSource = new CancellationTokenSource();
            }

            LookForPlayer(_lookForPlayerCancellationTokenSource.Token).Forget();
        }

        public void StopLookingForPlayer()
        {
            if (!_lookForPlayerCancellationTokenSource.IsCancellationRequested)
            {
                _lookForPlayerCancellationTokenSource.Cancel();
                _lookForPlayerCancellationTokenSource = new CancellationTokenSource();
            }
        }

        public void SetViewDistance(float viewDistance)
        {
            _currentViewDistance = viewDistance;
        }
    }
}
