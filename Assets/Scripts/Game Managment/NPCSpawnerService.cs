using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Controls NPCs present in the scene and that should be present based on schedules
// When NPC should appear on scene, this class creates them
public class NPCSpawnerService : IService
{
    private NPCsScheduleServiceScriptableObject _config;
    private DayScheduleScriptableObject[] _npcsSchedules;
    private WorldTimeService _worldTimeService;
    private SceneExitFinderService _sceneExitFinderService;
    private SceneScriptableObject _activeScene;
    private Dictionary<DayScheduleScriptableObject, NpcManager> _npcInstances;

    public NPCSpawnerService(NPCsScheduleServiceScriptableObject config)
    {
        _config = config;
        _npcInstances = new Dictionary<DayScheduleScriptableObject, NpcManager>();
    }

    private List<DayScheduleScriptableObject> GetAllNPCsSchedulesOnSceneAtTime(SceneScriptableObject scene, TimeData time)
    {
        List<DayScheduleScriptableObject> schedules = new List<DayScheduleScriptableObject>();

        foreach (DayScheduleScriptableObject schedule in _npcsSchedules)
        {
            DayActivity npcsActivity = schedule.GetActivityAtTime(time);

            if (npcsActivity == null) continue;

            if (npcsActivity.Scene == scene)
            {
                schedules.Add(schedule);
            }
        }

        return schedules;
    }

    private void InstantiateNPC(NPCSpawnData spawnData)
    {
        NpcManager npcManager = Object.Instantiate(spawnData.Prefab, spawnData.Position, Quaternion.identity).GetComponent<NpcManager>();
        npcManager.SetupSchedule(_worldTimeService, spawnData.Schedule);

        if (_npcInstances.ContainsValue(npcManager))
        {
            Debug.LogWarning("NPC that you want to instantiate is already in list of instantiated NPCs.");
            Object.Destroy(npcManager.gameObject);
        }

        _npcInstances.Add(spawnData.Schedule, npcManager);
        npcManager.OnDestroyed += DestroyNPC;
    }

    private void DestroyNPC(NpcManager npcManager)
    {
        npcManager.OnDestroyed -= DestroyNPC;

        _npcInstances.Remove(npcManager.DaySchedule.ScheduleConfig);
    }

    // Check schedules each time when world time updates. Spawns NPCs if needed.
    // "spawnNPCsInTheirLocation" = true than NPC will be spawn in location that is defined by their DayActivity. Otherwise they will be spawned near scene exit and walk from there.
    private void CheckSchedules(TimeData time, bool spawnNPCsInTheirLocations) 
    {
        List<DayScheduleScriptableObject> schedules = GetAllNPCsSchedulesOnSceneAtTime(_activeScene, time);

        foreach (DayScheduleScriptableObject schedule in schedules)
        {
            if (!_npcInstances.ContainsKey(schedule))
            {
                DayActivity activity = schedule.GetActivityAtTime(time);

                Vector3 location = activity.Location;
                if (!spawnNPCsInTheirLocations)
                {
                    SceneExit exit = _sceneExitFinderService.FindSceneExit(Vector3.zero, schedule.GetPreviousActivity(activity)?.Scene);
                    if (exit != null)
                    {
                        location = exit.ExitPosition;
                    }
                }

                NPCSpawnData spawnData = new NPCSpawnData(location, schedule.NPCPrefab, schedule);
                InstantiateNPC(spawnData);
            }
        }
    }

    private void CheckSchedulesOnWorldTimeUpdated(TimeData time)
    {
        CheckSchedules(time, false);
    }

    public void OnDestroy()
    {
        _worldTimeService.OnWorldTimeUpdated -= CheckSchedulesOnWorldTimeUpdated;
    }

    public void OnStart()
    {
        _npcsSchedules = Resources.LoadAll<DayScheduleScriptableObject>(_config.SchedulesResourcesPath);

        _npcsSchedules = _npcsSchedules.Where(schedule => schedule.ShouldBeChecked).ToArray();

        _worldTimeService = ServiceLocator.Instance.GetService<WorldTimeService>();
        _sceneExitFinderService = ServiceLocator.Instance.GetService<SceneExitFinderService>();

        _worldTimeService.OnWorldTimeUpdated += CheckSchedulesOnWorldTimeUpdated;
    }

    public void SetActiveScene(SceneScriptableObject scene)
    {
        _activeScene = scene;
        CheckSchedules(_worldTimeService.WorldTime, true);
    }
}
