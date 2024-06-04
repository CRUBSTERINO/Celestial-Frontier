using System;
using System.Linq;

namespace DialogSystem.Editor.Utilities
{
    public static class ReflectionsUtility
    {
        public static Type[] GetImplementations<T>()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

            var interfaceType = typeof(T);
            return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
        }
    } 
}
