using Microsoft.Xna.Framework.Graphics;

namespace SharpECS.Samples.Components
{
    public class GraphicsComponent 
        : IComponent
    {
        public string Id { get; set; }
        public Texture2D Texture { get; set; }
    }
}