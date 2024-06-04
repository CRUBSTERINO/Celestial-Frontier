using Enemies.MeleeEnemy;
using UnityEngine;
using UnityEngine.UI;

public class DetectionIndicatorUI : MonoBehaviour
{
    [SerializeField] private MeleeEnemyManager _enemyManager;
    [SerializeField] private Image _indicatorOutlineImage;
    [SerializeField] private Image _indicatorFillImage;

    private bool _isIndicatorEnabled;

    private void Start()
    {
        DisableIndicator();
    }

    private void Update()
    {
        if (!_isIndicatorEnabled && _enemyManager.PlayerInViewTime > 0)
        {
            EnableIndicator();
        }
        else if (_isIndicatorEnabled && _enemyManager.PlayerInViewTime == 0)
        {
            DisableIndicator();
        }

        if (_isIndicatorEnabled)
        {
            _indicatorFillImage.fillAmount = _enemyManager.PlayerInViewTime / _enemyManager.PlayerDetectionTime;
        }
    }

    private void EnableIndicator()
    {
        _indicatorOutlineImage.enabled = true;
        _indicatorFillImage.enabled = true;

        _isIndicatorEnabled = true;
    }

    private void DisableIndicator()
    {
        _indicatorOutlineImage.enabled = false;
        _indicatorFillImage.enabled = false;

        _isIndicatorEnabled = false;
    }
}
