using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS;

namespace SharpECS.Tests
{
    public class BetterSystem
        : EntitySystem<IComponent>
    {
        public BetterSystem(EntityPool entityPool)
            : base(entityPool)
        {
            Console.WriteLine("Hi from test system.");
        }

        public void Update()
        {
            foreach (var i in Compatible)
            {
                Console.WriteLine("Compatible Entity: " + i.Tag);
            }
        }

        public void Draw()
        {

        }
    }
}
