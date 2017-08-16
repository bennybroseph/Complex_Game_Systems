namespace BroEngine
{
    using System.Collections.Generic;

    using OpenTK;

    public class Camera : Behaviour
    {
        private static List<Camera> s_Cameras = new List<Camera>();

        public static List<Camera> allCameras => s_Cameras;
        public static int allCamerasCount => s_Cameras.Count;

        public static Camera main => GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();

        public float fieldOfView { get; protected set; }
        public float aspectRatio { get; protected set; }
        public float near { get; protected set; }
        public float far { get; protected set; }

        public Matrix4 projectionMatrix { get; protected set; }

        public Camera() : base("New Camera")
        {
            //SetLookAt(new Vector3(0f, 5f, 10f), Vector3.Zero, new Vector3(0f, 1f, 0f));
            SetPerspective(MathHelper.PiOver4, 6f / 10f, 0.1f, 75f);
        }

        public void SetPerspective(float fieldOfView, float aspectRatio, float near, float far)
        {
            this.fieldOfView = fieldOfView;
            this.aspectRatio = aspectRatio;
            this.near = near;
            this.far = far;

            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, near, far);
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
    }
}
