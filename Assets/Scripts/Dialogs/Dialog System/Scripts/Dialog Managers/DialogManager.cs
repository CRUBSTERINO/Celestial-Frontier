using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    public abstract class DialogManager : MonoBehaviour
    {
        [SerializeField] private DialogContainerScriptableObject _dialogConfiguration;

        // Dictionary of dialog lines
        // Key = Line config, Value = line instance
        // Lines are instantiated in "Start" method
        protected Dictionary<DialogLineScriptableObject, DialogLine> _dialogLines;
        protected DialogLine _activeDialogLine;
        protected int _activeSentenceIndex;
        protected Sentence _activeSentence;
        protected List<Person> _dialogParticipants;

        public bool IsOnLastSentenceInLine
        {
            get
            {
                if (_activeSentence != null)
                {
                    return _activeDialogLine.Sentences.Count - 1 == _activeSentenceIndex;
                }

                return false;
            }
        }
        public bool IsSentenceFinished
        {
            get
            {
                if (_activeSentence != null)
                {
                    return _activeSentence.IsFinished;
                }

                return false;
            }
        }

        public event Action<DialogLineScriptableObject, DialogLine> OnDialogLineSelected;
        public event Action<Sentence> OnDialogSentenceSelected;
        public event Action OnDialogInitiated;
        public event Action OnDialogSentenceFinished;
        public event Action OnDialogLeave;

        private void Start()
        {
            if (_dialogConfiguration == null) return;

            _dialogParticipants = new List<Person>(_dialogConfiguration.Participants.Count);
            _dialogLines = new Dictionary<DialogLineScriptableObject, DialogLine>(_dialogConfiguration.DialogLines.Count);

            // Instantiate DialogLines from their config in ScriptableObjects
            foreach (DialogLineScriptableObject lineConfig in _dialogConfiguration.DialogLines)
            {
                DialogLine dialogLine = new DialogLine(lineConfig);
                _dialogLines.Add(lineConfig, dialogLine);
            }
        }

        private void SelectDialogSentence(int index)
        {
            if (_activeSentence != null)
            {
                _activeSentence.OnFinished -= OnActiveSentenceFinishedHandler;
            }

            _activeSentenceIndex = index;
            _activeSentence = GetDialogSentence(index);
            OnDialogSentenceSelected?.Invoke(_activeSentence);

            _activeSentence.OnFinished += OnActiveSentenceFinishedHandler;
        }

        private void SelectDialogLine(DialogLineScriptableObject config) // Move to player-selected dialog line
        {
            _activeDialogLine = GetDialogLine(config);

            if (_activeDialogLine != null)
            {
                if (_activeDialogLine.EndsDialog)
                {
                    TriggerDialogLineActions(_activeDialogLine);
                    LeaveDialog();
                    return;
                }

                OnDialogLineSelected?.Invoke(config, _activeDialogLine);

                SelectDialogSentence(0);
            }
            else
            {
                LeaveDialog();
            }
        }

        private void MoveToNextDialogLine() // Move to next line without player choice (now used for dialog lines that aren't selected by player)
        {
            if (!_activeSentence.IsFinished) return; // If active sentence is not yet finished than we can't move to next line

            DialogLineScriptableObject nextLineConfig = _activeDialogLine.Choices[0].NextDialog; // Пока hard-coded значение 0 т.к. для этого нет никакой особенной игровой логики
            SelectDialogLine(nextLineConfig);
        }

        private void OnActiveSentenceFinishedHandler()
        {
            OnDialogSentenceFinished?.Invoke();
            _activeSentence.OnFinished -= OnActiveSentenceFinishedHandler;
        }

        private void TriggerDialogLineActions(DialogLine dialogLine)
        {
            foreach (DialogAction action in dialogLine.Actions)
            {
                action.Trigger(dialogLine.Speaker);
            }
        }

        protected abstract DialogLine GetDialogLine(DialogLineScriptableObject config);

        protected abstract Sentence GetDialogSentence(int sentenceIndex);

        public virtual void InitiateDialog()
        {
            SelectDialogLine(_dialogConfiguration.StartingDialogLine);

            OnDialogInitiated?.Invoke();
        }

        public virtual void LeaveDialog()
        {
            _activeDialogLine = null;

            OnDialogLeave?.Invoke();
        }

        public virtual void RestartDialogSentence()
        {
            SelectDialogSentence(_activeSentenceIndex);
        }

        public void RegisterDialogParticipant(Person person)
        {
            if (_dialogParticipants.Contains(person)) return;

            _dialogParticipants.Add(person);
            string id = person.GetComponent<UniqueIdentifier>().ID;

            foreach (DialogLine line in _dialogLines.Values)
            {
                if (line.SpeakerParticipant?.ParticipantId == id)
                {
                    line.Speaker = person;
                }

                if (line.ListenerParticipant?.ParticipantId == id)
                {
                    line.Listener = person;
                }
            }
        }

        public List<DialogChoiceData> GetNextDialogLineConfigs()
        {
            List<DialogChoiceData> choices = new List<DialogChoiceData>(_activeDialogLine.Choices.Count);

            foreach (DialogChoiceData choice in _activeDialogLine.Choices)
            {
                if (choice.NextDialog == null) continue;

                DialogLine nextLine = _dialogLines[choice.NextDialog];

                bool areAllConditionsFulfilled = true;
                foreach (DialogLineCondition condition in choice.Conditions)
                {
                    if (!condition.IsConditionFulfilled(nextLine))
                    {
                        areAllConditionsFulfilled = false;
                        break;
                    }
                }

                if (areAllConditionsFulfilled)
                {
                    choices.Add(choice);
                }
            }

            return choices;
        }

        public bool AreNextDialogLinesSelectedByPlayer()
        {
            foreach (DialogChoiceData choice in GetNextDialogLineConfigs())
            {
                if (!choice.NextDialog.IsSelectedByPlayer)
                {
                    return false;
                }
            }

            return true;
        }

        public void ContinueDialogChain(DialogLineScriptableObject selectedLineConfig = null)
        {
            int nextSentenceIndex = _activeSentenceIndex + 1;

            if (nextSentenceIndex < _activeDialogLine.Sentences.Count && _activeSentence.IsFinished)
            {
                SelectDialogSentence(nextSentenceIndex);
            }
            else
            {
                if (AreNextDialogLinesSelectedByPlayer())
                {
                    if (selectedLineConfig != null)
                    {
                        TriggerDialogLineActions(_activeDialogLine);
                        SelectDialogLine(selectedLineConfig);
                    }
                    else
                    {
                        Debug.Log("Need player input to procees dialog chain");
                    }
                }
                else
                {
                    TriggerDialogLineActions(_activeDialogLine);
                    MoveToNextDialogLine();
                }
            }
        }
    } 
}
