namespace SharpECS.Tests
{
    public class EvenBetterComponent
        : IComponent
    {
        public Entity Owner { get; set; }
    }
}