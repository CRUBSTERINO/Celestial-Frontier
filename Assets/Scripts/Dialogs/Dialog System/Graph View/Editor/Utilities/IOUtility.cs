using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogSystem.Editor.Utilities
{
    using Nodes;
    using Windows;
    using Saves;
    using BlackboardElements;
    using DialogSystem.Editor.Ports;

    // Responsible for saving, updating and loading data from/to GraphView
    public static class IOUtility
    {
        private static DialogGraphView _graphView;

        #region Pathes
        private static string _graphFileName;
        private static string _graphSavePath;

        private static string _containerFolderPath;
        private static string _containerDialogLinesFolderPath;
        private static string _containerDialogParticipantsFolderPath;
        #endregion

        // Elements of graph view that are collected in "GetElementsFromGraphView" method
        // Dictionary of all nodes present in GraphView
        // Key = node id, Value = DialogNode
        private static Dictionary<string, DialogNode> _nodes;
        private static List<DialogGroup> _groups;

        // Dictionary of created during saving DialogLine's ScriptableObjects
        // Key = DialogLineNode ID, Value = Created ScriptableObject
        private static Dictionary<string, DialogLineScriptableObject> _createdDialogLines;
        // Dictionary of created during saving participants ScriptableObject's
        // Key = BlackboardField ID, Value = Created ScriptableObject
        private static Dictionary<string, DialogParticipantScriptableObject> _createdParticipants;

        private static Dictionary<string, DialogGroup> _loadedGroups;
        private static Dictionary<string, DialogNode> _loadedNodes;

        public static void Initialize(DialogGraphView dsGraphView, string graphName)
        {
            _graphView = dsGraphView;

            _graphFileName = graphName;
            _graphSavePath = $"{IOConstants.ROOT_EDITOR_FOLDER_PATH}/Graphs";

            _containerFolderPath = $"{IOConstants.ROOT_RUNTIME_FOLDER_PATH}/{graphName}";
            _containerDialogLinesFolderPath = $"{_containerFolderPath}/Dialog Lines";
            _containerDialogParticipantsFolderPath = $"{_containerFolderPath}/Dialog Participants";

            _nodes = new Dictionary<string, DialogNode>();
            _groups = new List<DialogGroup>();

            _createdDialogLines = new Dictionary<string, DialogLineScriptableObject>();
            _createdParticipants = new Dictionary<string, DialogParticipantScriptableObject>();

            _loadedGroups = new Dictionary<string, DialogGroup>();
            _loadedNodes = new Dictionary<string, DialogNode>();
        }

        #region Data Saving

        public static void Save()
        {
            DeleteSavedRuntimeData();
            CreateDefaultFolders();

            GetElementsFromGraphView();

            GraphEditorDataScriptableObject graphData = CreateAsset<GraphEditorDataScriptableObject>(_graphSavePath, $"{_graphFileName}Graph");

            graphData.Initialize(_graphFileName);

            DialogContainerScriptableObject dialogContainer = CreateAsset<DialogContainerScriptableObject>(_containerFolderPath, _graphFileName);

            dialogContainer.Initialize(_graphFileName);

            SaveBlackboard(graphData, _graphView.Blackboard, dialogContainer); // Should be saved before nodes, because node-saving requires blackboard fields to be already saved
            SaveGroups(graphData); // Groups should be saved only for graph. Runtime shouldn't have any groups
            SaveNodes(graphData, dialogContainer); // Nodes might be splitted into groups for graph data. Runtime should have all nodes raw (without any groups)

            SaveAsset(graphData);
            SaveAsset(dialogContainer);
        }

        // Save groups for GraphView
        private static void SaveGroups(GraphEditorDataScriptableObject graphData)
        {
            foreach (DialogGroup group in _groups)
            {
                SaveGroupToGraph(group, graphData);
            }
        }

        // Save group of nodes in data for editor
        private static void SaveGroupToGraph(DialogGroup group, GraphEditorDataScriptableObject graphData)
        {
            GroupEditorData groupData = new GroupEditorData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        // Save nodes/dialogs in editor/runtime data
        private static void SaveNodes(GraphEditorDataScriptableObject graphData, DialogContainerScriptableObject dialogContainer)
        {
            List<string> nodeNames = new List<string>();

            // Save each dialog node for editor and runtime
            // Can make runtime data saving as same as loading (using upcasting to "NodeEditorData"), but then it will be harder to verify saved data
            // Might change it in the future
            foreach (DialogNode node in _nodes.Values)
            {
                switch (node)
                {
                    case DialogLineNode dialogLineNode:
                        graphData.DialogNodes.Add(dialogLineNode.SaveEditorData() as DialogLineNodeEditorData);
                        SaveDialogNodeToScriptableObject(dialogLineNode, dialogContainer);
                        break;

                    case PersonNode personNode:
                        graphData.PersonNodes.Add(personNode.SaveEditorData() as PersonNodeEditorData);
                        break;

                    case ConditionNode conditionNode:
                        graphData.ConditionNodes.Add(conditionNode.SaveEditorData() as ConditionNodeEditorData);
                        break;

                    case ActionNode actionNode:
                        graphData.ActionNodes.Add(actionNode.SaveEditorData() as ActionNodeEditorData);
                        break;

                    default:
                        Debug.LogError($"Data of type {node.GetType()} can't be saved.");
                        break;
                }
            }

            UpdateDialogChoicesConnections();
        }

        // Save nodes in data for runtime
        // Should be called after dialog participants are saved
        private static void SaveDialogNodeToScriptableObject(DialogLineNode dialogNode, DialogContainerScriptableObject dialogContainer)
        {
            DialogLineScriptableObject dialog = CreateAsset<DialogLineScriptableObject>(_containerDialogLinesFolderPath, dialogNode.Title);

            dialogContainer.DialogLines.Add(dialog);

            // Get node id's of persons
            string speakerNodeID = dialogNode.GetSpeakerPortData();
            string listenerNodeID = dialogNode.GetListenerPortData();
            string actionNodeID = dialogNode.GetActionPortData();

            DialogParticipantScriptableObject listener = null;
            DialogParticipantScriptableObject speaker = null;
            List<DialogAction> actions = null;

            // If there is any node connected to speaker port, than assign speaker
            if (!string.IsNullOrEmpty(speakerNodeID))
            {
                // Get PersonNode by it's ID
                PersonNode speakerNode = _nodes[speakerNodeID] as PersonNode;
                // Get instance of participant ScriptableObject by blackboard field ID
                speaker = _createdParticipants[speakerNode.BlackboardFieldID];
            }

            // If there is any node connected to listener port, than assign listener
            if (!string.IsNullOrEmpty(listenerNodeID))
            {
                // Get PersonNode by it's ID
                PersonNode listenerNode = _nodes[listenerNodeID] as PersonNode;
                // Get instance of participant ScriptableObject by blackboard field ID
                listener = _createdParticipants[listenerNode.BlackboardFieldID];
            }

            if (!string.IsNullOrEmpty(actionNodeID))
            {
                ActionNode actionNode = _nodes[actionNodeID] as ActionNode;
                actions = DialogAction.CloneActions(actionNode.Actions);
            }


            dialog.Initialize(
                dialogNode.Title,
                speaker,
                listener,
                actions,
                ConvertNodeDialogSentencesToDialogSentences(dialogNode.Sentences),
                ConvertNodeChoicesToDialogChoices(dialogNode.Choices),
                dialogNode.IsSelectedByPlayer,
                dialogNode.EndsDialog,
                dialogNode.IsStarting
            );

            _createdDialogLines.Add(dialogNode.ID, dialog);

            if (dialogNode.IsStarting)
            {
                if (dialogContainer.StartingDialogLine == null)
                {
                    dialogContainer.StartingDialogLine = dialog;
                }
                else
                {
                    Debug.LogError($"There is more than one starting node in the graph. Starting node {dialogContainer.StartingDialogLine.Title} is already registered, " +
                        $"but you are trying to add one more starting node {dialog.Title}.");
                }
            }

            SaveAsset(dialog);
        }

        private static void SaveBlackboard(GraphEditorDataScriptableObject graphData, DialogBlackboard blackboard, DialogContainerScriptableObject dialogContainer)
        {
            // Each person property from blackboard is saved
            foreach (PersonBlackboardField personField in blackboard.BlackboardFields)
            {
                SavePersonBlackboardFieldToGraph(personField, graphData);
                SaveDialogParticipantToScriptableObject(personField, dialogContainer);
            }
        }

        // Saves given blackboard field into graph data
        private static void SavePersonBlackboardFieldToGraph(PersonBlackboardField blackboardField, GraphEditorDataScriptableObject graphData)
        {
            PersonBlackboardFieldEditorData saveData = new PersonBlackboardFieldEditorData();

            saveData.ID = blackboardField.ID;
            saveData.FieldText = blackboardField.text;
            saveData.TypeText = blackboardField.typeText;
            saveData.PersonID = blackboardField.PersonId;

            graphData.BlackboardField.Add(saveData);
        }

        // Saves given participant id as ParticipantScriptableObject
        private static void SaveDialogParticipantToScriptableObject(PersonBlackboardField blackboardField, DialogContainerScriptableObject dialogContainer)
        {
            DialogParticipantScriptableObject participant = CreateAsset<DialogParticipantScriptableObject>(_containerDialogParticipantsFolderPath, blackboardField.text);

            participant.Initialize(blackboardField.PersonId);

            dialogContainer.Participants.Add(participant);
            _createdParticipants.Add(blackboardField.ID, participant);

            SaveAsset(participant);
        }

        #endregion

        #region Data Updating

        // Updates runtime dialog choices with editor information
        private static void UpdateDialogChoicesConnections()
        {
            // Get list of all nodes of type "DialogLineNode" from list of all nodes
            List<DialogLineNode> dialogLineNodes = _nodes.Values.OfType<DialogLineNode>().ToList();

            foreach (DialogLineNode dialogLineNode in dialogLineNodes)
            {
                // Runtime dialog that equals the editor node
                DialogLineScriptableObject dialogLine = _createdDialogLines[dialogLineNode.ID];

                // Update each choice in runtime with choice from editor node
                for (int choiceIndex = 0; choiceIndex < dialogLineNode.Choices.Count; choiceIndex++)
                {
                    ChoiceEditorData nodeChoice = dialogLineNode.Choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.ConnectedNodeID))
                    {
                        continue;
                    }

                    DialogLineScriptableObject nextDialogLine = null;
                    DialogNode chosenNode = _nodes[nodeChoice.ConnectedNodeID];

                    // if output node is connected directly to next dialog line node, than just set it as next node
                    if (chosenNode is DialogLineNode chosenDialogLineNode)
                    {
                        nextDialogLine = _createdDialogLines[chosenDialogLineNode.ID];
                    }
                    // If output node is connected to condition node, than set conditins for choice and than get target dialog line node
                    else
                    {
                        if (chosenNode is ConditionNode conditionNode)
                        {
                            nextDialogLine = _createdDialogLines[conditionNode.TargetNodeChoice.ConnectedNodeID];
                            dialogLine.Choices[choiceIndex].Conditions = conditionNode.Conditions;
                        }
                    }

                    dialogLine.Choices[choiceIndex].NextDialog = nextDialogLine;

                    SaveAsset(dialogLine);
                }
            }
        }

        /*    // Updates and replaces old nodes that are used on runtime. Nodes that were removed in graph view are deleted
            private static void UpdateOldNodes(List<string> currentNodeNames, DialogSystemGraphEditorDataScriptableObject graphData)
            {
                if (graphData.OldNodeNames != null && graphData.OldNodeNames.Count != 0)
                {
                    List<string> nodesToRemove = graphData.OldNodeNames.Except(currentNodeNames).ToList();

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{_containerDialogLinesFolderPath}", nodeToRemove);
                    }
                }

                graphData.OldNodeNames = new List<string>(currentNodeNames);
            }*/

        #endregion

        #region Data Loading

        // Loads graph view from saved data
        public static void Load()
        {
            GraphEditorDataScriptableObject graphData = LoadAsset<GraphEditorDataScriptableObject>(_graphSavePath, _graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/DialogueSystem/Graphs/{_graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }

            DialogEditorWindow.UpdateFileName(graphData.FileName);

            LoadBlackboard(graphData.BlackboardField);
            LoadGroups(graphData.Groups);
            LoadNodes(graphData.DialogNodes.Cast<NodeEditorData>().ToList());
            LoadNodes(graphData.ConditionNodes.Cast<NodeEditorData>().ToList());
            LoadNodes(graphData.PersonNodes.Cast<NodeEditorData>().ToList());
            LoadNodes(graphData.ActionNodes.Cast<NodeEditorData>().ToList());
            LoadNodesConnections();
        }

        private static void LoadBlackboard(List<PersonBlackboardFieldEditorData> properties)
        {
            DialogBlackboard blackboard = _graphView.Blackboard;

            // Create exposed property for every property saved in data
            foreach (PersonBlackboardFieldEditorData property in properties)
            {
                blackboard.AddExposedProperty(property.ID, property.PersonID, property.FieldText, property.TypeText);
            }
        }

        private static void LoadGroups(List<GroupEditorData> groups)
        {
            foreach (GroupEditorData groupData in groups)
            {
                DialogGroup group = _graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                _loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<NodeEditorData> nodesData)
        {
            foreach (NodeEditorData nodeData in nodesData)
            {
                DialogNode node = _graphView.CreateNode(nodeData);

                _loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                DialogGroup group = _loadedGroups[nodeData.GroupID];
                node.Group = group;
                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, DialogNode> loadedNode in _loadedNodes)
            {
                // This looks very bad, but I just hope that it will do, because I will have to refactor to much otherwise
                if (loadedNode.Value is DialogLineNode)
                {
                    // Load connections of choice ports
                    foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                    {
                        ChoiceEditorData choiceData = (ChoiceEditorData)choicePort.userData; // Userdata of ports was loaded during creation of nodes (draw methods of nodes)

                        if (string.IsNullOrEmpty(choiceData.ConnectedNodeID))
                        {
                            continue;
                        }

                        DialogNode nextNode = _loadedNodes[choiceData.ConnectedNodeID];

                        Port nextNodeInputPort = (Port)nextNode.inputContainer
                            .Children()
                            .Where(port => (port as Port).portType == typeof(DialogLinePort))
                            .First();

                        Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                        _graphView.AddElement(edge);
                    }

                    // Get input ports for speaker and listener in DialogNode
                    Port speakerPort = (Port)loadedNode.Value.inputContainer.Children().Where(match => (match as Port).portName == "Speaker").First();
                    Port listenerPort = (Port)loadedNode.Value.inputContainer.Children().Where(match => (match as Port).portName == "Listener").First();
                    Port actionPort = (Port)loadedNode.Value.inputContainer.Children().Where(match => (match as Port).portName == "Actions").First();

                    // Data in ports is already loaded in node-loading methods
                    string speakerNodeID = (string)speakerPort.userData;
                    string listenerNodeID = (string)listenerPort.userData;
                    string actionNodeID = (string)actionPort.userData;

                    // Creates connection with person node if there was one connected
                    if (!string.IsNullOrEmpty(speakerNodeID))
                    {
                        PersonNode personNode = _loadedNodes[speakerNodeID] as PersonNode;
                        Port personNodeOutputPort = (Port)personNode.outputContainer.Children().First();
                        Edge edge = speakerPort.ConnectTo(personNodeOutputPort);

                        _graphView.AddElement(edge);
                    }

                    if (!string.IsNullOrEmpty(listenerNodeID))
                    {
                        PersonNode personNode = _loadedNodes[listenerNodeID] as PersonNode;
                        Port personNodeOutputPort = (Port)personNode.outputContainer.Children().First();
                        Edge edge = listenerPort.ConnectTo(personNodeOutputPort);

                        _graphView.AddElement(edge);
                    }

                    if (!string.IsNullOrEmpty(actionNodeID))
                    {
                        ActionNode actionNode = _loadedNodes[actionNodeID] as ActionNode;
                        Port actionNodeOutputPort = (Port)actionNode.outputContainer.Children().First();
                        Edge edge = actionPort.ConnectTo(actionNodeOutputPort);

                        _graphView.AddElement(edge);
                    }

                    loadedNode.Value.RefreshPorts();
                }
                else if (loadedNode.Value is ConditionNode)
                {
                    foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                    {
                        ChoiceEditorData choiceData = (ChoiceEditorData)choicePort.userData; // Userdata of ports was loaded during creation of nodes (draw methods of nodes)

                        if (string.IsNullOrEmpty(choiceData.ConnectedNodeID))
                        {
                            continue;
                        }

                        DialogLineNode nextNode = _loadedNodes[choiceData.ConnectedNodeID] as DialogLineNode;

                        Port nextNodeInputPort = (Port)nextNode.inputContainer
                            .Children()
                            .Where(port => (port as Port).portType == typeof(DialogLinePort))
                            .First();

                        Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                        _graphView.AddElement(edge);
                    }
                }
            }
        }

        #endregion

        #region IO Functionality

        // Creates folder that are needed for saving of data
        private static void CreateDefaultFolders()
        {
            CreateFolder(IOConstants.ROOT_EDITOR_FOLDER_PATH, "Graphs", true);

            CreateFolder(IOConstants.ROOT_RUNTIME_FOLDER_PATH, _graphFileName, true);
            CreateFolder(_containerFolderPath, "Dialog Lines", true);
            CreateFolder(_containerFolderPath, "Dialog Participants", true);
        }

        // Collects all elements that are given in the GraphView
        private static void GetElementsFromGraphView()
        {
            _graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is DialogNode node)
                {
                    _nodes.Add(node.ID, node);
                }
                else if (graphElement is DialogGroup group)
                {
                    _groups.Add(group);
                }
            });
        }

        // Delets every data that is is saved for runtime
        private static void DeleteSavedRuntimeData()
        {
            RemoveFolder(_containerDialogLinesFolderPath);
            RemoveFolder(_containerDialogParticipantsFolderPath);
        }

        // Creates folder at path: "parentFolderPath" with name: "newFolderName"
        // If "shouldCreatePreviousFolders" == true, than it will also recursively create all missing folders on the given path
        private static void CreateFolder(string parentFolderPath, string newFolderName, bool shouldCreatePreviousFolders = true)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            // Creating previous folders in a path in a recursive way
            if (shouldCreatePreviousFolders)
            {
                string[] parentPath = parentFolderPath.Split('/');

                if (parentPath.Length > 1)
                {
                    string previousFolderPath = string.Join('/', parentPath, 0, parentPath.Length - 1);
                    string previousFolderName = parentPath[parentPath.Length - 1];

                    if (!AssetDatabase.IsValidFolder($"{previousFolderPath}/{previousFolderName}")) // Create folder recursively only if it doesn't already exist
                    {
                        CreateFolder(previousFolderPath, previousFolderName, shouldCreatePreviousFolders);
                    }
                }
            }

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }

        // Removes folder and it's meta-files at given path
        private static void RemoveFolder(string path)
        {
            AssetDatabase.DeleteAsset($"{path}.meta");
            AssetDatabase.DeleteAsset($"{path}");
        }

        // Creates asset at path: "path" and name: "assetName"
        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        // Loads asset at path: "path" and name: "assetName"
        private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        // Saves changes of given asset
        private static void SaveAsset(Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            /*            AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();*/
        }

        // Remove asset of name: "assetName" at path: "path"
        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        #endregion

        // Convert editor choices into choices suitable for runtime
        private static List<DialogChoiceData> ConvertNodeChoicesToDialogChoices(List<ChoiceEditorData> nodeChoices)
        {
            List<DialogChoiceData> dialogChoices = new List<DialogChoiceData>();

            foreach (ChoiceEditorData nodeChoice in nodeChoices)
            {
                DialogChoiceData choiceData = new DialogChoiceData()
                {
                    Text = nodeChoice.Text,
                };

                dialogChoices.Add(choiceData);
            }

            return dialogChoices;
        }

        // Converts editor data of sentences into runtime sentences
        private static List<Sentence> ConvertNodeDialogSentencesToDialogSentences(List<SentenceEditorData> nodeSentences)
        {
            List<Sentence> dialogSentences = new List<Sentence>(nodeSentences.Count);

            foreach (SentenceEditorData nodeSentence in nodeSentences)
            {
                Sentence dialogSentence = new Sentence();
                dialogSentence.Text = nodeSentence.Text;

                dialogSentences.Add(dialogSentence);
            }

            return dialogSentences;
        }
    } 
}