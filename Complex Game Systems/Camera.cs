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

    public Matrix4 view => m_PositionMatrix; // Thought I needed an inverse, but I'm not even sure anymore
    public Matrix4 projection => m_ProjectionMatrix;
    public Matrix4 viewProjection => m_PositionMatrix * m_ProjectionMatrix;

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
        m_PositionMatrix = Matrix4.LookAt(from, to, up);
    }
}