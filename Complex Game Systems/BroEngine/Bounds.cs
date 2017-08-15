namespace BroEngine
{
    using System.Diagnostics;
    using OpenTK;

    public class Bounds
    {
        public Vector3 center { get; set; }
        public Vector3 size { get; set; }

        public Vector3 min => center - extents;
        public Vector3 max => center + extents;

        public Vector3 extents => size / 2f;

        public Bounds(Vector3 newCenter, Vector3 newSize)
        {
            center = newCenter;
            size = newSize;
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            if (Contains(point))
                return point;

            return new Vector3();
        }

        public bool Contains(Vector3 point)
        {
            return point.X >= min.X && point.X <= max.X &&
                   point.Y >= min.Y && point.Y <= max.Y &&
                   point.Z >= min.Z && point.Z <= max.Z;
        }

        public void Encapsulate(Vector3 point)
        {


            Debug.Assert(Contains(point));
        }
    }
}
