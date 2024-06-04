using System;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    using Nodes;
    using Windows;

    // Saving data of person's node
    [Serializable]
	public class PersonNodeEditorData : NodeEditorData
	{
		[field: SerializeField] public string BlackboardFieldID { get; set; } // ID of person related to the node

        public override DialogNode CreateNodeInstance(DialogGraphView graphView, DialogNode nodeInstance = null)
        {
            PersonNode personNode = (nodeInstance == null) ? new PersonNode() : nodeInstance as PersonNode;

            personNode.BlackboardFieldID = (BlackboardFieldID == null) ? string.Empty : BlackboardFieldID;

            return base.CreateNodeInstance(graphView, personNode);
        }
    } 
}
