using System;
using UnityEditor.Experimental.GraphView;

namespace DialogSystem.Editor.Ports
{
    // An abstract class that can be implemented by classes that want to define the ports they can connect to
    // GraphView library doesn't support instantiating ports of types other than "Ports", so custom port type is only stored in "portType" field
    public abstract class ConfigurablePort : Port
    {
        public ConfigurablePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {

        }
    } 
}
