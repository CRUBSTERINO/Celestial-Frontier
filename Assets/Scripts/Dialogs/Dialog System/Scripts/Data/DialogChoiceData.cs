using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
	[Serializable]
	public class DialogChoiceData
	{
		[field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DialogLineScriptableObject NextDialog { get; set; }
        [field: SerializeReference] public List<DialogLineCondition> Conditions { get; set; }
    }
}