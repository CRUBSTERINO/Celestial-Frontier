using Cysharp.Threading.Tasks;
using UnityEngine;

public class InteractionTask : Task
{
    [SerializeField] private string _interactableID;

    private IInteractable _interactable;
    private bool _didInteractionOccur;

    private void InteractableInteractedHandler()
    {
        _didInteractionOccur = true;
        TryComplete();
    }

    private void InteractableInstanceBecameNonInteractableHandler()
    {
        _interactable = null;
        SearchForInteractable().Forget();
    }

    protected override bool AreCompletionRequirmentsFulfilled()
    {
        if (_didInteractionOccur)
        {
            return true;
        }

        return false;
    }

    protected override void SubscribeToCompletionRequirments()
    {
        SearchForInteractable().Forget();
    }

    protected override void UnsubscribeFromCompletionRequirments()
    {
        if(_interactable != null)
        {
            _interactable.OnInteracted -= InteractableInteractedHandler; 
        }
    }

    private async UniTask SearchForInteractable()
    {
        while (_interactable == null && !_didInteractionOccur)
        {
            UniqueIdentifier idInstance = UniqueIdentifier.FindObjectWithID(_interactableID);

            if (idInstance != null)
            {
                _interactable = idInstance.gameObject.GetComponent<IInteractable>();
                _interactable.OnInteracted += InteractableInteractedHandler;
                _interactable.OnBecameNonInteractable += InteractableInstanceBecameNonInteractableHandler;
            }

            await UniTask.Yield();
        }
    }
}
