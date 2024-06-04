using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    public class DialogLine
    {
        [field: SerializeField] public List<DialogChoiceData> Choices { get; set; }
        [field: SerializeField] public DialogParticipantScriptableObject SpeakerParticipant { get; set; } // Index of dialog participant that is being set in DialogManager
        [field: SerializeField] public DialogParticipantScriptableObject ListenerParticipant { get; set; }
        [field: SerializeField] public List<DialogAction> Actions { get; set; } // actions should be called at the end of the line
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField] public List<Sentence> Sentences { get; set; }
        [field: SerializeField] public bool IsSelectedByPlayer { get; set; }
        [field: SerializeField] public bool EndsDialog { get; set; }

        public Person Speaker { get; set; }
        public Person Listener { get; set; }

        public DialogLine(DialogLineScriptableObject config)
        {
            Choices = new List<DialogChoiceData>(config.Choices);
            SpeakerParticipant = config.Speaker;
            ListenerParticipant = config.Listener;
            Actions = config.Actions;
            Title = config.Title;
            Sentences = new List<Sentence>(config.Sentences);
            IsSelectedByPlayer = config.IsSelectedByPlayer;
            EndsDialog = config.EndsDialog;
        }
    } 
}