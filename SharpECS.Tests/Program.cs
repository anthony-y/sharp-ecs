using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS;

using System.Diagnostics;

namespace SharpECS.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var pool = EntityPool.New("Pool1");

            var timer = Stopwatch.StartNew();

            var newEntity = pool.CreateEntity("newEntity");

            timer.Stop();

            Console.WriteLine("It took " + timer.Elapsed.TotalMilliseconds + "ms to create a new instance of Entity.");

            timer.Restart();

            pool.DestroyEntity(ref newEntity);

            timer.Stop();

            Console.WriteLine("It took " + timer.Elapsed.TotalMilliseconds + "ms to delete an Entity and move it into the cache!");

            timer.Restart();

            var cacheEntity = pool.CreateEntity("cacheEntity");

            timer.Stop();

            Console.WriteLine("It took " + timer.Elapsed.TotalMilliseconds + "ms to pull an Entity in from the cache!");

            timer.Stop();

            Console.ReadKey();
        }
    }
}
