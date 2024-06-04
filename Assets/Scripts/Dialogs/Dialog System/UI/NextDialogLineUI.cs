using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem.UI
{
    public class NextDialogLineUI : DialogLineUI
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        [SerializeField] private Button _button;

        public event Action<DialogLineScriptableObject> OnDialogLineSelected;

        private void OnDialogLineSelectedHandler()
        {
            _button.onClick.RemoveAllListeners();
            OnDialogLineSelected?.Invoke(_choice.NextDialog);
        }

        public override void AssignDialogLineConfig(DialogChoiceData choice)
        {
            base.AssignDialogLineConfig(choice);

            _textMeshPro.text = _choice.Text;
            _button.onClick.AddListener(OnDialogLineSelectedHandler);
        }
    } 
}
