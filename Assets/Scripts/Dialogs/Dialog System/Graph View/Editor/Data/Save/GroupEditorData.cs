using System;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    // Saving data of group
    [Serializable]
    public class GroupEditorData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    } 
}