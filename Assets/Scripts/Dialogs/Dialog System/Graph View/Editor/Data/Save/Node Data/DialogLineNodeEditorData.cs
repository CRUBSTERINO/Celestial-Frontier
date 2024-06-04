using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    using Nodes;
    using Windows;

    //Saves the dialoge node
    [Serializable]
    public class DialogLineNodeEditorData : NodeEditorData
    {
        [field: SerializeField] public string SpeakerNodeID { get; set; }
        [field: SerializeField] public string ListenerNodeID { get; set; }
        [field: SerializeField] public string ActionNodeID { get; set; }
        [field: SerializeField] public List<SentenceEditorData> Sentences { get; set; }
        [field: SerializeField] public List<ChoiceEditorData> Choices { get; set; }
        [field: SerializeField] public bool IsSelectedByPlayer { get; set; }
        [field: SerializeField] public bool EndsDialog { get; set; }
        [field: SerializeField] public bool IsStarting { get; set; }

        public override DialogNode CreateNodeInstance(DialogGraphView graphView, DialogNode nodeInstance = null)
        {
            DialogLineNode dialogLineNode = (nodeInstance == null) ? new DialogLineNode() : nodeInstance as DialogLineNode;

            dialogLineNode.SpeakerNodeID = SpeakerNodeID;
            dialogLineNode.ListenerNodeID = ListenerNodeID;
            dialogLineNode.ActionNodeID = ActionNodeID;
            dialogLineNode.Sentences = (Sentences == null) ? new List<SentenceEditorData>() : SentenceEditorData.CloneSentences(Sentences);
            dialogLineNode.Choices = (Choices == null) ? new List<ChoiceEditorData>() { new ChoiceEditorData() { Text = "New Choice" } } : ChoiceEditorData.CloneChoices(Choices);
            dialogLineNode.IsSelectedByPlayer = IsSelectedByPlayer;
            dialogLineNode.EndsDialog = EndsDialog;
            dialogLineNode.IsStarting = IsStarting;

            return base.CreateNodeInstance(graphView, dialogLineNode);
        }
    } 
}