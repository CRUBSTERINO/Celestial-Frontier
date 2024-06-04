using System;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour, IInteractable
{
    [SerializeField] private MinigameInteraction _minigameInteraction;
    [SerializeField] private TaskScriptableObject _requiredForMinigameTask;

    public Vector3 InteractionPosition => transform.position;

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    public List<Interaction> GetInteractions()
    {
        if (!ServiceLocator.Instance.GetService<GameManager>().PlayerManager.TaskManager.IsTaskActive(_requiredForMinigameTask))
        {
            return null;
        }

        return new List<Interaction> { _minigameInteraction };
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
