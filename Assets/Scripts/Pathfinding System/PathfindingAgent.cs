using UnityEngine;
using DG.Tweening;
using System;

public class PathfindingAgent : MonoBehaviour
{
    [SerializeField] private float _speed;
    
    private Animator _animator;
    private bool _hasPath;
    private bool _isStopped;
    private PathfindingPath _currentPath;
    private PathfindingService _pathfindingService;
    private Tween _movementTween;
    private Vector3 _velocity;

    private int _animatorHorizontalParameterHash;
    private int _animatorVerticalParameterHash;
    private int _animatorIsMovingParameterHash;

    public bool HasPath => _hasPath;
    public bool IsStopped {  get { return _isStopped; } 
        set
        {
            _isStopped = value;

            if (_movementTween != null && _movementTween.active)
            {
                if (_isStopped)
                {
                    _animator?.SetBool(_animatorIsMovingParameterHash, false);

                    _movementTween.Pause();
                }
                else
                {
                    _animator?.SetBool(_animatorIsMovingParameterHash, true);

                    _movementTween.Play();
                }
            }
        }
    }

    public event Action OnPathStarted;
    public event Action OnPathReset;
    public event Action OnPathCompleted;

    private void Awake()
    {
        _pathfindingService = ServiceLocator.Instance.GetService<PathfindingService>();

        _animator = GetComponentInChildren<Animator>();

        _animatorHorizontalParameterHash = Animator.StringToHash("Horizontal");
        _animatorVerticalParameterHash = Animator.StringToHash("Vertical");
        _animatorIsMovingParameterHash = Animator.StringToHash("IsMoving");
    }

    // Bewegung der Spielfigur zum nächsten Wegpunkt
    private void ProceedOnPath() 
    {
        if (_currentPath == null) return;

        _movementTween.Kill();

        // Ermittlung des nächsten Wegpunkts
        Vector3 nextWaypoint;
        bool nextWaypointExists = _currentPath.TryGetNextWaypoint(out nextWaypoint);

        if (nextWaypointExists)
        {
            // Bewegungslogik der Spielfigur
            float duration = (nextWaypoint - transform.position).magnitude / _speed;
            _velocity = (nextWaypoint - transform.position) / duration;

            SetMovementDirection(_velocity.normalized);

            _movementTween = transform.DOMove(nextWaypoint, duration).SetEase(Ease.Linear);
            _movementTween.OnComplete(ProceedOnPath);

            if (_isStopped)
            {
                _movementTween.Pause();
            }
        }
        // Wenn es den nächsten Wegpunkt nicht gibt, hat die Spielfigur ihr Ziel erreicht.
        else
        {
            CompletePath();
        }
    }

    // Should be called to complete path (when destination is reached)
    private void CompletePath()
    {
        _currentPath = null;
        _hasPath = false;

        _movementTween?.Kill();
        _movementTween = null;
        _velocity = Vector3.zero;

        _animator?.SetBool(_animatorIsMovingParameterHash, false);

        OnPathCompleted?.Invoke();
    }

    // Assign path and start following it
    private void SetPath(PathfindingPath path) 
    {
        if (_currentPath != null)
        {
            ResetPath();
        }

        _currentPath = path;
        _hasPath = true;
        OnPathCompleted = null; // idk if it is a good idea to set event null value, but for now so

        if (!_isStopped)
        {
            _animator?.SetBool(_animatorIsMovingParameterHash, true); 
        }

        OnPathStarted?.Invoke();

        ProceedOnPath();
    }

    // Configuration of movement direction in animator
    private void SetMovementDirection(Vector3 direction)
    {
        if (_animator == null) return;

        if (_isStopped)
        {
            _animator.SetFloat(_animatorHorizontalParameterHash, 0f);
            _animator.SetFloat(_animatorVerticalParameterHash, 0f);

            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        Vector3 velocityDirection = Vector3.Normalize(_velocity);

        // 0.01f is just a magic number to exclude float inaccuracies
        if (velocityDirection.x > 0.01f || velocityDirection.x < -0.01f)
        {
            horizontal = (velocityDirection.x > 0) ? 1f : -1f;
        }

        // 0.01f is just a magic number to exclude float inaccuracies
        if (velocityDirection.y > 0.01f || velocityDirection.y < -0.01f)
        {
            vertical = (velocityDirection.y > 0) ? 1f : -1f;
        }

        if (horizontal != 0f && vertical != 0f)
        {
            vertical = 0f;
        }

        _animator.SetFloat(_animatorHorizontalParameterHash, horizontal);
        _animator.SetFloat(_animatorVerticalParameterHash, vertical);
    }

    // Request pathfinding to the given destination. Agent find path and than proceeds to follow it
    public void SetDestination(Vector3 destination) 
    {
        PathfindingPath path = _pathfindingService.FindPathInWorld(transform.position, destination);

        if (path != null)
        {
            SetPath(path);
        }
    }

    public void ResetPath()
    {
        _currentPath = null;
        _hasPath = false;

        _movementTween?.Kill();
        _movementTween = null;
        _velocity = Vector3.zero;

        _animator?.SetBool(_animatorIsMovingParameterHash, false);

        OnPathReset?.Invoke();
    }
}
