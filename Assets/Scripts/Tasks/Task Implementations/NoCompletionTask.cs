// Has no requirments that have to be fulfilled in order to complete this task
// It has to be force-completed from outside (usually in dialogs)
public class NoCompletionTask : Task
{
    protected override bool AreCompletionRequirmentsFulfilled()
    {
        return false;
    }

    protected override void SubscribeToCompletionRequirments()
    {
    }

    protected override void UnsubscribeFromCompletionRequirments()
    {
    }
}
