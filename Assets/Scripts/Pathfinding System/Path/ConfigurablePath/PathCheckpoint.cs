using UnityEditor;
using UnityEngine;

public class PathCheckpoint : MonoBehaviour
{
    public const int InvalidIdIndex = -1;

    [SerializeField] private float _inspectingTime;

    public Vector3 Position { get { return transform.position; } }
    public float InspectingTime { get { return _inspectingTime; } }
    public int Id { get; set; } = -1;

#if UNITY_EDITOR
    public static PathCheckpoint InstantiateCheckPoint(Vector3 position)
    {
        GameObject instance = new GameObject();
        instance.transform.position = position;
        instance.transform.rotation = Quaternion.identity;

        instance.AddComponent<PathCheckpoint>();

        EditorUtility.SetDirty(instance);

        return instance.GetComponent<PathCheckpoint>();
    }
#endif
}
