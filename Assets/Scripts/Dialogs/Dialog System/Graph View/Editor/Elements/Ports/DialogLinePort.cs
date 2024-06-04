using System;
using UnityEditor.Experimental.GraphView;

namespace DialogSystem.Editor.Ports
{
    public class DialogLinePort : ConfigurablePort
    {
        public DialogLinePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }
    } 
}
