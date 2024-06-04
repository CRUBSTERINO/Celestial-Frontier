using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    using Nodes;
    using Windows;

    [Serializable]
	public class ConditionNodeEditorData : NodeEditorData
	{
		[field: SerializeReference] public List<DialogLineCondition> Conditions { get; set; }
		[field: SerializeField] public ChoiceEditorData TargetNode { get; set; }

        public override DialogNode CreateNodeInstance(DialogGraphView graphView, DialogNode nodeInstance = null)
        {
            ConditionNode conditionNode = (nodeInstance == null) ? new ConditionNode() : nodeInstance as ConditionNode;

            conditionNode.Conditions = (Conditions == null) ? new List<DialogLineCondition>() : DialogLineCondition.CloneConditions(Conditions);
            conditionNode.TargetNodeChoice = (TargetNode == null) ? new ChoiceEditorData() : TargetNode.CloneChoice();

            return base.CreateNodeInstance(graphView, conditionNode);
        }
    } 
}
