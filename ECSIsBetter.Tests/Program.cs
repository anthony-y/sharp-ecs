using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter;

namespace ECSIsBetter.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var pool = new EntityPool();
            var entity = pool.CreateEntity("Player");
            var system = new BetterSystem(pool);
            var component = new BetterComponent();

            entity.AddComponent(component);

            Console.ReadKey();

            entity.RemoveComponent(component);

            Console.ReadKey();

            pool.RemoveEntity(entity);
            var newEnt = pool.CreateEntity("wow");

            Console.WriteLine("Entity tag is still " + newEnt.Tag);

            Console.ReadKey();
        }
    }
}
