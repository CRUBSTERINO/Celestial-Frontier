using DialogSystem.Editor.BlackboardElements;
using System;
using UnityEngine;

namespace DialogSystem.Editor.Saves
{
    // Saving data of person exposed property from blackboard
    [Serializable]
    public class PersonBlackboardFieldEditorData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string PersonID { get; set; } // Person ID shouldn't be there, but now there is only Person id that is goind to be used for saving (just too lazy)
        [field: SerializeField] public string FieldText { get; set; }
        [field: SerializeField] public string TypeText { get; set; }

        public PersonBlackboardField CreateBlackboardFieldInstance()
        {
            PersonBlackboardField personField = new PersonBlackboardField(ID, PersonID, FieldText, TypeText);

            return personField;
        }
    } 
}
