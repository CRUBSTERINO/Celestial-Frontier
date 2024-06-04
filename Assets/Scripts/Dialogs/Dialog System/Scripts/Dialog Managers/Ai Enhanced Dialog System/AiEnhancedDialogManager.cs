using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DialogSystem
{
    public class AiEnhancedDialogManager : DialogManager
    {
        [SerializeField] private DialogAiEnhancer _dialogEnhancer;

        private List<string> _currentSentences;
        private Sentence _enhancedSentence;
        private CancellationTokenSource _sentenceGenerationCancellationTokenSource;

        // Adresses to dialogEnhancer to enhance the initial text of sentence with AI
        private async UniTask EnhanceSentence(Person speaker, string initialSentenceText)
        {
            _enhancedSentence.IsFinished = false;

            await UniTask.DelayFrame(5); // now the pause is needed to make the cancellation token work. The method is being called when reseting sentence, causing token to reset.

            _sentenceGenerationCancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _sentenceGenerationCancellationTokenSource.Token;

            // Enhance initial sentence. Every responseHandler call the enchancedSentence text will be updated
            string response = await _dialogEnhancer.EnhanceInitialSentence(speaker, initialSentenceText, (response) => _enhancedSentence.Text = response, token);

            // If sentence generation wasn't cancelled than we consider it as successful and add results to current sentences
            if (!token.IsCancellationRequested)
            {
                _currentSentences.Add(response);
                _enhancedSentence.IsFinished = true;
            }
        }

        public override void InitiateDialog()
        {
            _currentSentences = null;
            _enhancedSentence = null;
            _dialogEnhancer.SetupDialogParticipants(_dialogParticipants);
            base.InitiateDialog();
        }

        public override void RestartDialogSentence()
        {
            if (_sentenceGenerationCancellationTokenSource != null)
            {
                _sentenceGenerationCancellationTokenSource.Cancel(true);
            }

            base.RestartDialogSentence();
        }

        protected override DialogLine GetDialogLine(DialogLineScriptableObject config)
        {
            if (_dialogLines.ContainsKey(config))
            {
                DialogLine dialogLine = _dialogLines[config];

                if (!dialogLine.EndsDialog)
                {
                    if (_currentSentences != null)
                    {
                        // Pass everything that was said now to the next speaker
                        _dialogEnhancer.RecordInterlocutorsSentences(dialogLine.Speaker, _currentSentences);
                    }

                    _currentSentences = new List<string>(dialogLine.Sentences.Count);
                }
               
                return dialogLine;
            }
            else
            {
                Debug.LogError("Given line config is not registered in dialog lines dictionary.");
                return null;
            }
        }

        protected override Sentence GetDialogSentence(int sentenceIndex)
        {
            _enhancedSentence = new Sentence();

            string initialSentenceText = _activeDialogLine.Sentences[sentenceIndex].Text;
            _enhancedSentence.Text = string.Empty;

            EnhanceSentence(_activeDialogLine.Speaker, initialSentenceText).Forget();

            return _enhancedSentence;
        }
    } 
}
