namespace ComplexGameSystems
{
    using Geometry;

    using OpenTK;

    public abstract class Camera
    {
        public static Camera main { get; protected set; }

        protected Matrix4 m_ProjectionMatrix;
        protected Matrix4 m_PositionMatrix;

        public Matrix4 view => Matrix4.Invert(m_PositionMatrix);
        public Matrix4 projection => m_ProjectionMatrix;
        public Matrix4 modelViewProjection => m_ProjectionMatrix * view;

        protected Camera()
        {
            if (main == null)
                main = this;
        }

        public abstract void Update();

        public void SetPerspective(float fieldOfView, float aspectRatio, float near, float far)
        {
            m_ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, near, far);
        }
        public void SetLookAt(Vector3 from, Vector3 to, Vector3 up)
        {
            m_PositionMatrix = Matrix4.LookAt(from, to, up);
        }
    }
}
