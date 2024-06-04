using Cysharp.Threading.Tasks;
using UnityEngine;

public class MinigamesService : IService
{
    private MinigamesServiceScriptableObject _config;

    public MinigamesService(MinigamesServiceScriptableObject config)
    {
        _config = config;
    }

    public void OnStart()
    {

    }

    public void OnDestroy()
    {

    }

    public async UniTask<bool> RequestDefaultMinigame()
    {
        Minigame minigame = Object.Instantiate(_config.DefaultMinigamePrefab).GetComponent<Minigame>();
        MinigameUI minigameUI = Object.Instantiate(_config.DefaultMinigameUIPrefab).GetComponentInChildren<MinigameUI>();

        minigameUI.AssignMinigame(minigame);

        return await minigame.Launch();
    }
}
