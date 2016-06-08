namespace SharpECS.Samples.Components
{
    public class ControllerComponent 
        : IComponent
    {
        public string Id { get; set; }
        public float MoveSpeed { get; set; } = 700;
    }
}