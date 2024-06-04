using System;
using UnityEngine;

[Serializable]
public class PickUpInteraction : Interaction
{
    [SerializeField] private ItemScriptableObject _item;
    [SerializeField] private GameObject _gameObjectToDestroy;

    public override void PerformInteraction(Interactor interactor)
    {
        PlayerManager playerManager = interactor.transform.root.GetComponent<PlayerManager>();

        if (playerManager == null)
        {
            Debug.LogError("Interacting object is not a player");
            return;
        }

        if (playerManager.TryToPickUpItem(_item))
        {
            DestroyPickupableGameObject();
        }
    }

/*    public override void AcceptInteractionVisitor(IInteractionVisitor visitor)
    {
        visitor.Visit(this);
    }*/

    public void DestroyPickupableGameObject()
    {
        UnityEngine.Object.Destroy(_gameObjectToDestroy);
    }
}
