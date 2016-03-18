using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECSIsBetter;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ECSIsBetter.Samples.Components
{
    public class ControllerComponent : IComponent
    {
        public Entity Owner { get; set; }

        public float MoveSpeed { get; set; } = 700;
    }
}
