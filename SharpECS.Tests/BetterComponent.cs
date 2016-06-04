namespace SharpECS.Tests
{
    public class BetterComponent
        : IComponent
    {
        public Entity Owner { get; set; }
    }
}