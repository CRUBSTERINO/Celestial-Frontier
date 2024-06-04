using System.Collections.Generic;

namespace DialogSystem.Editor.Errors
{
    using Nodes;

    public class GroupErrorData
    {
        public ErrorData ErrorData { get; set; }
        public List<DialogGroup> Groups { get; set; }

        public GroupErrorData()
        {
            ErrorData = new ErrorData();
            Groups = new List<DialogGroup>();
        }
    } 
}