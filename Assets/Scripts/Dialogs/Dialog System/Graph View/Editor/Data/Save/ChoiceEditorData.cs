using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
	[Serializable]
	public class ChoiceEditorData
	{
        // Text of choice TextField
        [field: SerializeField] public string Text { get; set; } 
		// Node to which this choice is connected
		[field: SerializeField] public string ConnectedNodeID { get; set; }

        // Clones choices in order to break references
        // Prevents affecting already saved data before pressing save button when editing in graph view
        public ChoiceEditorData CloneChoice()
		{
            ChoiceEditorData clonedChoiceData = new ChoiceEditorData()
            {
                Text = Text,
                ConnectedNodeID = ConnectedNodeID,
            };

			return clonedChoiceData;
        }

		public static List<ChoiceEditorData> CloneChoices(List<ChoiceEditorData> choiceData)
		{
			List<ChoiceEditorData> clonedChoiceData = new List<ChoiceEditorData>(choiceData.Count);

			foreach (ChoiceEditorData choice in choiceData)
			{                
				clonedChoiceData.Add(choice.CloneChoice());
			}

			return clonedChoiceData;
		}
	} 
}