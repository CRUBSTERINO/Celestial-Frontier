using UnityEngine;

namespace Enemies
{
	using States;

	public abstract class EnemyManager : MonoBehaviour
	{
		private EnemyState _currentState;

        private void Awake()
        {
            AwakeManager();
        }

        private void Start()
        {
            EnemyState initialState = GetInitialState();
            SetState(initialState);

            StartManager();
        }

        private void Update()
        {
            EnemyState nextState = _currentState.Update();
            if (nextState != _currentState)
            {
                SetState(nextState);
            }

            UpdateManager();
        }

        protected void SetState(EnemyState state)
        {
            _currentState?.ExitState();
            _currentState = state;
            _currentState.EnterState();
        }

        // Replacement for Awake() for inherited classes
        protected abstract void AwakeManager();

        // Replacement for Start() for inherited classes
        protected abstract void StartManager();

        // Replacement for Update() for inherited classes, since Update() is used for state-update
        protected abstract void UpdateManager();

        // Implementations should create and return initial state
        protected abstract EnemyState GetInitialState();
    } 
}
