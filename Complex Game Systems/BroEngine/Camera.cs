namespace BroEngine
{
    using System.Linq;

    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    [DisallowMultipleComponent]
    public class Camera : Behaviour
    {
        public static Camera[] allCameras => FindObjectsOfType<Camera>().ToArray();
        public static int allCamerasCount => allCameras.Length;

        public static Camera main => GameObject.FindGameObjectWithTag("Main Camera")?.GetComponent<Camera>();

        private Matrix4 m_PerspectiveProjection;
        private Matrix4 m_OrthographicProjection;

        private float m_FieldOfView;
        private float m_Near;
        private float m_Far;

        public bool isOrthographic { get; set; }

        public Rect rect { get; set; } = new Rect(0f, 0f, 1f, 1f);

        public float aspectRatio { get; protected set; }

        public float fieldOfView
        {
            get => m_FieldOfView;
            set => SetPerspective(value, aspectRatio, near, far);
        }
        public float near
        {
            get => m_Near;
            set => SetPerspective(m_FieldOfView, aspectRatio, value, m_Far);
        }
        public float far
        {
            get => m_Far;
            set => SetPerspective(m_FieldOfView, aspectRatio, m_Near, value);
        }

        public Matrix4 view => transform.worldSpaceMatrix.Inverted();

        public Matrix4 projectionMatrix =>
            isOrthographic ? m_OrthographicProjection : m_PerspectiveProjection;

        public Matrix4 viewProjection => view * projectionMatrix;

        public Camera()
        {
            //SetLookAt(new Vector3(0f, 5f, 10f), Vector3.Zero, new Vector3(0f, 1f, 0f));
            SetPerspective(MathHelper.PiOver4, 16f / 9f, 0.1f, 75f);
            SetOrthographic(16, 9, m_Near, m_Far);
        }

        public void SetPerspective(float fieldOfView, float aspectRatio, float near, float far)
        {
            this.aspectRatio = aspectRatio;

            m_FieldOfView = fieldOfView;
            m_Near = near;
            m_Far = far;

            m_PerspectiveProjection =
                Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, near, far);
        }
        public void SetOrthographic(float width, float height, float near, float far)
        {
            m_Far = far;
            m_Near = near;

            m_OrthographicProjection = Matrix4.CreateOrthographic(width, height, near, far);
        }

        public void SetLookAt(Vector3 from, Vector3 to, Vector3 up)
        {
            var zAxis = (from - to).Normalized();
            var xAxis = Vector3.Cross(up, zAxis).Normalized();
            var yAxis = Vector3.Cross(zAxis, xAxis);

            var orientation =
                new Matrix4(
                    xAxis.X, yAxis.X, zAxis.X, 0,
                    xAxis.Y, yAxis.Y, zAxis.Y, 0,
                    xAxis.Z, yAxis.Z, zAxis.Z, 0,
                    0, 0, 0, 1);

            var translation =
                new Matrix4(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    -from.X, -from.Y, -from.Z, 1);

            transform.localSpaceMatrix = (translation * orientation).Inverted();
        }

        internal void Render()
        {
            if (!enabled)
                return;

            GL.Viewport(
                (int)(MyGameWindow.main.Width * rect.position.X),
                (int)(MyGameWindow.main.Height * rect.position.Y),
                (int)(MyGameWindow.main.Width * rect.size.X),
                (int)(MyGameWindow.main.Height * rect.size.Y));

            foreach (var renderer in FindObjectsOfType<Renderer>())
                renderer.Render(this);
        }
    }
}
