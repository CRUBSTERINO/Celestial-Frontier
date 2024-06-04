using UnityEngine;

[CreateAssetMenu(fileName = "Minigames Service Config", menuName = "Scriptables/Minigames/Minigames Service Config")]
public class MinigamesServiceScriptableObject : ScriptableObject
{
    [field: SerializeField] public GameObject DefaultMinigamePrefab { get; set; }
    [field: SerializeField] public GameObject DefaultMinigameUIPrefab { get; set; }
}
