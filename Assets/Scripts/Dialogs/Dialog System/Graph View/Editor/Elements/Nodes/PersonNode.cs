using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogSystem.Editor.Nodes
{
    using Windows;
    using Saves;
    using Ports;
    using Utilities;

    // Node for representation of exposed property for Person
    public class PersonNode : DialogNode
    {
        public override bool ShouldGiveNamingErrors { get => false; }
        public string BlackboardFieldID { get; set; } // ID of blackboard field

        public override void Initialize(string id, string nodeName, DialogGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(id, nodeName, dsGraphView, position);

            ShouldGiveNamingErrors = false;
            _titleBackgroundColor = new Color32(103, 148, 54, 255);
        }

        public override void Draw()
        {
            base.Draw();

            #region OUTPUT CONTAINER
            Port outputPort = this.CreatePort<PersonPort>("Person", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);

            outputContainer.Add(outputPort);
            #endregion

            RefreshExpandedState();
        }

        public override NodeEditorData SaveEditorData(NodeEditorData dataInstance = null)
        {
            PersonNodeEditorData personData = (dataInstance == null) ? new PersonNodeEditorData() : dataInstance as PersonNodeEditorData;

            personData.BlackboardFieldID = BlackboardFieldID;

            return base.SaveEditorData(personData);
        }
    } 
}
