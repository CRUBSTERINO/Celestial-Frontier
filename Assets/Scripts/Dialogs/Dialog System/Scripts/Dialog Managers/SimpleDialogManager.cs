using UnityEngine;

namespace DialogSystem
{
    public class SimpleDialogManager : DialogManager
    {
        protected override Sentence GetDialogSentence(int sentenceIndex)
        {
            return _activeDialogLine.Sentences[sentenceIndex];
        }

        protected override DialogLine GetDialogLine(DialogLineScriptableObject lineConfig)
        {
            if (_dialogLines.ContainsKey(lineConfig))
            {
                return _dialogLines[lineConfig];
            }
            else
            {
                Debug.LogError("Given line config is not registered in dialog lines dictionary.");
                return null;
            }
        }
    } 
}
