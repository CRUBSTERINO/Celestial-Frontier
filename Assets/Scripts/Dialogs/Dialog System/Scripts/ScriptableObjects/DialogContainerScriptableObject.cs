using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    // The main container in which the whole course of the dialogue is saved for runtime
    public class DialogContainerScriptableObject : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<DialogLineScriptableObject> DialogLines { get; set; }
        [field: SerializeField] public DialogLineScriptableObject StartingDialogLine { get; set; }
        [field: SerializeField] public List<DialogParticipantScriptableObject> Participants { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            DialogLines = new List<DialogLineScriptableObject>();
            Participants = new List<DialogParticipantScriptableObject>();
        }
    } 
}