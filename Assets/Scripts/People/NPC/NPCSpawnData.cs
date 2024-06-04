using UnityEngine;

public class NPCSpawnData
{
    private Vector3 _position;
    private GameObject _prefab;
    private DayScheduleScriptableObject _scheduleConfig;

    public Vector3 Position => _position;
    public GameObject Prefab => _prefab;
    public DayScheduleScriptableObject Schedule => _scheduleConfig;

    public NPCSpawnData(Vector3 position, GameObject prefab, DayScheduleScriptableObject schedule)
    {
        _position = position;
        _prefab = prefab;
        _scheduleConfig = schedule;
    }
}