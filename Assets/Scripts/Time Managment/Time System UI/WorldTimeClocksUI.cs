using UnityEngine;
using TMPro;

public class WorldTimeClocksUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hoursText;
    [SerializeField] private TextMeshProUGUI _minutesText;

    private WorldTimeService _worldTimeService;

    private void Start()
    {
        _worldTimeService = ServiceLocator.Instance.GetService<WorldTimeService>();

        _worldTimeService.OnWorldTimeUpdated += UpdateClockVisuals;
    }

    private void OnDestroy()
    {
        _worldTimeService.OnWorldTimeUpdated -= UpdateClockVisuals;
    }

    private void UpdateClockVisuals(TimeData time)
    {
        _hoursText.text = $"{time.Time.Hours.ToString("00")}";
        _minutesText.text = $"{time.Time.Minutes.ToString("00")}";
    }
}
