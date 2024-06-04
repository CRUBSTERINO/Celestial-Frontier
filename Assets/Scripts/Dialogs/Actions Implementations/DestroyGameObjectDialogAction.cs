using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Actions
{
    // When triggered destroys all gameObjects of unique selected ID's
    public class DestroyGameObjectDialogAction : DialogAction
    {
        // Unique identifiers of gameObjects that should be destroyed
        [SerializeField] private List<string> _gameObjectIDs;

        public override void Trigger(Person person)
        {
            foreach (string id in _gameObjectIDs)
            {
                GameObject gameObjectToDestroy = UniqueIdentifier.FindObjectWithID(id)?.gameObject;
                Object.Destroy(gameObjectToDestroy);
            }
        }

        public override DialogAction CloneAction()
        {
            DestroyGameObjectDialogAction action = new DestroyGameObjectDialogAction()
            {
                _gameObjectIDs = _gameObjectIDs
            };

            return action;
        }
    }
}
