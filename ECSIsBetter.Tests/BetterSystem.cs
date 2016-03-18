using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter;

namespace ECSIsBetter.Tests
{
    public class BetterSystem
        : EntitySystem
    {
        public BetterSystem(EntityPool entityPool)
            : base(entityPool)
        {

        }

        public override void Initialize()
        {
            Console.WriteLine("Hi from test system.");
        }

        public override void Update()
        {

        }

        public override void Draw()
        {

        }
    }
}
