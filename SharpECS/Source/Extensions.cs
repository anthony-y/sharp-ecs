using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpECS 
{
    public static class Extensions
    {
        public static bool IsComponent(this Type classType) => typeof(IComponent).IsAssignableFrom(classType);
    }
}