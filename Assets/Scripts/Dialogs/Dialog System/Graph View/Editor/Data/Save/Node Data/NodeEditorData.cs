using System;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    using Windows;
    using Nodes;

    // Base saving data of every node in graph view
    [Serializable]
    public class NodeEditorData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField] public string GroupID { get; set; } // Group ID, to which this node belongs
        [field: SerializeField] public Vector2 Position { get; set; }

        // Implements memento pattern
        // Every class that overrides this class should create node instance, configure it and than return this method of base class
        // Restores state of node using upcasting (every NodeEditorData class restores only part of data that is saved in it)
        public virtual DialogNode CreateNodeInstance(DialogGraphView graphView, DialogNode nodeInstance = null)
        {
            nodeInstance ??= new DialogNode();

            nodeInstance.Initialize(ID, Title, graphView, Position);

            return nodeInstance;
        }
    } 
}
