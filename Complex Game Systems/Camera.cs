using System;

using OpenTK;

public abstract class Camera
{
    public static Camera main { get; protected set; }

    public float fieldOfView { get; protected set; }
    public float aspectRatio { get; protected set; }
    public float near { get; protected set; }
    public float far { get; protected set; }

    protected Matrix4 m_ProjectionMatrix;
    protected Matrix4 m_PositionMatrix;

    public Matrix4 view => m_PositionMatrix.Inverted();
    public Matrix4 projection => m_ProjectionMatrix;
    public Matrix4 viewProjection => (view * projection);

    protected Camera()
    {
        if (main == null)
            main = this;

        GameWindow.main.Resize += OnResize;
    }

    protected abstract void OnResize(object sender, EventArgs eventArgs);
    public abstract void Update();

    public void SetPerspective(float fieldOfView, float aspectRatio, float near, float far)
    {
        this.fieldOfView = fieldOfView;
        this.aspectRatio = aspectRatio;
        this.near = near;
        this.far = far;

        m_ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, near, far);
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

        m_PositionMatrix = (translation * orientation).Inverted();
    }
}