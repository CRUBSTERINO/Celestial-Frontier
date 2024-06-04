using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneExitInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private SwitchSceneInteraction _switchSceneInteraction;

    public SceneScriptableObject TargetScene
    {
        get
        {
            return _switchSceneInteraction.GetTargetScene();
        }
    }

    public Vector3 InteractionPosition => transform.position;

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    private void OnDestroy()
    {
        OnBecameNonInteractable?.Invoke();
    }

    public List<Interaction> GetInteractions()
    {
        return new List<Interaction>() { _switchSceneInteraction };
    }

    public void PerformInteraction(Interaction interaction, Interactor interactor)
    {
        interaction.PerformInteraction(interactor);
        OnInteracted?.Invoke();
    }
}
