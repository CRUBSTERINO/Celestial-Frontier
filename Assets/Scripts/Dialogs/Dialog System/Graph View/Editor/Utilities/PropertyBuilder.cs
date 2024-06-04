using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace DialogSystem.Editor.Utilities
{
    // Used to draw property of non UnityEngine.Object type when using UIElements
    // Works by using ScriptableObject wrapper with [SerializeReference] object property
    public static class PropertyBuilder
    {
        public static void DrawProperty(object propertyInstance, string label, VisualElement root)
        {
            GenericProperty instance = ScriptableObject.CreateInstance<GenericProperty>();
            instance.property = propertyInstance;
            SerializedObject serializedObject = new SerializedObject(instance);

            PropertyField property = new PropertyField();
            property.label = label;
            property.bindingPath = nameof(instance.property);
            property.Bind(serializedObject);

            root.Add(property);
        }
    }


    public class GenericProperty : ScriptableObject
    {
        [SerializeReference] public object property;
    } 
}