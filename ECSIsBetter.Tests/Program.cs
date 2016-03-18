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
            // SystemsTest();
            // ComponentOwnerTest();
            // GameLoopTest();

            ComponentMoveTest();

            // Done.
            Console.ReadKey();
        }

        static void ComponentMoveTest()
        {
            var pool = EntityPool.New("My Amazing Pool");

            var entity = pool.CreateEntity("My Amazing Entity");
            var entity2 = pool.CreateEntity("My Other Amazing Entity");

            var component = new BetterComponent();
            entity.AddComponent(component);

            Console.WriteLine("Entity1 component count: " + entity.GetAllComponents().Count);

            entity.MoveComponent(component, entity2);

            Console.WriteLine("Entity1 component count: " + entity.GetAllComponents().Count);
            Console.WriteLine("Entity2 component count: " + entity2.GetAllComponents().Count);

            // It works :3
        }

        static void ComponentOwnerTest()
        {
            var pool = EntityPool.New("My Pool");

            var entity = pool.CreateEntity("My Entity");

            entity += new BetterComponent();

            // Yay
            Console.WriteLine("Component owner should be \"My Entity\"... it's actually: " + entity.GetComponent<BetterComponent>().Owner.Tag);

        }

        static void GameLoopTest()
        {
            //var pool1 = EntityPool.New("My Pool");
            //var system = new BetterSystem(pool1);

            //var entity = new Entity("My Entity", null);

            //pool1 += entity;

            //// Simulate a game loop
            //while (true)
            //{
            //    system.Update();
                
            //    System.Threading.Thread.Sleep(100);
            //}
        }

        static void SetHandlers(params EntityPool[] pools)
        {
            foreach (var pool in pools)
            {
                pool.EntityAdded += (EntityPool entPool, Entity e) =>
                {
                    Console.WriteLine("Entity " + e.Tag + " was added to pool " + entPool.Name + "\n");
                };

                pool.EntityRemoved += (EntityPool entPool, Entity e) =>
                {
                    Console.WriteLine("Entity " + e.Tag + " was removed from pool " + entPool.Name + "\n");
                };
            }
        }

        static void SystemsTest()
        {
            //var pool1 = new EntityPool("Pool1");
            //var pool2 = new EntityPool("Pool2");

            //SetHandlers(pool1, pool2);

            //var entity1 = pool1.CreateEntity("Entity1");
            //var entity2 = pool2.CreateEntity("Entity2");

            //var system1 = new BetterSystem(pool1);
            //var system2 = new BetterSystem(pool2);

            //Console.WriteLine("Count before: " + system1.CompatibleEntities.Count);

            //var entity3 = pool1.CreateEntity("Entity3");

            //Console.WriteLine("Count after: " + system1.CompatibleEntities.Count);

        }

        /// <summary>
        /// These tests are NOT exaples of how to structure your game
        /// If your code looks like this...
        /// get yo ass back to the drawing board :P
        /// </summary>
        /// <param name="pool"></param>
        static void CachingTests(EntityPool pool)
        {
            SetHandlers(pool);

            var ent1 = pool.CreateEntity("Player");
            pool.DestroyEntity(ent1);

            // DO NOT make your entities like this.
            //var wow = new Entity("magic");

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

            Console.WriteLine("Cached Entity count: " + pool.CachedEntities.Count);

            Console.ReadKey();

            pool.CreateEntity("Player7");
            pool.CreateEntity("Player8");
            pool.CreateEntity("Player9");

            Console.WriteLine();

            pool.CreateEntity("Player10");

            Console.WriteLine("Cached Entity count: " + pool.CachedEntities.Count);

            pool.WipeEntities();
        }

    }
}
