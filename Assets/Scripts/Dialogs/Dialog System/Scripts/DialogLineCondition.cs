// Inherited classes must implement different conditions that must be fulfilled in order to access dialog line
// Any conditions on a dialog line must be exclusively on lines in which the "Speaker" is the player.
using System;
using System.Collections.Generic;

namespace DialogSystem
{
	[Serializable]
	public abstract class DialogLineCondition
	{
		public abstract bool IsConditionFulfilled(DialogLine dialogLine);

		public abstract DialogLineCondition CloneCondition();

		public static List<DialogLineCondition> CloneConditions(List<DialogLineCondition> conditions)
		{
            List<DialogLineCondition> clonedConditions = new List<DialogLineCondition>(conditions.Count);

            foreach (DialogLineCondition condition in conditions)
            {
                clonedConditions.Add(condition.CloneCondition());
            }

            return clonedConditions;
        }
	} 
}
