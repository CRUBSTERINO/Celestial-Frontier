using System;
using UnityEngine;

[Serializable]
public class TaskAssignmentInteraction : Interaction
{
    [SerializeField] private TaskScriptableObject _taskConfig;

/*    public override void AcceptInteractionVisitor(IInteractionVisitor visitor)
    {
        visitor.Visit(this);
    }*/

    public override void PerformInteraction(Interactor interactor)
    {
        PlayerManager playerManager = interactor.transform.root.GetComponent<PlayerManager>();

        if (playerManager == null)
        {
            Debug.LogError("Interacting object is not a player");
            return;
        }

        playerManager.TaskManager.AssignTask(_taskConfig);
    }
}
