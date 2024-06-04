using DialogSystem;
using System;
using UnityEngine;

public class NpcManager : Person
{
    [SerializeField] private PathfindingAgent _agent;
    [SerializeField] private Interactor _interactor;
    [SerializeField] private DialogManager _dialogManager;
    
    private DaySchedule _daySchedule;
    private SceneExitFinderService _sceneExitFinderService;
    private IInteractable _targetInteractable;
    private SceneScriptableObject _currentScene;

    public Vector3 InteractionPosition => transform.position;
    public DaySchedule DaySchedule => _daySchedule;

    public event Action<NpcManager> OnDestroyed; // For now with event. Don't know how it should be better for NPCSpawnerService to record when npc leaves scene
    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    private void Start()
    {
        _sceneExitFinderService = ServiceLocator.Instance.GetService<SceneExitFinderService>();

        _currentScene = ServiceLocator.Instance.GetService<SceneManagmentService>().CurrentScene;

        _dialogManager.OnDialogInitiated += OnDialogInitiatedHandler;
        _dialogManager.OnDialogLeave += OnDialogLeaveHandler;
    }

    private void OnDestroy()
    {
        _daySchedule.OnActivityChanged -= AdjustToActivity;
        _dialogManager.OnDialogInitiated -= OnDialogInitiatedHandler;
        _dialogManager.OnDialogLeave -= OnDialogLeaveHandler;

        OnDestroyed?.Invoke(this);
        OnBecameNonInteractable?.Invoke();
    }

    private void OnDialogInitiatedHandler()
    {
        _agent.IsStopped = true;
    }

    private void OnDialogLeaveHandler()
    {
        _agent.IsStopped = false;
    }

    private void AdjustToActivity(DayActivity currentActivity)
    {
        if (_currentScene != currentActivity.Scene)
        {
            SceneExit sceneExit = _sceneExitFinderService.FindSceneExit(transform.position, currentActivity.Scene);

            if (sceneExit != null)
            {
                HeadToSceneExit(sceneExit);
                return;
            }
        }

        _agent.SetDestination(currentActivity.Location); // For now only set destination
    }

    private void HeadToSceneExit(SceneExit exit)
    {
        _agent.SetDestination(exit.ExitPosition);

        _agent.OnPathCompleted += LeaveScene;
    }

    private void LeaveScene()
    {
        _agent.OnPathCompleted -= LeaveScene;

        Destroy(gameObject);
    }

/*    private void SetTargetInteractable(IInteractable interactable) // To work should add NPCInteractor to prefab. Functionality of interaction is not yet complete.
    {
        _agent.SetDestination(interactable.InteractionPosition);
        _targetInteractable = interactable;

        _agent.OnPathFinished += InteractWithTargetInteractable;
    }

    private void InteractWithTargetInteractable()
    {
        _interactor.InteractWithGivenInteractable(_targetInteractable);
        _targetInteractable = null;
    }*/

    public void SetupSchedule(WorldTimeService worldTimeService, DayScheduleScriptableObject scheduleScriptableObject)
    {
        if (_daySchedule != null)
        {
            _daySchedule.OnActivityChanged -= AdjustToActivity;
        }

        _daySchedule = new DaySchedule(worldTimeService, scheduleScriptableObject);
        _daySchedule.OnActivityChanged += AdjustToActivity;
    }
}
