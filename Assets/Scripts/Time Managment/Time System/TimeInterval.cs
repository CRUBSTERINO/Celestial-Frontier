using System;
using UnityEngine;

[Serializable]
public struct TimeInterval
{
    [SerializeField] private int _fromHours;
    [SerializeField] private int _fromMinutes;
    [Space]
    [SerializeField] private int _toHours;
    [SerializeField] private int _toMinutes;

    public void GetTimeInterval(out TimeSpan intervalStart, out TimeSpan intervalEnd)
    {
        intervalStart = new TimeSpan(_fromHours, _fromMinutes, 0); // ��� ���� �������� DateTime �� TimeSpan
        intervalEnd = new TimeSpan(_toHours, _toMinutes, 0);
    }
}
