using System;
using UnityEngine;

[Serializable]
public class SwitchSceneInteraction : Interaction
{
    [SerializeField] private SceneLoader _sceneLoader;

/*    public override void AcceptInteractionVisitor(IInteractionVisitor visitor)
    {
        visitor.Visit(this);
    }*/

    public override void PerformInteraction(Interactor interactor)
    {
        _sceneLoader.LoadScene();
    }

    public SceneScriptableObject GetTargetScene()
    {
        return _sceneLoader.TargetScene;
    }
}