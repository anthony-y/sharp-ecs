using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpECS;

using Microsoft.Xna.Framework.Graphics;

namespace SharpECS.Samples.Components
{
    internal class GraphicsComponent 
        : IComponent
    {
        public Entity Owner { get; set; }
        public Texture2D Texture { get; set; }
    }
}
