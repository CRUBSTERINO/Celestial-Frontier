using System;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour, IInteractable
{
    [SerializeField] private PickUpInteraction _pickUpInteraction;

    public Vector3 InteractionPosition => transform.position;

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    private void OnDestroy()
    {
        OnBecameNonInteractable?.Invoke();
    }

    public List<Interaction> GetInteractions()
    {
        return new List<Interaction> { _pickUpInteraction };
    }

    public void PerformInteraction(Interaction interaction, Interactor interactor)
    {
        interaction.PerformInteraction(interactor);
        OnInteracted?.Invoke();
    }
}
