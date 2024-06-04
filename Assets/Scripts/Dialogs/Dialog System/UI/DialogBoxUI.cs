using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem.UI
{
    public class DialogBoxUI : MonoBehaviour
    {
        [SerializeField] private ActiveDialogSentenceUI _activeDialogSentenceUi;
        [SerializeField] private GameObject _nextDialogLinePrefab;
        [SerializeField] private Transform _nextDialogLineParent;
        [SerializeField] private TextMeshProUGUI _speakerNameTextMeshPro;
        [SerializeField] private Image _speakerPortraitImage;
        [SerializeField] private Button _restartDialogSentenceButton;

        private DialogManager _dialogManager;
        private List<NextDialogLineUI> _dialogLineUiInstances;

        private void AssignDialogSentence(Sentence sentence)
        {
            DestroyNextLinesUIInstances();
            _activeDialogSentenceUi.AssignDialogSentence(sentence);

            if (_dialogManager.AreNextDialogLinesSelectedByPlayer())
            {
                if (_dialogManager.IsOnLastSentenceInLine && _dialogManager.IsSentenceFinished)
                {
                    DrawNextDialogLines();
                }
                else if (_dialogManager.IsOnLastSentenceInLine && !_dialogManager.IsSentenceFinished)
                {
                    _dialogManager.OnDialogSentenceFinished += DrawNextDialogLinesOnSentenceFinishedHandler;
                }
            }
        }

        private void DrawDialogBox(DialogLineScriptableObject dialogLineConfig, DialogLine activeDialogLine)
        {
            ClearDialogBox();

            if (activeDialogLine == null) return;

            _speakerNameTextMeshPro.text = activeDialogLine.Speaker.Name;
            _speakerPortraitImage.sprite = activeDialogLine.Speaker.Portrait;
        }

        // Draw the list of avaliable dialog options for player to choose.
        private void DrawNextDialogLines()
        {
            List<DialogChoiceData> choices = _dialogManager.GetNextDialogLineConfigs();

            _dialogLineUiInstances = new List<NextDialogLineUI>(choices.Count);
            for (int i = 0; i < choices.Count; i++)
            {
                NextDialogLineUI dialogLineUiInstance = Instantiate(_nextDialogLinePrefab, _nextDialogLineParent).GetComponent<NextDialogLineUI>();
                dialogLineUiInstance.AssignDialogLineConfig(choices[i]);
                dialogLineUiInstance.OnDialogLineSelected += SelectDialogLine;
                _dialogLineUiInstances.Add(dialogLineUiInstance);
            }
        }

        private void DrawNextDialogLinesOnSentenceFinishedHandler()
        {
            DrawNextDialogLines();
            _dialogManager.OnDialogSentenceFinished -= DrawNextDialogLinesOnSentenceFinishedHandler;
        }

        private void RestartCurrentSentence()
        {
            _dialogManager.OnDialogSentenceFinished -= DrawNextDialogLinesOnSentenceFinishedHandler;
            _dialogManager.RestartDialogSentence();
        }

        private void ClearDialogBox()
        {
            DestroyNextLinesUIInstances();

            _speakerNameTextMeshPro.text = "";
            _activeDialogSentenceUi.Clear();
        }

        private void DestroyNextLinesUIInstances()
        {
            if (_dialogLineUiInstances != null)
            {
                foreach (var dialogLineInstance in _dialogLineUiInstances)
                {
                    dialogLineInstance.OnDialogLineSelected -= SelectDialogLine;
                    Destroy(dialogLineInstance.gameObject);
                }
                _dialogLineUiInstances.Clear();
            }
        }

        private void LeaveDialog()
        {
            ClearDialogBox();
            _dialogManager.OnDialogLineSelected -= DrawDialogBox;
            _dialogManager.OnDialogSentenceSelected -= AssignDialogSentence;
            _dialogManager.OnDialogLeave -= LeaveDialog;
            _restartDialogSentenceButton.onClick.RemoveListener(RestartCurrentSentence);
            _dialogManager = null;
        }

        private void SelectDialogLine(DialogLineScriptableObject lineConfig)
        {
            _dialogManager.ContinueDialogChain(lineConfig);
        }

        public void AssignDialogManager(DialogManager manager)
        {
            _dialogManager = manager;
            _dialogManager.OnDialogLineSelected += DrawDialogBox;
            _dialogManager.OnDialogSentenceSelected += AssignDialogSentence;
            _dialogManager.OnDialogLeave += LeaveDialog;
            _restartDialogSentenceButton.onClick.AddListener(RestartCurrentSentence);
        }
    }

}