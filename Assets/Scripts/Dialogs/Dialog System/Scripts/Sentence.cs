using System;
using UnityEngine;

namespace DialogSystem
{
    [Serializable]
    public class Sentence
    {
        [SerializeField, TextArea] private string _text;

        private bool _isFinished = true;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnChanged?.Invoke();
            }
        }
        public bool IsFinished
        {
            get => _isFinished;
            set
            {
                _isFinished = value;
                if (_isFinished)
                {
                    OnFinished?.Invoke();
                }
            }
        }

        public event Action OnChanged;
        public event Action OnFinished;
    } 
}
