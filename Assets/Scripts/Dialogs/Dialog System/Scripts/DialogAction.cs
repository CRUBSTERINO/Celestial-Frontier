using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    // Is MonoBehaviour to try to evade cyclic dependencies (might not work with visitor pattern)
    [Serializable]
    public abstract class DialogAction
    {
        public virtual event Action OnTriggered;
        public event Action OnDestroyed;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }

        protected void OnActionTriggered()
        {
            OnTriggered?.Invoke();
        }

        public abstract void Trigger(Person person);

        public abstract DialogAction CloneAction();

        public static List<DialogAction> CloneActions(List<DialogAction> actions)
        {
            List<DialogAction> clonedActions = new List<DialogAction>(actions.Count);

            foreach (DialogAction action in actions)
            {
                clonedActions.Add(action.CloneAction());
            }

            return clonedActions;
        }
    } 
}
