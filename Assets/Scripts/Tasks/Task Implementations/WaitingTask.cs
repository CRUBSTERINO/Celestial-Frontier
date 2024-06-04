using System;
using UnityEngine;

// Player should wait some time in order to complete this task
public class WaitingTask : Task
{
    // Time that should elapse to complete task
    [SerializeField] private int _days;
    [SerializeField] private int _hours;
    [SerializeField] private int _minutes;

    // World time should be greater than this timeSpan for completion
    private TimeSpan _targetTimeSpan;
    private TimeSpan _currentTimeSpan;
    private WorldTimeService _worldTimeService;

    private void WorldTimeUpdatedHandler(TimeData timeData)
    {
        _currentTimeSpan = timeData.Time;
        TryComplete();
    }

    protected override bool AreCompletionRequirmentsFulfilled()
    {
        return _targetTimeSpan.CompareTo(_currentTimeSpan) <= 0;
    }

    protected override void SubscribeToCompletionRequirments()
    {
        _worldTimeService = ServiceLocator.Instance.GetService<WorldTimeService>();
        _targetTimeSpan = _worldTimeService.WorldTime.Time.Add(new TimeSpan(_days, _hours, _minutes, 0));

        _worldTimeService.OnWorldTimeUpdated += WorldTimeUpdatedHandler;
    }

    protected override void UnsubscribeFromCompletionRequirments()
    {
        _worldTimeService.OnWorldTimeUpdated -= WorldTimeUpdatedHandler;
    }
}
