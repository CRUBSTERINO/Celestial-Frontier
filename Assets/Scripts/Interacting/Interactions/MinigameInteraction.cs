using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[Serializable]
public class MinigameInteraction : Interaction
{
    [SerializeField] private int _minigamesNumber;
    // For now minigames only complete tasks
    [SerializeField] private TaskScriptableObject _taskToComplete;

    // Number of completed minigames in a sequence
    // Should be reseted if minigame is failed
    private int _completedMinigames;

    public override void PerformInteraction(Interactor interactor)
    {
        MinigamesService minigamesService = ServiceLocator.Instance.GetService<MinigamesService>();
        PlayerManager playerManager = interactor.transform.root.GetComponent<PlayerManager>();

        FireMinigameTask(minigamesService, playerManager);
    }

    private async void FireMinigameTask(MinigamesService minigamesService, PlayerManager playerManager)
    {
        bool isSuccessful = await minigamesService.RequestDefaultMinigame();

        Debug.Log(isSuccessful);

        if (isSuccessful)
        {
            _completedMinigames++;

            if (_completedMinigames == _minigamesNumber)
            {
                playerManager.TaskManager.CompleteTask(_taskToComplete);
            }
            else
            {
                await UniTask.WaitForSeconds(1f);

                FireMinigameTask(minigamesService, playerManager);
            }
        }
        else
        {
            _completedMinigames = 0;

            await UniTask.WaitForSeconds(1f);

            FireMinigameTask(minigamesService, playerManager);
        }
    }
}
