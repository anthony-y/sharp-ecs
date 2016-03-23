using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS;

namespace SharpECS.Tests
{
    public class BetterComponent
        : IComponent
    {
        public Entity Owner { get; set; }
    }
}
