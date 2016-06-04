using SharpECS;
using System;

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
                Console.WriteLine($"Compatible Entity: {i.Tag}");
            }
        }

        public void Draw()
        {

        }
    }
}