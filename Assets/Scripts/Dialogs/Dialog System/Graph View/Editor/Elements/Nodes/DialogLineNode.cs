using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.Nodes
{
    using Saves;
    using Windows;
    using Utilities;
    using Ports;

    // Node for dialog line
    public class DialogLineNode : DialogNode
    {
        private Port _speakerPort;
        private Port _listenerPort;
        private Port _actionPort;

        // ID of PersonNode that is connected to input port, should be used only during data-loading, because it isn't updated when PersonNode is connected to port
        // Should use GetSpeakerData() instead
        [field: SerializeField] public string SpeakerNodeID { get; set; }
        // ID of PersonNode that is connected to input port, should be used only during data-loading, because it isn't updated when PersonNode is connected to port
        // Should use GetListenerData() instead
        [field: SerializeField] public string ListenerNodeID { get; set; }
        [field: SerializeField] public string ActionNodeID { get; set; }
        [field: SerializeField] public List<ChoiceEditorData> Choices { get; set; }
        [field: SerializeField] public List<SentenceEditorData> Sentences { get; set; }
        [field: SerializeField] public bool IsSelectedByPlayer { get; set; }
        [field: SerializeField] public bool EndsDialog { get; set; }
        [field: SerializeField] public bool IsStarting { get; set; }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }

        public override void Initialize(string id, string nodeName, DialogGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(id, nodeName, dsGraphView, position);

            _titleBackgroundColor = new Color32(24, 78, 119, 255);

            ShouldGiveNamingErrors = true;
        }

        public override void Draw()
        {
            base.Draw();

            #region MAIN CONTAINER
            Button addChoiceButton = ElementUtility.CreateButton("Add Choice", () =>
            {
                ChoiceEditorData choiceData = new ChoiceEditorData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");

            mainContainer.Insert(1, addChoiceButton);
            #endregion

            #region INPUT CONTAINER
            Port inputLinePort = this.CreatePort<DialogLinePort>("Dialog Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            _speakerPort = this.CreatePort<PersonPort>("Speaker", Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            _listenerPort = this.CreatePort<PersonPort>("Listener", Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            _actionPort = this.CreatePort<ActionPort>("Actions", Orientation.Horizontal, Direction.Input, Port.Capacity.Single);

            // Input ports have to store reference types as "userData" in order to be changed when ports are connected
            _speakerPort.userData = SpeakerNodeID;
            _listenerPort.userData = ListenerNodeID;
            _actionPort.userData = ActionNodeID;

            inputContainer.Add(inputLinePort);
            inputContainer.Add(_speakerPort);
            inputContainer.Add(_listenerPort);
            inputContainer.Add(_actionPort);
            #endregion

            #region OUTPUT CONTAINER
            foreach (ChoiceEditorData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }
            #endregion

            #region EXTENSION CONTAINER
            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = ElementUtility.CreateFoldout("Sentences");

            Button addSentenceButton = ElementUtility.CreateButton("Add Sentence", () =>
            {
                string sentenceText = "Dialog Sentence Text.";

                SentenceEditorData sentenceData = new SentenceEditorData();
                sentenceData.Text = sentenceText;

                Sentences.Add(sentenceData);
                int index = Sentences.Count - 1;

                TextField textField = CreateSentence(index);
                textFoldout.Insert(textFoldout.childCount - 1, textField);
            });

            addSentenceButton.AddToClassList("ds-node__button");

            textFoldout.Add(addSentenceButton);

            for (int i = 0; i < Sentences.Count; i++)
            {
                TextField textField = CreateSentence(i);
                textFoldout.Insert(textFoldout.childCount - 1, textField);
            }

            customDataContainer.Add(textFoldout);

            Toggle isStartingToggle = ElementUtility.CreateToggle(IsStarting, "Is Starting", callback =>
            {
                IsStarting = callback.newValue;
            });

            Toggle selectedByPlayerToggle = ElementUtility.CreateToggle(IsSelectedByPlayer, "Selected By Player", callback =>
            {
                IsSelectedByPlayer = callback.newValue;
            });

            Toggle endsDialogToggle = ElementUtility.CreateToggle(EndsDialog, "Ends Dialog", callback =>
            {
                EndsDialog = callback.newValue;
            });

            extensionContainer.Add(isStartingToggle);
            extensionContainer.Add(selectedByPlayerToggle);
            extensionContainer.Add(endsDialogToggle);
            extensionContainer.Add(customDataContainer);
            #endregion

            RefreshExpandedState();
        }

        public override NodeEditorData SaveEditorData(NodeEditorData dataInstance = null)
        {
            DialogLineNodeEditorData dialogLineData = (dataInstance == null) ? new DialogLineNodeEditorData() : dataInstance as DialogLineNodeEditorData;

            dialogLineData.SpeakerNodeID = GetSpeakerPortData();
            dialogLineData.ListenerNodeID = GetListenerPortData();
            dialogLineData.ActionNodeID = GetActionPortData();
            dialogLineData.Choices = ChoiceEditorData.CloneChoices(Choices);
            dialogLineData.Sentences = SentenceEditorData.CloneSentences(Sentences);
            dialogLineData.IsStarting = IsStarting;
            dialogLineData.IsSelectedByPlayer = IsSelectedByPlayer;
            dialogLineData.EndsDialog = EndsDialog;

            return base.SaveEditorData(dialogLineData);
        }

        // Returns data from the speaker port
        public string GetSpeakerPortData()
        {
            return _speakerPort.userData as string;
        }

        // Returns data from the lsitener port
        public string GetListenerPortData()
        {
            return _listenerPort.userData as string;
        }

        public string GetActionPortData()
        {
            return _actionPort.userData as string;
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort<DialogLinePort>();

            choicePort.userData = userData;

            ChoiceEditorData choiceData = (ChoiceEditorData)userData;

            Button deleteChoiceButton = ElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    _graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);

                _graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = ElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }

        private TextField CreateSentence(int sentenceIndex)
        {
            SentenceEditorData sentence = Sentences[sentenceIndex];
            TextField textField = ElementUtility.CreateTextArea(sentence.Text, null, callback => sentence.Text = callback.newValue);

            textField.AddClasses(
                "ds-node__text-field",
                "ds-node__quote-text-field"
            );

            Button deleteSentenceButton = ElementUtility.CreateButton("X", () =>
            {
                Sentences.Remove(sentence);
                textField.RemoveFromHierarchy();
            });

            deleteSentenceButton.AddToClassList("ds-node__button");

            textField.Add(deleteSentenceButton);

            return textField;
        }
    } 
}