using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class WorldTimeService : IService
{
    private TimeData _worldTime;
    private float _timeRate;
    private bool _isTimePaused;
    private CancellationTokenSource _cancellationTokenSource;

    public TimeData WorldTime => _worldTime;

    public event Action<TimeData> OnWorldTimeUpdated;

    public WorldTimeService(WorldTimeServiceScriptableObject config)
    {
        _timeRate = config.TimeRate;
        _worldTime.AddTimeSpan(TimeSpan.FromHours(config.InitialTime.x));
        _worldTime.AddTimeSpan(TimeSpan.FromMinutes(config.InitialTime.y));
    }

    public void OnStart()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        ElapseGameMinute(_cancellationTokenSource.Token).Forget();
    }

    public void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
    }

    private async UniTask ElapseGameMinute(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // Wenn die Zeit angehalten wurde, dann warten, bis die Zeit wiederhergestellt ist.
            if (_isTimePaused)
            {
                await UniTask.WaitUntil(() => !_isTimePaused);
            }

            // Warten auf die Anzahl der Sekunden, die 60 Sekunden / Zeitflussrate entspricht.
            await UniTask.WaitForSeconds(TimeConstants.SECONDS_IN_MINUTE / _timeRate);

            // Hinzufügen einer Minute zur Spielzeit
            _worldTime.AddTimeSpan(TimeSpan.FromMinutes(1));

            // Aufrufen eines Events, das eine Aktualisierung der Spielzeit meldet.
            OnWorldTimeUpdated?.Invoke(_worldTime);
        }
    }

    public void FreezeTime()
    {
        _isTimePaused = true;
    }

    public void UnfreezeTime()
    {
        _isTimePaused = false;
    }

    public async UniTask WarpTime(float warpingSpeed, TimeSpan targetTime)
    {
        float defaultTimeRate = _timeRate;

        _timeRate = warpingSpeed;

        if (_worldTime.Time.CompareTo(targetTime) > 0)
        {
            targetTime = targetTime.Add(TimeSpan.FromDays(_worldTime.Time.Days + 1));
        }

        await UniTask.WaitUntil(() => _worldTime.Time.CompareTo(targetTime) >= 0);

        _timeRate = defaultTimeRate;
    }
}
