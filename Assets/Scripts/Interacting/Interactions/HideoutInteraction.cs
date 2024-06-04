using System;
using UnityEngine;

[Serializable]
public class HideoutInteraction : Interaction
{
    [SerializeField] private Hideout _hideout;

    public override void PerformInteraction(Interactor interactor)
    {
        PlayerManager playerManager = interactor.transform.root.GetComponent<PlayerManager>();

        if (playerManager == null)
        {
            Debug.LogError("Interacting object is not a player");
            return;
        }

        _hideout.EnterHideout(playerManager);
    }
}
