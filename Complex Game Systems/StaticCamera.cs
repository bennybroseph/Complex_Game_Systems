using System;

using OpenTK;

public class StaticCamera : Camera
{
    protected override void OnResize(object sender, EventArgs eventArgs)
    {
        var window = sender as OpenTK.GameWindow;
        if (window == null)
            return;

        aspectRatio = window.Width / (float)window.Height;

        //m_ProjectionMatrix =
        //    Matrix4.CreatePerspectiveFieldOfView(
        //        fieldOfView, aspectRatio, near, far);
    }
    public override void Update() { }
}