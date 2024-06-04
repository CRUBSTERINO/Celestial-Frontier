using UnityEngine;

namespace DialogSystem.Actions
{
    public class LoadSceneDialogAction : DialogAction
    {
        [SerializeField] private SceneEntranceScriptableObject _targetSceneEntrance;

        public override void Trigger(Person person)
        {
            SceneManagmentService sceneService = ServiceLocator.Instance.GetService<SceneManagmentService>();
            sceneService.LoadScene(_targetSceneEntrance);
        }

        public override DialogAction CloneAction()
        {
            LoadSceneDialogAction action = new LoadSceneDialogAction()
            {
                _targetSceneEntrance = _targetSceneEntrance,
            };

            return action;
        }
    }
}
