namespace BroEngine
{
    public abstract class Behaviour : Component
    {
        public bool enabled { get; } = true;

        protected Behaviour(string name = "New Behaviour") : base(name) { }
    }
}

