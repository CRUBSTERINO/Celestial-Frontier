using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskManagerUI : MonoBehaviour
{
    [SerializeField] private TaskManager _taskManager;
    [SerializeField] private RectTransform _taskDescriptionContentTransform;
    [SerializeField] private GameObject _descriptionPrefab;

    private Dictionary<Task, GameObject> _descriptionInstances;

    private void Start()
    {
        _descriptionInstances = new Dictionary<Task, GameObject>();

        ClearTaskUI();

        _taskManager.OnTaskAssigned += AddTaskToUI;
        _taskManager.OnTaskCompleted += RemoveTaskFromUI;
    }

    private void OnDestroy()
    {
        _taskManager.OnTaskAssigned -= AddTaskToUI;
        _taskManager.OnTaskCompleted -= RemoveTaskFromUI;
    }

    private void AddTaskToUI(Task task)
    {
        TextMeshProUGUI text = InstantiateDescriptionPrefab();
        _descriptionInstances.Add(task, text.transform.parent.gameObject);

        text.text = task.Config.Description;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_taskDescriptionContentTransform);
    }

    private void RemoveTaskFromUI(Task task)
    {
        if (_descriptionInstances.ContainsKey(task))
        {
            Destroy(_descriptionInstances[task]);
            _descriptionInstances.Remove(task);
        }
    }

    private TextMeshProUGUI InstantiateDescriptionPrefab()
    {
        return Instantiate(_descriptionPrefab, _taskDescriptionContentTransform).GetComponentInChildren<TextMeshProUGUI>();
    }

    private void ClearTaskUI()
    {
        foreach (GameObject instance in _descriptionInstances.Values)
        {
            Destroy(instance);
        }
    }
}
