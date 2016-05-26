using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SharpECS.Samples.Components
{
    internal class ControllerComponent 
        : IComponent
    {
        public Entity Owner { get; set; }

        public float MoveSpeed { get; set; } = 700;
    }
}
