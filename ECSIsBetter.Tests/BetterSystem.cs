﻿using System;
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

        protected override List<Entity> CompatibleEntities { get; set; }
        
        public override void Draw()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}
