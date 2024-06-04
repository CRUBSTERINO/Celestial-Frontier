using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    // Datenformat für die Speicherung von Repliken
    public class DialogLineScriptableObject : ScriptableObject
    {
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField] public DialogParticipantScriptableObject Speaker { get; set; }
        [field: SerializeField] public DialogParticipantScriptableObject Listener { get; set; }
        [field: SerializeReference] public List<DialogAction> Actions { get; set; }
        [field: SerializeField] public List<DialogChoiceData> Choices { get; set; }
        [field: SerializeField] public List<Sentence> Sentences { get; set; }
        [field: SerializeField] public bool IsSelectedByPlayer { get; set; }
        [field: SerializeField] public bool EndsDialog { get; set; }
        [field: SerializeField] public bool IsStarting { get; set; }

        // Speichert das Replik mit den in den Methodenparametern angegebenen Einstellungen
        public void Initialize(string title, DialogParticipantScriptableObject speakerId, DialogParticipantScriptableObject listenerId, List<DialogAction> actions, List<Sentence> sentences, List<DialogChoiceData> choices,
            bool isSelectedByPlayer, bool endsDialog, bool isStarting)
        {
            Title = title;
            Speaker = speakerId;
            Listener = listenerId;
            Actions = actions;
            Choices = choices;
            Sentences = sentences;
            IsSelectedByPlayer = isSelectedByPlayer;
            EndsDialog = endsDialog;
            IsStarting = isStarting;
        }
    } 
}