using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionalGameObjectSpawner : MonoBehaviour
{
    // Prefabs of GameObjects that should be instantiated
    [SerializeField] private List<GameObject> _gameObjectPrefabs;

    // GameObject is tried to be spawned only in Start()
    private void Start()
    {
        if (AreSpawnConditionsFulfilled())
        {
            InstantiateGameObjects();
        }
    }

    private void InstantiateGameObjects()
    {
        foreach (GameObject gameObject in _gameObjectPrefabs)
        {
            Instantiate(gameObject);
        }
    }

    // Iherited classes should make realization to define spawn conditions
    protected abstract bool AreSpawnConditionsFulfilled();
}
