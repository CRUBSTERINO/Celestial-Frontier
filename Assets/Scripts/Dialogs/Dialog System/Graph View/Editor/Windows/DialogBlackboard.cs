using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.Windows
{
    using BlackboardElements;
    using System;
    using UnityEditor;
    using Utilities;

    public class DialogBlackboard : Blackboard
    {
        // Should depend on abstracts, but it is planned to only use people as exposed properties, so for now it is made this way
        public List<PersonBlackboardField> BlackboardFields { get; set; }

        public DialogBlackboard(GraphView graphView) : base(graphView)
        {
            title = "Dialog Blackboard";
            subTitle = string.Empty;

            style.left = 0;
            style.top = 35;

            BlackboardFields = new List<PersonBlackboardField>();

            addItemRequested += (Blackboard blackboard) =>
            {
                AddExposedProperty(string.Empty, string.Empty);
            };
        }

        // Creates exposed property, but now it is only person id selection (I know that SOLID is broken here, but it is not planned to use exposed properties for something else and I have no time)
        private VisualElement CreateExposedProperty(string id, string personId, string fieldText = "Field Text", string typeText = "string")
        {
            VisualElement container = new VisualElement();

            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            PersonBlackboardField field = new PersonBlackboardField(id, personId, fieldText, typeText);

            BlackboardFields.Add(field);

            container.Add(field);

            TextField textField = ElementUtility.CreateTextField(personId, "Person ID", callback =>
            {
                field.PersonId = callback.newValue;
            });

            BlackboardRow row = new BlackboardRow(field, textField);
            container.Add(row);

            return container;
        }

        public void AddExposedProperty(string id, string personId, string fieldText = "Field Text", string typeText = "string")
        {
            Add(CreateExposedProperty(id, personId, fieldText, typeText));
        }
    } 
}
