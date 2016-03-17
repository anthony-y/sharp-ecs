using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter;

using System.Diagnostics;

namespace ECSIsBetter.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var pool = new EntityPool("GenericPool");
            var pool2 = new EntityPool("NotGenericPool");

            // Caching tests.
            //CachingTests(pool);

            var entity = pool.CreateEntity("Player");

            Console.WriteLine("Current entity pool: " + entity.OwnerPool.Name);

            Console.ReadKey();

            entity.MoveTo(pool2);

            Console.WriteLine("Current entity pool: " + entity.OwnerPool.Name);

            // Done.
            Console.ReadKey();
        }

        /// <summary>
        /// These tests are NOT exaples of how to structure your game
        /// If your code looks like this...
        /// get yo ass back to the drawing board :P
        /// </summary>
        /// <param name="pool"></param>
        static void CachingTests(EntityPool pool)
        {
            var ent1 = pool.CreateEntity("Player");
            pool.DestroyEntity(ent1);

            // DO NOT make your entities like this.
            var wow = new Entity("magic");

            //pool.DestroyEntity(wow); <-- causes an EntityNotFoundException

            Console.WriteLine();

            // Use this instead.
            var ent2 = pool.CreateEntity("Player2");
            pool.DestroyEntity(ent2);

            Console.WriteLine();

            Console.ReadKey();

            var ent3 = pool.CreateEntity("Player3");
            pool.DestroyEntity(ent3);

            Console.WriteLine();

            var ent4 = pool.CreateEntity("Player4");
            var ent5 = pool.CreateEntity("Player5");
            var ent6 = pool.CreateEntity("Player6");

            Console.WriteLine();

            pool.DestroyEntity(ent4);
            pool.DestroyEntity(ent5);
            pool.DestroyEntity(ent6);

            Console.WriteLine();

            pool.CreateEntity("Player7");
            pool.CreateEntity("Player8");
            pool.CreateEntity("Player9");
            pool.CreateEntity("Player10");

            pool.WipeEntities();
        }

    }
}
