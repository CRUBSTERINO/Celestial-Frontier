using UnityEngine;

[CreateAssetMenu(fileName = "World Time Service Config", menuName = "Scriptables/Time System/World Time Service Config")]
public class WorldTimeServiceScriptableObject : ScriptableObject
{
    [SerializeField] public float TimeRate;
    // Starting Time. x = Hours, y = Minutes
    [SerializeField] public Vector2Int InitialTime;
}