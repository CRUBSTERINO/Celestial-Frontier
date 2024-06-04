using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public abstract class Minigame : MonoBehaviour
{
    public event Action Launched;
    public event Action<bool> Finished;

    public async UniTask<bool> Launch()
    {
        UniTask<bool> task = GetMinigameResult();

        Launched?.Invoke();

        bool isSuccessful = await task;

        Finished?.Invoke(isSuccessful);

        Destroy(gameObject);

        return isSuccessful;
    }

    // This method should call all mini-game logic from this method and return whether it was successfully completed or not
    protected abstract UniTask<bool> GetMinigameResult();
}
