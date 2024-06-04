using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Manager Config", menuName = "Scriptables/Game Managment/Game Manager Config")]
public class GameManagerScriptableObject : ScriptableObject
{
    [SerializeField] private GameObject _personalityTraitsUIPrefab;
    [SerializeField] private bool _offerPlayerPersonalityChoice;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _cameraPrefab;
    [SerializeField] private SceneEntranceScriptableObject _defeatScreenSceneEntrance;

    public GameObject PersonalityTraitsUIPrefab => _personalityTraitsUIPrefab;
    public bool OfferPlayerPersonalityChoice => _offerPlayerPersonalityChoice;
    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject CameraPrefab => _cameraPrefab;
    public SceneEntranceScriptableObject DefeatScreenSceneEntrance => _defeatScreenSceneEntrance;
}