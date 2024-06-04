using System;

public struct TimeData
{
    private TimeSpan _time;

    public TimeSpan Time => _time;

    public TimeData(int hours, int minutes)
    {
        _time = new TimeSpan(hours, minutes, 0);
    }

    public void AddTimeSpan(TimeSpan time)
    {
        _time = _time.Add(time);
    }

    public float GetPercentageOfDay()
    {
        float percentage = (float)(_time.TotalMinutes % TimeConstants.MINUTES_IN_DAY) / TimeConstants.MINUTES_IN_DAY;

        return percentage;
    }

    public bool IsInTimeInterval(TimeInterval interval)
    {
        interval.GetTimeInterval(out TimeSpan intervalStart, out TimeSpan intervalEnd);

        TimeSpan clampedTime = new TimeSpan(_time.Hours, _time.Minutes, 0); // Clamp the time (Remove year, month and day information)

        // Start is bigger than end, than time goes over midnight
        if (intervalStart.CompareTo(intervalEnd) > 0)
        {
            if (clampedTime.CompareTo(intervalStart) >= 0 || clampedTime.CompareTo(intervalEnd) < 0)
            {
                return true;
            }
        }
        else if (clampedTime.CompareTo(intervalStart) >= 0 && clampedTime.CompareTo(intervalEnd) < 0)
        {
            return true;
        }

        return false;
    }
}
