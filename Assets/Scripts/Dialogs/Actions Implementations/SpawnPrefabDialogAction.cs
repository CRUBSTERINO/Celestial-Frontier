using UnityEngine;

namespace DialogSystem.Actions
{
    // Spawns selected prefab in selected position when triggered
    public class SpawnPrefabDialogAction : DialogAction
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Vector3 _position;

        public override void Trigger(Person person)
        {
            Object.Instantiate(_prefab, _position, Quaternion.identity);
        }

        public override DialogAction CloneAction()
        {
            SpawnPrefabDialogAction action = new SpawnPrefabDialogAction()
            {
                _prefab = _prefab,
                _position = _position
            };

            return action;
        }
    }

}