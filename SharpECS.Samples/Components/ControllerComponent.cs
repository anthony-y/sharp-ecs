namespace SharpECS.Samples.Components
{
    public class ControllerComponent 
        : IComponent
    {
        public Entity Owner { get; set; }

        public float MoveSpeed { get; set; } = 700;
    }
}