using System;
using System.Collections.Generic;
using UnityEngine;

// Interactable with simple interaction that is used for prototyping
public class SimpleInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleInteraction _simpleInteraction;

    public Vector3 InteractionPosition => throw new NotImplementedException();

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    private void OnDestroy()
    {
        OnBecameNonInteractable?.Invoke();
    }

    public List<Interaction> GetInteractions()
    {
        return new List<Interaction>() { _simpleInteraction };
    }

    public void PerformInteraction(Interaction interaction, Interactor interactor)
    {
        interaction.PerformInteraction(interactor);
        OnInteracted?.Invoke();
    }
}
