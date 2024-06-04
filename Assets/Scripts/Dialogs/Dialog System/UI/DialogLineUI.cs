using UnityEngine;

namespace DialogSystem.UI
{
    public class DialogLineUI : MonoBehaviour
    {
        protected DialogChoiceData _choice;

        public virtual void AssignDialogLineConfig(DialogChoiceData choice)
        {
            _choice = choice;
        }
    } 
}
