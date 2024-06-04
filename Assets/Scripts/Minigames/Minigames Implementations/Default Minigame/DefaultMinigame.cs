using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DefaultMinigame : Minigame
{
    private const float MINIGAME_WIDTH = 0.12f;

    private DefaultMinigameSettings _settings;
    private float _currentValue;

    public DefaultMinigameSettings Settings => _settings;
    public float CurrentValue => _currentValue;

    public event Action<float> CurrentValueUpdated;

    private DefaultMinigameSettings GenerateSettings()
    {
        DefaultMinigameSettings settings = new DefaultMinigameSettings();

        settings.Speed = 1.5f;
        settings.AllowableError = 0.02f;
        settings.CeilingValue = 1f;

        settings.GoalMin = UnityEngine.Random.Range(0.3f, 0.88f);
        settings.GoalMax = settings.GoalMin + MINIGAME_WIDTH;

        return settings;
    }

    protected async override UniTask<bool> GetMinigameResult()
    {
        _settings = GenerateSettings();

        bool isSuccessful = false;

        while (_currentValue < _settings.CeilingValue)
        {
            _currentValue += _settings.Speed * Time.deltaTime;
            _currentValue = Mathf.Clamp(_currentValue, 0f, _settings.CeilingValue);
            CurrentValueUpdated?.Invoke(_currentValue);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }

            await UniTask.Yield();
        }

        if (_currentValue + _settings.AllowableError > _settings.GoalMin && _currentValue - _settings.AllowableError < _settings.GoalMax)
        {
            isSuccessful = true;
        }
        else
        {
            isSuccessful = false;
        }

        return isSuccessful;
    }
}
