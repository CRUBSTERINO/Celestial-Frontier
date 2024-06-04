using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    using Nodes;
    using System;
    using Windows;

    [Serializable]
    public class ActionNodeEditorData : NodeEditorData
    {
        [field: SerializeReference] public List<DialogAction> Actions { get; set; }

        public override DialogNode CreateNodeInstance(DialogGraphView graphView, DialogNode nodeInstance = null)
        {
            ActionNode actionNode = (nodeInstance == null) ? new ActionNode() : nodeInstance as ActionNode;

            actionNode.Actions = (Actions == null) ? new List<DialogAction>() : DialogAction.CloneActions(Actions);

            return base.CreateNodeInstance(graphView, actionNode);
        }
    } 
}
