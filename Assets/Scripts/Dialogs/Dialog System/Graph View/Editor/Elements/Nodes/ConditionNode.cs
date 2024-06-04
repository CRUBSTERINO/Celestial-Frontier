using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.Nodes
{
    using Windows;
    using Saves;
    using Utilities;
    using Ports;

    // Node that represents condition that has to be met in order to access TargetNode
    // TargetNode should be a DialogLineNode!!!
    public class ConditionNode : DialogNode
    {
        // List of all types that inherit from "DialogLineCondition"
        private List<Type> _conditionImplementations;

        // List of conditions that have to be completed in order to gave access to TargetNode
        [field: SerializeReference] public List<DialogLineCondition> Conditions { get; set; }
        // Node that requires given conditions fullfilled to be met in order to be access
        [field: SerializeField] public ChoiceEditorData TargetNodeChoice { get; set; }

        public override void Initialize(string id, string nodeName, DialogGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(id, nodeName, dsGraphView, position);

            _titleBackgroundColor = new Color32(36, 0, 70, 255);

            ShouldGiveNamingErrors = true;
        }

        public override void Draw()
        {
            base.Draw();

            #region INPUT CONTAINER
            Port inputLinePort = this.CreatePort<DialogLinePort>("Dialog Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputLinePort);
            #endregion

            #region OUTPUT CONTAINER
            Port outputLinePort = this.CreatePort<DialogLinePort>("Dialog Connection", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);

            // Output port references to the target node
            outputLinePort.userData = TargetNodeChoice;

            outputContainer.Add(outputLinePort);
            #endregion

            #region EXTENSION CONTAINER

            // Gets List of all types that inherit from "DialogLineCondition"
            _conditionImplementations = ReflectionsUtility.GetImplementations<DialogLineCondition>().ToList();
            // Creates popup field that shows names of conditions implementations
            PopupField<string> popupField = new PopupField<string>("Implementations", _conditionImplementations.Select(impl => impl.Name).ToList(), 0);

            extensionContainer.Add(popupField);

            Button addConditionButton = ElementUtility.CreateButton("Add Condition", () =>
            {
                // Creates instance of Condition of specified with popupField type
                DialogLineCondition condition = (DialogLineCondition)Activator.CreateInstance(_conditionImplementations[popupField.index]);
                Conditions.Add(condition);
                CreateCondition(condition);
            });

            addConditionButton.AddToClassList("ds-node__button");
            extensionContainer.Add(addConditionButton);

            foreach (DialogLineCondition condition in Conditions)
            {
                CreateCondition(condition);
            }
            #endregion

            RefreshExpandedState();
        }

        public override NodeEditorData SaveEditorData(NodeEditorData dataInstance = null)
        {
            ConditionNodeEditorData conditionData = (dataInstance == null) ? new ConditionNodeEditorData() : dataInstance as ConditionNodeEditorData;

            conditionData.Conditions = DialogLineCondition.CloneConditions(Conditions);
            conditionData.TargetNode = TargetNodeChoice.CloneChoice();

            return base.SaveEditorData(conditionData);
        }

        // Creates visuals for provided condition
        private void CreateCondition(DialogLineCondition condition)
        {
            VisualElement container = new VisualElement();

            Button deleteConditionButton = ElementUtility.CreateButton("X", () =>
            {
                Conditions.Remove(condition);

                extensionContainer.Remove(container);
            });

            deleteConditionButton.AddToClassList("ds-node__button");
            container.Add(deleteConditionButton);

            // Draw inspector for property of given type
            // Uses workaround for drawing serializableReferences with wrapping property in ScriptableObject
            PropertyBuilder.DrawProperty(condition, condition.ToString(), container);

            extensionContainer.Add(container);
        }
    } 
}
