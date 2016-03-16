using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter;

namespace ECSIsBetter.Tests
{
    public class BetterSystem
        : EntitySystem<BetterComponent>
    {
        public BetterSystem(EntityPool entityPool)
            : base(entityPool)
        {
            
        }

        public void OnCompatibleEntityChanged(Entity sender, IComponent component)
        {
            Console.WriteLine(sender.Tag + " was changed or something :P");
        }

        public override void Draw()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}
