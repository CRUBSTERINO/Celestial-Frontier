namespace DialogSystem.Conditions
{
    // Base class for access conditions that require player manager for their functionality
    public abstract class DialogLinePlayerCoupledCondition : DialogLineCondition
    {
        protected PlayerManager _playerManager;

        protected void GetPlayerManager(DialogLine dialogLine)
        {
            _playerManager = dialogLine.Speaker as PlayerManager; // SPEAKER SHOULD ALWAYS BE THE PLAYER
        }

        public abstract override bool IsConditionFulfilled(DialogLine dialogLine);
    } 
}
