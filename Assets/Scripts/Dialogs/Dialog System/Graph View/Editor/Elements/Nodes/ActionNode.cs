using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace DialogSystem.Editor.Nodes
{
    using DialogSystem.Editor.Saves;
    using Ports;
    using System.Linq;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using Utilities;
    using Windows;

    public class ActionNode : DialogNode
	{
        private List<Type> _actionsImplementations;

        [field: SerializeReference] public List<DialogAction> Actions { get; set; }

        public override void Initialize(string id, string nodeName, DialogGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(id, nodeName, dsGraphView, position);

            _titleBackgroundColor = new Color32(110, 13, 37, 255);

            ShouldGiveNamingErrors = true;
        }

        public override void Draw()
        {
            base.Draw();

            #region OUTPUT CONTAINER
            Port outputPort = this.CreatePort<ActionPort>("Action", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);

            outputContainer.Add(outputPort);
            #endregion

            #region EXTENSION CONTAINER

            // Gets List of all types that inherit from "DialogAction"
            _actionsImplementations = ReflectionsUtility.GetImplementations<DialogAction>().ToList();
            // Creates popup field that shows names of actions implementations
            PopupField<string> popupField = new PopupField<string>("Implementations", _actionsImplementations.Select(impl => impl.Name).ToList(), 0);

            extensionContainer.Add(popupField);

            Button addConditionButton = ElementUtility.CreateButton("Add Condition", () =>
            {
                // Creates instance of Action of specified with popupField type
                DialogAction action = (DialogAction)Activator.CreateInstance(_actionsImplementations[popupField.index]);
                Actions.Add(action);
                CreateAction(action);
            });

            addConditionButton.AddToClassList("ds-node__button");
            extensionContainer.Add(addConditionButton);

            foreach (DialogAction action in Actions)
            {
                CreateAction(action);
            }
            #endregion

            RefreshExpandedState();
        }

        public override NodeEditorData SaveEditorData(NodeEditorData dataInstance = null)
        {
            ActionNodeEditorData actionData = (dataInstance == null) ? new ActionNodeEditorData() : dataInstance as ActionNodeEditorData;

            actionData.Actions = DialogAction.CloneActions(Actions);

            return base.SaveEditorData(actionData);
        }

        private void CreateAction(DialogAction action)
        {
            VisualElement container = new VisualElement();

            Button deleteActionButton = ElementUtility.CreateButton("X", () =>
            {
                Actions.Remove(action);

                extensionContainer.Remove(container);
            });

            deleteActionButton.AddToClassList("ds-node__button");
            container.Add(deleteActionButton);

            // Draw inspector for property of given type
            // Uses workaround for drawing serializableReferences with wrapping property in ScriptableObject
            PropertyBuilder.DrawProperty(action, action.ToString(), container);

            extensionContainer.Add(container);
        }
    } 
}
