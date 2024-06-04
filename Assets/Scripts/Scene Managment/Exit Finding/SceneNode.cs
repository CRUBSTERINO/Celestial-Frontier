using System.Collections.Generic;
// Used in exit-finding as node representing scene
public class SceneNode
{
    public SceneNode(SceneScriptableObject scene, SceneNode previousNode)
    {
        Scene = scene;
        PreviousNode = previousNode;
    }

    public SceneScriptableObject Scene { get; set; }
    public SceneNode PreviousNode { get; set; }

    // Returns "path" from all previous scenes including this
    public List<SceneScriptableObject> GetPreviousScenesFromThisNode()
    {
        List<SceneScriptableObject> previousNodes = new List<SceneScriptableObject>();

        SceneNode currentNode = this;

        while (currentNode != null)
        {
            previousNodes.Add(currentNode.Scene);

            currentNode = currentNode.PreviousNode;
        }

        previousNodes.Reverse();

        return previousNodes;
    }
}