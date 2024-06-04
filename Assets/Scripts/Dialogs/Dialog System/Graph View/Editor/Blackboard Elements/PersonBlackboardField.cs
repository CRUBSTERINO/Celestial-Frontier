using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogSystem.Editor.BlackboardElements
{
    // Blackboard field that represents a person
    public class PersonBlackboardField : BlackboardField
    {
        [field: SerializeField] public string PersonId { get; set; } // ID of person
        [field: SerializeField] public string ID { get; set; } // ID of field

        public PersonBlackboardField(string id, string personId, string fieldText, string typeText)
        {
            ID = id;
            PersonId = personId;
            text = fieldText;
            this.typeText = typeText;
        }
    } 
}
