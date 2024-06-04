using System.Collections.Generic;
using UnityEngine;

public class SceneExitFinderService : IService
{
    private SceneManagmentService _sceneManagment;
    private SceneExit[] _exitsOnActiveScene;
    private bool _areExitsUpToDate;

    private void FindExitsOnScene()
    {
        if (_areExitsUpToDate) return;

        _exitsOnActiveScene = Object.FindObjectsOfType<SceneExit>();
        _areExitsUpToDate = true;
    }

    private void FindExitsOnSceneLoadedHandler(SceneEntranceScriptableObject scene)
    {
        FindExitsOnScene();
    }

    private void OnSceneLoadingStartedHandler(SceneEntranceScriptableObject scene)
    {
        _areExitsUpToDate = false;
    }

    // Finds closest scene that should be entered to reach target scene using BFS
    private SceneScriptableObject FindClosestSceneToTarget(SceneScriptableObject targetScene)
    {
        SceneNode currentNode = new SceneNode(_sceneManagment.CurrentScene, null);

        // Scenes marked visited
        List<SceneScriptableObject> visitedScenes = new List<SceneScriptableObject>();
        // SceneNodes that are avaliable for visiting (already marked visited)
        Queue<SceneNode> openQueue = new Queue<SceneNode>();
        // SceneNodes that are visited
        Queue<SceneNode> closeQueue = new Queue<SceneNode>();

        // Enqueue root node
        openQueue.Enqueue(currentNode);

        // Loop through scenes until all scenes visited
        while (openQueue.Count > 0)
        {
            // Dequeue current node from open queue
            currentNode = openQueue.Dequeue();

            // If current node is target than build "path"
            if (currentNode.Scene == targetScene)
            {
                // Magic number 1. because we are looking for first scene that should entered. 0 is current scene
                return currentNode.GetPreviousScenesFromThisNode()[1];
            }

            // Enqueue current node to close queue
            closeQueue.Enqueue(currentNode);

            // Add all unvisited neighbours to open queue and mark as visited
            foreach (SceneScriptableObject sceneConfig in currentNode.Scene.ReachableScenes)
            {
                // If scene is already visited than skip it
                if (visitedScenes.Contains(sceneConfig))
                {
                    continue;
                }

                openQueue.Enqueue(new SceneNode(sceneConfig, currentNode));
                visitedScenes.Add(sceneConfig);
            }
        }

        Debug.LogWarning("Unable to find closest scene to target");
        return null;
    }

    public void OnDestroy()
    {
        _sceneManagment.OnGameplaySceneLoaded -= FindExitsOnSceneLoadedHandler;
        _sceneManagment.OnGameplaySceneLoadingStarted -= OnSceneLoadingStartedHandler;
    }

    public void OnStart()
    {
        _sceneManagment = ServiceLocator.Instance.GetService<SceneManagmentService>();
        _sceneManagment.OnGameplaySceneLoaded += FindExitsOnSceneLoadedHandler;
        _sceneManagment.OnGameplaySceneLoadingStarted += OnSceneLoadingStartedHandler;
    }

    public SceneExit FindSceneExit(Vector3 lookFromPosition, SceneScriptableObject lookedForScene)
    {
        if (!_areExitsUpToDate) FindExitsOnScene();

        SceneScriptableObject targetScene = FindClosestSceneToTarget(lookedForScene);

        List<SceneExit> suitableExits = new List<SceneExit>();

        foreach (SceneExit sceneExit in _exitsOnActiveScene)
        {
            if (sceneExit.TargetScene == targetScene)
            {
                suitableExits.Add(sceneExit);
            }
        }

        if (suitableExits.Count <= 0)
        {
            Debug.Log("Unable to find suitable scene exit.");
            return null;
        }

        SceneExit closestExit;

        if (suitableExits.Count > 1)
        {
            closestExit = suitableExits[0];
            float shortestDistance = (closestExit.transform.position - lookFromPosition).magnitude;

            for (int i = 1; i < suitableExits.Count; i++)
            {
                float comparedDistance = (suitableExits[i].transform.position - lookFromPosition).magnitude;

                if (comparedDistance < shortestDistance)
                {
                    closestExit = suitableExits[i];
                    shortestDistance = comparedDistance;
                }
            }
        }
        else
        {
            closestExit = suitableExits[0];
        }

        return closestExit;
    }
}
