using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private TimeInterval _sleepingInterval;
    [SerializeField] private SleepInteraction _sleepInteraction;

    private WorldTimeService _worldTimeService;

    public Vector3 InteractionPosition => transform.position;

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    private void OnDestroy()
    {
        OnBecameNonInteractable?.Invoke();
    }

    private void Start()
    {
        _worldTimeService = ServiceLocator.Instance.GetService<WorldTimeService>();
    }

    public List<Interaction> GetInteractions()
    {
        if (_worldTimeService.WorldTime.IsInTimeInterval(_sleepingInterval))
        {
            return new List<Interaction>() { _sleepInteraction };
        }
        else
        {
            return null;
        }
    }

    public void PerformInteraction(Interaction interaction, Interactor interactor)
    {
        interaction.PerformInteraction(interactor);
        OnInteracted?.Invoke();
    }
}
