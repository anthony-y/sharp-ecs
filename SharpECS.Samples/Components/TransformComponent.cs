using Microsoft.Xna.Framework;

namespace SharpECS.Samples.Components
{
    public class TransformComponent 
        : IComponent
    {
        public Entity Owner { get; set; }
        
        private Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public void SetX(float newX) => _position.X = newX;
        public void SetY(float newY) => _position.Y = newY;
    }
}