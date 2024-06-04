namespace Enemies.States
{
    public abstract class EnemyState
	{
		private EnemyManager _enemyManager;

		protected EnemyManager EnemyManager => _enemyManager;

		public EnemyState(EnemyManager manager)
		{
			_enemyManager = manager;
		}

        // Wird von der Zustandsmaschine aufgerufen, um den aktuellen Zustand zu aktualisieren und wenn es notwendig ist, ihn zu ändern
        public EnemyState Update()
		{
            // Prüfung, ob ein Zustandswechsel erforderlich ist
            EnemyState nextState = ChangeState();

            // Wenn nicht, wird der aktuelle Zustand aktualisiert.
            if (nextState == this)
			{
				UpdateState();
			}
            // Falls erforderlich, wird der aktuelle Zustand verlassen.
            else
            {
                ExitState();
            }

            // Der aktuelle oder neue Zustand wird an den Zustandsmachine zurückgegeben
            return nextState;
		}

        // Aktionen beim Übergang in einen Zustand
        public abstract void EnterState();

        // Aktionen beim Verlassen des Zustands
        public abstract void ExitState();

        // Aktualisieren des aktuellen Zustands
        protected abstract void UpdateState();

        // Bedingungen für die Änderung des aktuellen Zustands
        protected abstract EnemyState ChangeState();
	} 
}
