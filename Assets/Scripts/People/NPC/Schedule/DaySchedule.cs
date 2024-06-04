using System;

// Controls current activity related to world time
public class DaySchedule
{
    private DayScheduleScriptableObject _scheduleConfig; // Schedule config that defines activities of schedule
    private WorldTimeService _worldTimeService;
    private DayActivity _currentActivity;

    public DayScheduleScriptableObject ScheduleConfig => _scheduleConfig;

    public event Action<DayActivity> OnActivityChanged;

    public DaySchedule(WorldTimeService worldTimeService, DayScheduleScriptableObject scheduleConfig)
    {
        _worldTimeService = worldTimeService;
        _scheduleConfig = scheduleConfig;

        _worldTimeService.OnWorldTimeUpdated += UpdateActivity;
    }

    private void UpdateActivity(TimeData currentTime) // Checks for new avaliable activity each time when time changes
    {
        foreach (DayActivity activity in _scheduleConfig.Activities)
        {
            if (currentTime.IsInTimeInterval(activity.Interval) && activity != _currentActivity) // Time interval of activity should match to current local time and checked activity should differ from current
            {
                SetCurrentActivity(activity);
            }
        }
    }

    private void SetCurrentActivity(DayActivity activity)
    {
        DayActivity previousActivity = _currentActivity;
        _currentActivity = activity;
        OnActivityChanged?.Invoke(activity);
    }
}
