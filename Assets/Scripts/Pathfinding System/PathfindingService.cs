using System.Collections.Generic;
using UnityEngine;

public class PathfindingService : IService
{
    private const int STRAIGHT_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;
    private const int MAXIMUM_ITERATIONS = 1200;

    private PathfindingSurface _surface;
    private Grid<PathfindingNode> _grid; // Might be temp if _surface has no other usages
    private bool _isSurfaceLatest;

    public PathfindingSurface Surface => _surface;

    public void OnStart()
    {
        ServiceLocator.Instance.GetService<SceneManagmentService>().OnGameplaySceneLoaded += GameplaySceneLoadedHandler;
        ServiceLocator.Instance.GetService<SceneManagmentService>().OnGameplaySceneLoadingStarted += GameplaySceneLoadingStartedHandler;

    }

    public void OnDestroy()
    {
        ServiceLocator.Instance.GetService<SceneManagmentService>().OnGameplaySceneLoaded -= GameplaySceneLoadedHandler;
        ServiceLocator.Instance.GetService<SceneManagmentService>().OnGameplaySceneLoadingStarted -= GameplaySceneLoadingStartedHandler;
    }

    private void GameplaySceneLoadedHandler(SceneEntranceScriptableObject config)
    {
        if (_isSurfaceLatest) return;

        UpdatePathfindingSurface();
    }

    private void GameplaySceneLoadingStartedHandler(SceneEntranceScriptableObject config)
    {
        _isSurfaceLatest = false;
    }

    private void UpdatePathfindingSurface() // Find and assign new PathfindingSurface that exists in current scene
    {
        _surface = Object.FindObjectOfType<PathfindingSurface>();
        _grid = _surface.Grid;

        _isSurfaceLatest = true;
    }

    private List<PathfindingNode> GetNeighboursList(PathfindingNode node) // Return nodes that are neighbours of the given
    {
        List<PathfindingNode> neighbours = new List<PathfindingNode>();

        if (node.X - 1 >= 0)
        {
            // Left
            neighbours.Add(GetNode(node.X - 1, node.Y));
            // Top Left
            if (node.Y + 1 < _grid.Height)
            {
                neighbours.Add(GetNode(node.X - 1, node.Y + 1));
            }
            // Bottom Left
            if (node.Y - 1 >= 0)
            {
                neighbours.Add(GetNode(node.X - 1, node.Y - 1));
            }
        }

        if (node.X + 1 < _grid.Width)
        {
            // Right
            neighbours.Add(GetNode(node.X + 1, node.Y));
            // Top Right
            if (node.Y + 1 < _grid.Height)
            {
                neighbours.Add(GetNode(node.X + 1, node.Y + 1));
            }
            // Bottom Right
            if (node.Y - 1 >= 0)
            {
                neighbours.Add(GetNode(node.X + 1, node.Y - 1));
            }
        }

        if (node.Y + 1 < _grid.Height)
        {
            // Top
            neighbours.Add(GetNode(node.X, node.Y + 1));
        }

        if (node.Y - 1 >= 0)
        {
            // Top
            neighbours.Add(GetNode(node.X, node.Y - 1));
        }

        return neighbours;
    }

    private PathfindingNode GetNode(int x, int y) // Return node by it's local coordinates in grid
    {
        return _grid.GetGridObject(x, y);
    }

    private Vector3 GetNodeWorldPosition(int x, int y) // Get world position of node by it's local coordinates
    {
        return _grid.GetWorldPosition(x, y);
    }

    private PathfindingNode GetLowestFCostNode(List<PathfindingNode> pathNodesList) // Return node with the lowest F cost in the list
    {
        PathfindingNode lowestFCostNode = pathNodesList[0];

        // Cycle through the given list to determine the node with lowest F cost
        for (int i = 1; i <  pathNodesList.Count; i++)
        {
            if (pathNodesList[i].FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = pathNodesList[i];
            }
        }

        return lowestFCostNode;
    }

    private int CalculateRelocationCost(PathfindingNode from, PathfindingNode to) // Calculate the cost to move from "from" node to "to" node based on movement cost
    {
        int xDistance = Mathf.Abs(to.X - from.X);
        int yDistance = Mathf.Abs(to.Y - from.Y);
        int straightDistance = Mathf.Abs(xDistance - yDistance);
        int diagonalDistance = Mathf.Min(xDistance, yDistance);

        return DIAGONAL_MOVE_COST * diagonalDistance + STRAIGHT_MOVE_COST * straightDistance;
    }

    private PathfindingPath CreatePath(PathfindingNode endNode) // Creating a path by retracing comeFromNodes from the endNode
    {
        List<Vector3> positions = new List<Vector3>();

        PathfindingNode currentPathNode = endNode;

        // Retrace all cameFromNodes beginning from endNode and adding their world positions to the positions list
        while (currentPathNode.CameFromNode != null)
        {
            positions.Add(GetNodeWorldPosition(currentPathNode.X, currentPathNode.Y));
            currentPathNode = currentPathNode.CameFromNode;
        }

        PathfindingPath path = new PathfindingPath(positions);
        path.Inverse();
        
        return path;
    }

    private PathfindingPath FindPathInNodes(int startX, int startY, int endX, int endY) // Find path from start node to the end node with A* algorithm
    {
        // Reset costs of all pathfinding nodes in grid
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                _grid.GetGridObject(x, y).ResetNodeCosts();
            }
        }

        PathfindingNode startNode = GetNode(startX, startY);
        PathfindingNode endNode = GetNode(endX, endY);

        // Used as end node to backtrack from, if no path is found
        PathfindingNode closestNode = null;

        List<PathfindingNode> closeList = new List<PathfindingNode>();
        List<PathfindingNode> openList = new List<PathfindingNode> { startNode };

        // Reset all nodes in grid and calculate their F values
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                PathfindingNode node = GetNode(x, y);
                node.GCost = int.MaxValue;
                node.CameFromNode = null;
                node.CalculateFCost();
            }
        }

        // Configure startNode
        startNode.GCost = 0;
        startNode.HCost = CalculateRelocationCost(startNode, endNode);
        startNode.CalculateFCost();

        // Limits number of iterations to prevent lag spikes
        int iterations = 0;

        // Loop while there are still nodes in open list
        while (openList.Count > 0 && iterations < MAXIMUM_ITERATIONS)
        {
            // Select current node from open list with the lowest F Cost
            PathfindingNode currentNode = GetLowestFCostNode(openList);

            // Path found
            if (currentNode == endNode)
            {
                return CreatePath(currentNode);
            }

            // Update closest node to the target
            if (closestNode == null)
            {
                closestNode = currentNode;
            }
            else if (currentNode.HCost < closestNode.HCost)
            {
                closestNode = currentNode;
            }

            // Add current node in close list and remove from open list
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            List<PathfindingNode> neighbourNodes = GetNeighboursList(currentNode);

            // Loop through neighbour nodes 
            foreach (PathfindingNode neighbour in neighbourNodes)
            {
                // If neighbour node is already in close list than skip to next
                if (closeList.Contains(neighbour))
                {
                    continue;
                }

                // If neighbour is not walkable than add it in close list and skip to next
                if (!neighbour.IsWalkable)
                {
                    closeList.Add(neighbour);
                    continue;
                }

                // Cost from current to neighbour node
                int predictedGCost = currentNode.GCost + CalculateRelocationCost(currentNode, neighbour) * neighbour.Priority;
                // We know that it is neighbour, so we can multiply relocation g cost by it's priority, because it's only one node away

                if (predictedGCost < neighbour.GCost)
                {
                    neighbour.GCost = predictedGCost;
                    neighbour.HCost = CalculateRelocationCost(neighbour, endNode);
                    neighbour.CalculateFCost();
                    neighbour.CameFromNode = currentNode;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }

            iterations++;
        }

        // No direct path found, so builds path to closest node to destination;
        if (closestNode != null)
        {
            return CreatePath(closestNode);
        }

        // Searched all nodes, no path found
        Debug.Log("Path not found");
        return null;
    }

    public PathfindingPath FindPathInWorld(Vector3 startPosition, Vector3 endPosition) // Find path with A* algorithm, but using world positions of start and end nodes as input (converts to local position)
    {
        int startX, startY, endX, endY;

        // Update Pathfinding surface if update didn't occur yet
        if (!_isSurfaceLatest)
        {
            UpdatePathfindingSurface();
        }

        // Get local positions from world positions
        _grid.GetGridPosition(startPosition, out startX, out startY);
        _grid.GetGridPosition(endPosition, out endX, out endY);
        
        // Use A* in nodes to calculate paht
        PathfindingPath path = FindPathInNodes(startX, startY, endX, endY);

        return path;
    }
}
