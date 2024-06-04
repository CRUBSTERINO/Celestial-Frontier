using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    // Saves the Graph for later opening in GraphView
    // Here the dialoges are divided into groups, and there are also ungrouped dialoges available
    public class GraphEditorDataScriptableObject : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<PersonBlackboardFieldEditorData> BlackboardField { get; set; } // Save data of all fields in blackboard
        [field: SerializeField] public List<GroupEditorData> Groups { get; set; } // Save data of all groups
        [field: SerializeField] public List<DialogLineNodeEditorData> DialogNodes { get; set; } // Save data of all DialogNodes
        [field: SerializeField] public List<PersonNodeEditorData> PersonNodes { get; set; } // Save data of all PersonNodes
        [field: SerializeField] public List<ConditionNodeEditorData> ConditionNodes { get; set; } // Saved data of all ConditionNodes
        [field: SerializeField] public List<ActionNodeEditorData> ActionNodes { get; set; } // Saved data of all ActionNodes

        public void Initialize(string fileName)
        {
            FileName = fileName;

            BlackboardField = new List<PersonBlackboardFieldEditorData>();
            Groups = new List<GroupEditorData>();
            DialogNodes = new List<DialogLineNodeEditorData>();
            PersonNodes = new List<PersonNodeEditorData>();
            ConditionNodes = new List<ConditionNodeEditorData>();
            ActionNodes = new List<ActionNodeEditorData>();
        }
    } 
}