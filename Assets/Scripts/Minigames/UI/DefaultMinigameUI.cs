using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DefaultMinigameUI : MinigameUI
{
    [SerializeField] private RectTransform _currentValueTransform;
    [SerializeField] private RectTransform _goalRectTransform;
    [SerializeField] private Image _minigameSpaceImage;
    [SerializeField] private float _sizeMultiplier;
    [Space]
    [SerializeField] private Color _successfulColor;
    [SerializeField] private Color _unsuccessfulColor;

    private DefaultMinigame _minigame;
    private DefaultMinigameSettings _settings;

    public override void AssignMinigame(Minigame minigame)
    {
        _minigame = minigame as DefaultMinigame;

        _minigame.Launched += SetupMinigameUI;
        _minigame.Finished += OnFinishedHandler;
    }

    private void SetupMinigameUI()
    {
        _settings = _minigame.Settings;

        _minigame.Launched -= SetupMinigameUI;

        _currentValueTransform.anchoredPosition = new Vector2(_minigame.CurrentValue, 0f);
        _currentValueTransform.sizeDelta = new Vector2(_settings.AllowableError * _sizeMultiplier, _currentValueTransform.sizeDelta.y);
        _currentValueTransform.ForceUpdateRectTransforms();

        _goalRectTransform.anchoredPosition = new Vector2((_settings.GoalMin + _settings.GoalMax) * _sizeMultiplier / 2f, 0);
        _goalRectTransform.sizeDelta = new Vector2((_settings.GoalMax - _settings.GoalMin) * _sizeMultiplier, _goalRectTransform.sizeDelta.y);
        _goalRectTransform.ForceUpdateRectTransforms();

        _minigame.CurrentValueUpdated += UpdateCurrentValue;
    }

    private void UpdateCurrentValue(float currentValue)
    {
        _currentValueTransform.anchoredPosition = new Vector2(_minigame.CurrentValue * _sizeMultiplier, 0f);
        _goalRectTransform.ForceUpdateRectTransforms();
    }

    private async void OnFinishedHandler(bool isSuccessful)
    {
        _minigame.Finished -= OnFinishedHandler;
        _minigame.CurrentValueUpdated -= UpdateCurrentValue;

        _minigameSpaceImage.color = (isSuccessful) ? _successfulColor : _unsuccessfulColor;

        await UniTask.WaitForSeconds(0.8f);

        Destroy(transform.root.gameObject);
    }
}
