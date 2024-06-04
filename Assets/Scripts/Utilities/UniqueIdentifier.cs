using NaughtyAttributes;
using System;
using UnityEngine;

public class UniqueIdentifier : MonoBehaviour
{
    [SerializeField, ReadOnly] private string _id;

    public string ID => _id;

    private void SetID(string id)
    {
        _id = id;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    private void AssignNewID()
    {
        string id = Guid.NewGuid().ToString();
        SetID(id);
    }

    public static UniqueIdentifier FindObjectWithID(string id)
    {
        UniqueIdentifier[] ids = FindObjectsOfType<UniqueIdentifier>();

        foreach (UniqueIdentifier comaredId in ids)
        {
            if (comaredId._id == id)
            {
                return comaredId;
            }
        }

        return null;
    }

#if UNITY_EDITOR
    [Button("Generate ID")]
    public void GenerateNewID()
    {
        AssignNewID();
    }

    [Button("Copy ID")]
    public void CopyID()
    {
        UnityEditor.EditorGUIUtility.systemCopyBuffer = _id;
    }

    [Button("Paste ID")]
    public void PasteID()
    {
        SetID(UnityEditor.EditorGUIUtility.systemCopyBuffer);
    }
#endif
}
