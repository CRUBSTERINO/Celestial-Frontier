using TMPro;
using UnityEngine;

namespace DialogSystem.UI
{
    public class ActiveDialogSentenceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        private Sentence _activeSentence;

        private void DrawActiveSentence()
        {
            _textMeshPro.text = _activeSentence.Text;
        }

        public void AssignDialogSentence(Sentence sentence)
        {
            if (_activeSentence != null)
            {
                _activeSentence.OnChanged -= DrawActiveSentence;
            }

            _activeSentence = sentence;
            DrawActiveSentence();
            sentence.OnChanged += DrawActiveSentence;
        }

        public void Clear()
        {
            _textMeshPro.text = "";
        }
    } 
}
