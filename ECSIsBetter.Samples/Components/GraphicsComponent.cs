using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECSIsBetter;

using Microsoft.Xna.Framework.Graphics;

namespace ECSIsBetter.Samples.Components
{
    public class GraphicsComponent : IComponent
    {
        public Entity Owner { get; set; }
        public Texture2D Texture { get; set; }
    }
}
