using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS;

namespace SharpECS.Tests
{
    public class EvenBetterComponent
        : IComponent
    {
        public Entity Owner { get; set; }
    }
}
