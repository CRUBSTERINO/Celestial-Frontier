using DialogSystem.Editor.Nodes;
using System.Collections.Generic;

namespace DialogSystem.Editor.Errors
{
    public class NodeErrorData
    {
        public ErrorData ErrorData { get; set; }
        public List<DialogNode> Nodes { get; set; }

        public NodeErrorData()
        {
            ErrorData = new ErrorData();
            Nodes = new List<DialogNode>();
        }
    } 
}