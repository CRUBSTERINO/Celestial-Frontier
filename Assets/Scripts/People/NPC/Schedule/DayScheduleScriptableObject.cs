using System.Collections.Generic;
using UnityEngine;

// Config of a schedule for NPC. Activities should be added and placed in list relative to their time (earliear - at start of the list)
[CreateAssetMenu(fileName = "NPC Schedule", menuName = "Scriptables/NPC/Schedule")]
public class DayScheduleScriptableObject : ScriptableObject
{
    [SerializeField] private List<DayActivity> _activities;
    [SerializeField] private GameObject _npcPrefab;
    // If "NPCSpawnerService" should check this schedule when looking for NPCs that should be spawned
    // True if it is regular schedule for NPC
    // False if this schedule is conditional
    [SerializeField] private bool _shouldBeChecked = true;

    public List<DayActivity> Activities => _activities;
    public GameObject NPCPrefab => _npcPrefab;
    public bool ShouldBeChecked => _shouldBeChecked;

    public DayActivity GetActivityAtTime(TimeData time)
    {
        foreach (DayActivity activity in _activities)
        {
            if (time.IsInTimeInterval(activity.Interval)) // Time interval of activity should match to given time
            {
                return activity;
            }
        }

        return null;
    }

    public DayActivity GetPreviousActivity(DayActivity activity)
    {
        int currentActivityIndex = _activities.IndexOf(activity);

        return (currentActivityIndex > 0) ? _activities[currentActivityIndex - 1] : _activities[_activities.Count - 1];
    }
}
