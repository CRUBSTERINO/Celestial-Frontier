using UnityEngine;

[CreateAssetMenu(fileName = "NPC Schedule Service Config", menuName = "Scriptables/NPC/NPC Schedule Service Config")]
public class NPCsScheduleServiceScriptableObject :ScriptableObject
{
    [SerializeField] private string _schedulesResourcesPath; // Path to all schedules in Resources folder

    public string SchedulesResourcesPath => _schedulesResourcesPath;
}