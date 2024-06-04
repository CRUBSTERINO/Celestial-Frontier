using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.Nodes
{
    using Windows;
    using Utilities;
    using Ports;
    using Saves;

    // Base class for any node in GraphView
    public class DialogNode : Node
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField] public DialogGroup Group { get; set; } // Group to which this node belongs
        [field: SerializeField] public virtual bool ShouldGiveNamingErrors { get; set; } // Should this node generate errors when two of them have the same name

        protected DialogGraphView _graphView;
        protected Color _titleBackgroundColor;
        private Color _defaultBackgroundColor;

        public virtual void Initialize(string id, string title, DialogGraphView dsGraphView, Vector2 position)
        {
            ID = (string.IsNullOrEmpty(id)) ? Guid.NewGuid().ToString() : id;
            Title = title;

            Rect rectPosition = new Rect(position, Vector2.zero);
            SetPosition(rectPosition);

            _graphView = dsGraphView;
            _defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public virtual void Draw()
        {
            #region TITLE CONTAINER
            titleContainer.style.backgroundColor = _titleBackgroundColor;

            TextField dialogueNameTextField = ElementUtility.CreateTextField(Title, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(Title))
                    {
                        ++_graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Title))
                    {
                        --_graphView.NameErrorsAmount;
                    }
                }

                if (Group == null)
                {
                    _graphView.RemoveUngroupedNode(this);

                    Title = target.value;

                    _graphView.AddUngroupedNode(this);

                    return;
                }

                DialogGroup currentGroup = Group;

                _graphView.RemoveGroupedNode(this, Group);

                Title = target.value;

                _graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, dialogueNameTextField);
            #endregion

            #region INPUT CONTAINER
            // Created invisible input port (without this port, when all ports are connected node won't be collapsable) (kind of a workaround)
            Port hiddenPort = this.CreatePort<ConfigurablePort>(string.Empty, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            hiddenPort.style.display = DisplayStyle.None;
            inputContainer.Add(hiddenPort);
            #endregion

            RefreshExpandedState();
        }

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        public void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        public void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        // Saves current node state into editor data
        // Overrided methods should create data of their type, save their data in it and then call base method
        // Every polymorphic class saves only it's specific data, delegating saving of shared data to base methods
        public virtual NodeEditorData SaveEditorData(NodeEditorData dataInstance = null)
        {
            dataInstance ??= new NodeEditorData();

            dataInstance.ID = ID;
            dataInstance.Title = Title;
            dataInstance.GroupID = Group?.ID;
            dataInstance.Position = GetPosition().position;

            return dataInstance;
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                _graphView.DeleteElements(port.connections);
            }
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = _defaultBackgroundColor;
        }
    } 
}
