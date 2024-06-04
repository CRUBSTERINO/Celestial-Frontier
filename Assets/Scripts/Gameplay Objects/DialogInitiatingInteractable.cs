using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogInitiatingInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private InitiateDialogInteraction _initiateDialogInteraction;

    public Vector3 InteractionPosition => throw new NotImplementedException();

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    public List<Interaction> GetInteractions()
    {
        return new List<Interaction> { _initiateDialogInteraction };
    }

    public void PerformInteraction(Interaction interaction, Interactor interactor)
    {
        interaction.PerformInteraction(interactor);
        OnInteracted?.Invoke();
    }

    private void OnDestroy()
    {
        OnBecameNonInteractable?.Invoke();
    }
}
