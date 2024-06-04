using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
	// Saving data of sentence
	[Serializable]
	public class SentenceEditorData
	{
		[field: SerializeField] public string Text { get; set; } // Text of sentence

        // Clones choices in order to break references
        // Prevents affecting already saved data before pressing save button when editing in graph view
        public SentenceEditorData CloneSentence()
        {
            SentenceEditorData clonedSentenceData = new SentenceEditorData()
            {
                Text = Text,
            };

            return clonedSentenceData;
        }

        public static List<SentenceEditorData> CloneSentences(List<SentenceEditorData> sentenceData)
        {
            List<SentenceEditorData> clonedSentenceData = new List<SentenceEditorData>(sentenceData.Count);

            foreach (SentenceEditorData sentence in sentenceData)
            {
                clonedSentenceData.Add(sentence.CloneSentence());
            }

            return clonedSentenceData;
        }
    } 
}
