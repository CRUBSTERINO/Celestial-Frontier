public class PathfindingNode
{
    private int _x;
    private int _y;

    private int _gCost; // Distance from starting node
    private int _hCost; // (heuristic) distance to end node (ignoring obstacles)
    private int _fCost; // Summ of G and H costs

    private int _priority; // Priority is a cost multiplier. 1 - is the first priority
    private bool _isWalkable;
    private PathfindingNode _cameFromNode;

    public int X => _x;
    public int Y => _y;
    public int GCost { get => _gCost; set => _gCost = value; }
    public int HCost { get => _hCost; set => _hCost = value; }
    public int FCost => _fCost;
    public int Priority { get => _priority; set => _priority = value; }
    public bool IsWalkable { get => _isWalkable; set => _isWalkable = value; }
    public PathfindingNode CameFromNode { get => _cameFromNode; set => _cameFromNode = value; }

    public PathfindingNode(int x, int y, bool isWalkable, int priority)
    {
        _x = x;
        _y = y;
        _isWalkable = isWalkable;
        _priority = priority;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost + _hCost;
    }

    public void ResetNodeCosts()
    {
        _gCost = 0;
        _hCost = 0;
        _fCost = 0;
    }
}
