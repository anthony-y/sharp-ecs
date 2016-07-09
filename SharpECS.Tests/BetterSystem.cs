using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS;

namespace SharpECS.Tests
{
    public class BetterSystem
        : EntitySystem
    {
        public BetterSystem(EntityPool entityPool)
            : base(entityPool, typeof(IComponent))
        {
            Console.WriteLine("Hi from test system.");
        }

        public void Update()
        {
            foreach (var i in Compatible)
            {
                Console.WriteLine("Compatible Entity: " + i.Id);
            }
        }

        public void Draw()
        {

        }
    }
}
