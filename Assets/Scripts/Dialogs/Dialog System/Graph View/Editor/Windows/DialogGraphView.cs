using DialogSystem.Editor.Nodes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.Windows
{
    using Saves;
    using Errors;
    using BlackboardElements;
    using Ports;
    using Utilities;

    public class DialogGraphView : GraphView
    {
        private DialogEditorWindow _editorWindow;
        private DialogSearchWindow _searchWindow;
        private DialogBlackboard _blackboard;

        private MiniMap _miniMap;

        // These dictionaries are made to contain graph elements names in order to give errors
        private SerializableDictionary<string, NodeErrorData> _ungroupedNodesErrorDataDictionary;
        // Dictionary of groups that can generate errors
        // Key = title of group, Value = group error data associated with this title
        // If there is more than one group with the same name than this groups are being stored in error data
        private SerializableDictionary<string, GroupErrorData> _groupsErrorDataDictionary;
        // Dictionary of nodes that are grouped and can generate errors
        // Key = Group, Value = Dictionary of grouped nodes, where Key is node's name nad Value is it's error data
        private SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>> _groupedNodesErrorDataDictionary;

        private int _nameErrorsAmount;

        public DialogBlackboard Blackboard => _blackboard;
        public int NameErrorsAmount
        {
            get
            {
                return _nameErrorsAmount;
            }

            set
            {
                _nameErrorsAmount = value;

                if (_nameErrorsAmount == 0)
                {
                    _editorWindow.EnableSaving();
                }

                if (_nameErrorsAmount == 1)
                {
                    _editorWindow.DisableSaving();
                }
            }
        }

        public DialogGraphView(DialogEditorWindow dsEditorWindow)
        {
            _editorWindow = dsEditorWindow;

            _ungroupedNodesErrorDataDictionary = new SerializableDictionary<string, NodeErrorData>();
            _groupsErrorDataDictionary = new SerializableDictionary<string, GroupErrorData>();
            _groupedNodesErrorDataDictionary = new SerializableDictionary<UnityEditor.Experimental.GraphView.Group, SerializableDictionary<string, NodeErrorData>>();

            AddManipulators();
            AddGridBackground();
            AddSearchWindow();
            AddMiniMap();
            AddBlackboard();
            SetupDragAndDrop();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                if (startPort.portType != port.portType)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #region Custom Manipulators
        private IManipulator CreateDialogLineNodeContextualMenu(string actionTitle)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(new DialogLineNodeEditorData()
                {
                    Title = "DialogName",
                    Position = GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)
                }))));

            return contextualMenuManipulator;

        }

        private IManipulator CreateConditionNodeContextualMenu(string actionTitle)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(new ConditionNodeEditorData()
                {
                    Title = "ConditionName",
                    Position = GetLocalMousePosition(actionEvent.eventInfo.localMousePosition),
                }))));

            return contextualMenuManipulator;
        }

        private IManipulator CreateActionNodeContextualMenu(string actionTitle)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(new ActionNodeEditorData()
                {
                    Title = "ActionName",
                    Position = GetLocalMousePosition(actionEvent.eventInfo.localMousePosition),
                }))));

            return contextualMenuManipulator;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }
        #endregion

        #region GraphView Handlers
        // Handels deletion of graph elements
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                List<Group> groupsToDelete = new List<Group>();
                List<Node> nodesToDelete = new List<Node>();
                List<Edge> edgesToDelete = new List<Edge>();

                // Prepare elements for deletion by their types
                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is Node node)
                    {
                        nodesToDelete.Add(node);
                    }
                    else if (selectedElement is Edge edge)
                    {
                        edgesToDelete.Add(edge);
                    }
                    else if (selectedElement is Group group)
                    {
                        groupsToDelete.Add(group);
                    }
                    else
                    {
                        Debug.LogWarning($"Graph element {selectedElement.title} canno't be deleted. You should add a deletion handler for it's type.");
                    }
                }

                // Delete groups
                foreach (DialogGroup groupToDelete in groupsToDelete)
                {
                    List<Node> groupNodes = new List<Node>();

                    // Find all nodes contained in this groups
                    foreach (GraphElement groupElement in groupToDelete.containedElements)
                    {
                        if (groupElement is not Node)
                        {
                            continue;
                        }

                        Node groupNode = (Node)groupElement;

                        groupNodes.Add(groupNode);
                    }

                    groupToDelete.RemoveElements(groupNodes);
                    RemoveGroup(groupToDelete);
                    RemoveElement(groupToDelete);
                }

                // Delete edges
                DeleteElements(edgesToDelete);

                // Delete nodes
                foreach (DialogNode nodeToDelete in nodesToDelete)
                {
                    if (nodeToDelete.Group != null)
                    {
                        nodeToDelete.Group.RemoveElement(nodeToDelete);
                    }

                    RemoveUngroupedNode(nodeToDelete);

                    nodeToDelete.DisconnectAllPorts();

                    RemoveElement(nodeToDelete);
                }
            };
        }

        // Handels addition of graph elements to group
        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is DialogNode node)
                    {
                        RemoveUngroupedNode(node);
                        AddGroupedNode(node, group as DialogGroup);
                    }
                }
            };
        }

        // Handels deletion of graph elements from group
        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is DialogNode node)
                    {
                        RemoveGroupedNode(node, group as DialogGroup);
                        AddUngroupedNode(node);
                    }
                }
            };
        }

        // Handels renaming of group
        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DialogGroup dsGroup = (DialogGroup)group;

                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        ++NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        --NameErrorsAmount;
                    }
                }

                RemoveGroup(dsGroup);

                dsGroup.OldTitle = dsGroup.title;

                AddGroup(dsGroup);
            };
        }

        // Handels changes in GraphView e.g. elements to remove, edges to create and moved elements
        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                // Create edges if there are any given
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        // Here should be the actions to transfer edge data depending on the type of ports to be connected
                        // using name of, because 'is' comparsion won't work
                        // "userData" of an edge should containt REFERENCE TYPE in order to be updated
                        // Cannot delegate task of saving port data on ports, because ports instances can be only of "Port" type. They doesn't support inheritance and because of that I have to use switch case
                        switch (edge.input.portType.Name)
                        {
                            case nameof(DialogLinePort):
                                Node inputNode = edge.input.node;
                                ChoiceEditorData choiceData = (ChoiceEditorData)edge.output.userData;

                                choiceData.ConnectedNodeID = (inputNode as DialogNode).ID;

                                break;

                            case nameof(PersonPort):
                                edge.input.userData = (edge.output.node as DialogNode).ID;

                                break;

                            case nameof(ActionPort):
                                edge.input.userData = (edge.output.node as DialogNode).ID;

                                break;
                        }
                    }
                }

                // Remove elements if there are any given
                if (changes.elementsToRemove != null)
                {
                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element is not Edge)
                        {
                            continue;
                        }

                        Edge edge = (Edge)element;

                        //DialogSystemChoiceSaveData choiceData = (DialogSystemChoiceSaveData)edge.output.userData;

                        // Here should clear "userData" in ports depending on their types
                        // using name of, because 'is' comparsion won't work
                        // "userData" should be a REFERENCE TYPE
                        switch (edge.input.portType.Name)
                        {
                            case nameof(DialogLinePort):
                                ChoiceEditorData choiceData = (ChoiceEditorData)edge.output.userData;

                                choiceData.ConnectedNodeID = string.Empty;

                                break;

                            case nameof(PersonPort):
                                edge.input.userData = string.Empty;

                                break;

                            case nameof(ActionPort):
                                edge.input.userData = string.Empty;

                                break;
                        }
                    }
                }

                return changes;
            };
        }

        private void OnDragUpdatedEvent(DragUpdatedEvent e)
        {
            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> selection && (selection.OfType<PersonBlackboardField>().Count() >= 0))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }
        }

        private void OnDragPerformEvent(DragPerformEvent e)
        {
            var selection = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;

            if (selection.OfType<PersonBlackboardField>().Count() >= 0)
            {
                IEnumerable<PersonBlackboardField> fields = selection.OfType<PersonBlackboardField>();
                foreach (PersonBlackboardField field in fields)
                {
                    PersonNodeEditorData personData = new PersonNodeEditorData()
                    {
                        Title = field.text,
                        BlackboardFieldID = field.ID,
                        Position = GetLocalMousePosition(e.localMousePosition)
                    };
                    DialogNode node = CreateNode(personData);

                    AddElement(node);
                }
            }

        }
        #endregion

        #region Group-related Methods
        // Creates group with given title and on given position
        public DialogGroup CreateGroup(string title, Vector2 position)
        {
            DialogGroup group = new DialogGroup(title, position);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (selectedElement is DialogLineNode)
                {
                    DialogLineNode node = (DialogLineNode)selectedElement;

                    group.AddElement(node);
                }
            }

            return group;
        }

        // Adds the group to dictionary
        private void AddGroup(DialogGroup group)
        {
            string groupName = group.title.ToLower();

            // If there is no group created with given name than we add it to groups dictionary
            if (!_groupsErrorDataDictionary.ContainsKey(groupName))
            {
                GroupErrorData groupErrorData = new GroupErrorData();

                groupErrorData.Groups.Add(group);
                _groupsErrorDataDictionary.Add(groupName, groupErrorData);

                return;
            }

            // Otherwise we add group to already existent group's with the same name errorData

            List<DialogGroup> groupsList = _groupsErrorDataDictionary[groupName].Groups;

            groupsList.Add(group);

            Color errorColor = _groupsErrorDataDictionary[groupName].ErrorData.Color;
            group.SetErrorStyle(errorColor);

            // Error amount will be incremented only when the first group with the same name is added
            if (groupsList.Count == 2)
            {
                ++NameErrorsAmount;

                groupsList[0].SetErrorStyle(errorColor);
            }
        }

        // Removes group from the dictionary
        private void RemoveGroup(DialogGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();

            List<DialogGroup> groupsList = _groupsErrorDataDictionary[oldGroupName].Groups;

            groupsList.Remove(group);

            group.ResetStyle();

            // If there is only one group left in error's data list than there are no more errors in naming
            if (groupsList.Count == 1)
            {
                --NameErrorsAmount;

                groupsList[0].ResetStyle();
            }
            // If number of groups in error data is 0 than there are no more groups with this key and this key should be deleted from the dicitonary
            else if (groupsList.Count == 0)
            {
                _groupsErrorDataDictionary.Remove(oldGroupName);
            }
        }
        #endregion

        #region Node-related Methods
        // Creates node from provided data
        public DialogNode CreateNode(NodeEditorData editorData)
        {
            // node instance is created by data (memento)
            DialogNode dialogNode = editorData.CreateNodeInstance(this);

            dialogNode.Draw();
            AddUngroupedNode(dialogNode);
            AddElement(dialogNode);

            return dialogNode;
        }

        public void AddUngroupedNode(DialogNode node)
        {
            if (!node.ShouldGiveNamingErrors) return; // If node shouldn't give errors than no need to add it to dictionary with errors

            string nodeName = node.Title.ToLower();

            // If there is no node created with given name than we add it to nodes dictionary
            if (!_ungroupedNodesErrorDataDictionary.ContainsKey(nodeName))
            {
                NodeErrorData nodeErrorData = new NodeErrorData();

                nodeErrorData.Nodes.Add(node);
                _ungroupedNodesErrorDataDictionary.Add(nodeName, nodeErrorData);

                return;
            }

            // Otherwise we add node to already existent node's with the same name errorData

            List<DialogNode> ungroupedNodesList = _ungroupedNodesErrorDataDictionary[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = _ungroupedNodesErrorDataDictionary[nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);

            // Error amount will be incremented only when the first node with the same name is added
            if (ungroupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveUngroupedNode(DialogNode node)
        {
            if (!node.ShouldGiveNamingErrors) return; // If node shouldn't give errors than no need to remove it from dictionary with errors

            string nodeName = node.Title.ToLower();

            List<DialogNode> ungroupedNodesList = _ungroupedNodesErrorDataDictionary[nodeName].Nodes;

            ungroupedNodesList.Remove(node);

            node.ResetStyle();

            // If there is only one group left in error's data list than there are no more errors in naming
            if (ungroupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                ungroupedNodesList[0].ResetStyle();
            }
            // If number of groups in error data is 0 than there are no more groups with this key and this key should be deleted from the dicitonary
            else if (ungroupedNodesList.Count == 0)
            {
                _ungroupedNodesErrorDataDictionary.Remove(nodeName);
            }
        }

        public void AddGroupedNode(DialogNode node, DialogGroup group)
        {
            node.Group = group;

            if (!node.ShouldGiveNamingErrors) return; // If node shouldn't give errors than no need to add it to dictionary with errors

            string nodeName = node.Title.ToLower();

            // If there were no nodes inside group than we create data in grouped nodes dictionary
            if (!_groupedNodesErrorDataDictionary.ContainsKey(group))
            {
                _groupedNodesErrorDataDictionary.Add(group, new SerializableDictionary<string, NodeErrorData>());
            }

            // If there is no node inside group created with given name than we add it to nodes dictionary
            if (!_groupedNodesErrorDataDictionary[group].ContainsKey(nodeName))
            {
                NodeErrorData nodeErrorData = new NodeErrorData();

                nodeErrorData.Nodes.Add(node);
                _groupedNodesErrorDataDictionary[group].Add(nodeName, nodeErrorData);

                return;
            }

            // Otherwise we add node to already existent node's with the same name errorData

            List<DialogNode> groupedNodesList = _groupedNodesErrorDataDictionary[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = _groupedNodesErrorDataDictionary[group][nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);

            // Error amount will be incremented only when the first node with the same name is added
            if (groupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveGroupedNode(DialogNode node, DialogGroup group)
        {
            node.Group = null;

            if (!node.ShouldGiveNamingErrors) return; // If node shouldn't give errors than no need to remove it from dictionary with errors

            string nodeName = node.Title.ToLower();

            List<DialogNode> groupedNodesList = _groupedNodesErrorDataDictionary[group][nodeName].Nodes; // Get list of nodes that have the same name inside of group

            groupedNodesList.Remove(node);

            node.ResetStyle();

            // If there is only one group left in error's data list than there are no more errors in naming
            if (groupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                groupedNodesList[0].ResetStyle();

                return;
            }

            // If number of groups in error data is 0 than there are no more groups with this key and this key should be deleted from the dicitonary
            if (groupedNodesList.Count == 0)
            {
                _groupedNodesErrorDataDictionary[group].Remove(nodeName);

                if (_groupedNodesErrorDataDictionary[group].Count == 0)
                {
                    _groupedNodesErrorDataDictionary.Remove(group);
                }
            }
        }
        #endregion

        #region Graph View Setup
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            /*this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DSDialogueType.MultipleChoice));*/
            this.AddManipulator(CreateDialogLineNodeContextualMenu("Add Dialog Node"));
            this.AddManipulator(CreateConditionNodeContextualMenu("Add Condition Node"));
            this.AddManipulator(CreateActionNodeContextualMenu("Add Action Node"));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddSearchWindow()
        {
            if (_searchWindow == null)
            {
                _searchWindow = ScriptableObject.CreateInstance<DialogSearchWindow>();
            }

            _searchWindow.Initialize(this);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        private void AddMiniMap()
        {
            _miniMap = new MiniMap()
            {
                anchored = true
            };

            _miniMap.SetPosition(new Rect(15, 50, 200, 180));

            Add(_miniMap);

            _miniMap.visible = false;
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "Dialog System/DSGraphViewStyles.uss",
                "Dialog System/DSNodeStyles.uss"
            );
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            _miniMap.style.backgroundColor = backgroundColor;
            _miniMap.style.borderTopColor = borderColor;
            _miniMap.style.borderRightColor = borderColor;
            _miniMap.style.borderBottomColor = borderColor;
            _miniMap.style.borderLeftColor = borderColor;
        }

        private void AddBlackboard()
        {
            _blackboard = new DialogBlackboard(this);

            Add(_blackboard);
        }

        // Registers Drag&Drop callbacks
        private void SetupDragAndDrop()
        {
            RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent); // Is called when dragging the object
            RegisterCallback<DragPerformEvent>(OnDragPerformEvent); // Is called when object is dropped (after dragging)
        }
        #endregion

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent, mousePosition - _editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));

            _groupsErrorDataDictionary.Clear();
            _groupedNodesErrorDataDictionary.Clear();
            _ungroupedNodesErrorDataDictionary.Clear();

            NameErrorsAmount = 0;
        }

        public void ToggleMiniMap()
        {
            _miniMap.visible = !_miniMap.visible;
        }
    } 
}