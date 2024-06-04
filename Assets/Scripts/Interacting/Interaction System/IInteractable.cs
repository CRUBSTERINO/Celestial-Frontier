using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public Vector3 InteractionPosition { get; }

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable; // Should call this event when gameObject is e.g. destroyed

    public List<Interaction> GetInteractions();

    public void PerformInteraction(Interaction interaction, Interactor interactor); // Without realization so each interactable can define his behaviour by interaction themselves (might lead to code repetition)
}
