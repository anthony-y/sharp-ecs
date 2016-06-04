using Microsoft.Xna.Framework.Graphics;

namespace SharpECS.Samples.Components
{
    public class GraphicsComponent 
        : IComponent
    {
        public Entity Owner { get; set; }
        public Texture2D Texture { get; set; }
    }
}