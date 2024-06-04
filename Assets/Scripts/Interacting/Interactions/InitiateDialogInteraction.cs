using System;
using UnityEngine;
using DialogSystem;

[Serializable]
public class InitiateDialogInteraction : Interaction
{
    [SerializeField] private DialogManager _dialogManager;
    [SerializeField] private Person _interactedPerson; // Person that has this interaction and should be registered in dialog

    public DialogManager DialogManager => _dialogManager;

/*    public override void AcceptInteractionVisitor(IInteractionVisitor visitor)
    {
        visitor.Visit(this);
    }*/

    public override void PerformInteraction(Interactor interactor)
    {
        if (_interactedPerson != null)
        {
            _dialogManager.RegisterDialogParticipant(_interactedPerson); 
        }

        PlayerManager playerManager = interactor.transform.root.GetComponent<PlayerManager>();

        if (playerManager == null)
        {
            Debug.LogError("Interacting object is not a player");
            return;
        }

        playerManager.InitiateDialog(_dialogManager);
    }
}
