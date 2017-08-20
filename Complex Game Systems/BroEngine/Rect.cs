namespace BroEngine
{
    using OpenTK;

    public struct Rect
    {
        public static Rect zero => new Rect();

        public Vector2 position { get; set; }
        public Vector2 size { get; set; }

        public Vector2 center { get; set; }

        public float width => size.X;
        public float height => size.Y;

        public Vector2 min => center;

        public Rect(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;

            center = position + size / 2f;
        }
        public Rect(float x, float y, float width, float height) :
            this(new Vector2(x, y), new Vector2(width, height))
        {

        }
    }
}
