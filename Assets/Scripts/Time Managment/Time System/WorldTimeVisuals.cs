using UnityEngine;
using UnityEngine.Rendering.Universal;

// Visual representation of time on scene
// Can differ from scene to scene
public class WorldTimeVisuals : MonoBehaviour
{
    [SerializeField] private Gradient _timeOfDayGradient;
    [SerializeField] private Light2D _globalLightSource;

    private WorldTimeService _worldTime;

    private void Start()
    {
        _worldTime = ServiceLocator.Instance.GetService<WorldTimeService>();
        _worldTime.OnWorldTimeUpdated += AdjustVisualsToTime;
    }

    private void OnDestroy()
    {
        _worldTime.OnWorldTimeUpdated -= AdjustVisualsToTime;
    }

    private void AdjustVisualsToTime(TimeData timeData)
    {
        _globalLightSource.color = _timeOfDayGradient.Evaluate(timeData.GetPercentageOfDay());
    }
}
