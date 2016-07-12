using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS
{
    internal static class Util
    {
        public static bool ImplementsInterface(Type classType, Type interfaceType)
        {
            return (interfaceType.IsAssignableFrom(classType));
        }
    }
}
