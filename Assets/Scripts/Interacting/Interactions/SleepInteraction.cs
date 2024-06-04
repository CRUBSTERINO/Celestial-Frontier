using System;
using UnityEngine;

[Serializable]
public class SleepInteraction : Interaction
{
    [SerializeField] private float _timeWarpingSpeed;
    [SerializeField] private int _warpToHours;
    [SerializeField] private int _warpToMinutes;

    private WorldTimeService _worldTimeService;

    public async override void PerformInteraction(Interactor interactor)
    {
        PlayerManager playerManager = interactor.transform.root.GetComponent<PlayerManager>();
        playerManager.FreezePlayer();

        _worldTimeService = ServiceLocator.Instance.GetService<WorldTimeService>();

        TimeSpan targetTime = new TimeSpan(_warpToHours, _warpToMinutes, 0);
        await _worldTimeService.WarpTime(_timeWarpingSpeed, targetTime);

        playerManager.UnfreezePlayer();
    }
}
