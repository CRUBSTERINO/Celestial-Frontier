using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogSystem.Editor.Windows
{
    using DialogSystem.Editor.Saves;
    using Nodes;


    public class DialogSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DialogGraphView dsGraphView)
        {
            graphView = dsGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeEntry(new GUIContent("Dialog Node", indentationIcon))
                {
                    userData = new DialogLineNode(),
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Condition Node", indentationIcon))
                {
                    userData = new ConditionNode(),
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Action Node", indentationIcon))
                {
                    userData = new ActionNode(),
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Group", indentationIcon))
                {
                    userData = new Group(),
                    level = 1
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch (SearchTreeEntry.userData)
            {
                case DialogLineNode:
                    {
                        DialogLineNodeEditorData dialogLineData = new DialogLineNodeEditorData()
                        {
                            Title = "DialogName",
                            Position = localMousePosition
                        };
                        DialogNode dialogNode = graphView.CreateNode(dialogLineData);

                        graphView.AddElement(dialogNode);

                        return true;
                    }

                case ConditionNode:
                    {
                        ConditionNodeEditorData conditionData = new ConditionNodeEditorData()
                        {
                            Title = "ConditionName",
                            Position = localMousePosition
                        };
                        DialogNode conditionNode = graphView.CreateNode(conditionData);

                        graphView.AddElement(conditionNode);

                        return true;
                    }

                case ActionNode:
                    {
                        ActionNodeEditorData actionData = new ActionNodeEditorData()
                        {
                            Title = "ActionName",
                            Position = localMousePosition
                        };
                        DialogNode actionNode = graphView.CreateNode(actionData);

                        graphView.AddElement(actionNode);

                        return true;
                    }

                case Group:
                    {
                        graphView.CreateGroup("DialogeGroup", localMousePosition);

                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
    } 
}