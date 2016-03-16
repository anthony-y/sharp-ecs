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
            Entity entity = new Entity("Player");

            var component = new BetterComponent();

            entity.ComponentAdded += (Entity changed, IComponent added) => {
                Console.WriteLine("Added component " + added.GetType().Name + " to " + changed.Tag);
            };

            entity.ComponentRemoved += (Entity changed, IComponent added) => {
                Console.WriteLine("Removed component " + added.GetType().Name + " from " + changed.Tag);
            };

            entity.AddComponent(component);

            Console.ReadKey();

            entity.RemoveComponent(component);

            Console.ReadKey();
        }
    }
}
