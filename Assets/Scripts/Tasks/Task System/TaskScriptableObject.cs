using NaughtyAttributes;
using UnityEngine;

// Configuration of Task. Is also used to choose aproptiate task prefab with help of "TaskService"
[CreateAssetMenu(fileName = "Task Config", menuName = "Scriptables/Task System/Task Config")]
public class TaskScriptableObject : ScriptableObject
{
    [SerializeField, ResizableTextArea] private string _description;

    public string Description => _description;
}
