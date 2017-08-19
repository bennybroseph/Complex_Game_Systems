namespace BroEngineEditor
{
    using System;
    using System.Windows.Forms;

    using BroEngine;

    using Geometry.Shapes;

    using ImGuiNET;

    using OpenTK;
    using OpenTK.Graphics;

    using Vector2 = System.Numerics.Vector2;

    internal class ResizableWindow
    {
        protected bool m_IsHoveringNS;
        protected bool m_IsHoveringWE;

        protected bool m_IsResizingNS;
        protected bool m_IsResizingWE;

        protected string m_WindowTitle = "";
        protected WindowFlags m_WindowFlags = WindowFlags.Default | WindowFlags.NoMove;

        //protected abstract void DrawWindowElements();

        public virtual void DrawGui()
        {
            if (ImGui.BeginWindow(m_WindowTitle, m_WindowFlags))
            {
                TryResizeNSWE();

                //DrawWindowElements();
            }
            ImGui.EndWindow();
        }

        protected unsafe void TryResizeNSWE()
        {
            if (m_IsHoveringNS && m_IsHoveringWE)
                Cursor.Current = Cursors.SizeNESW;

            if (m_IsResizingNS && m_IsResizingWE)
            {
                Cursor.Current = Cursors.SizeNESW;

                var io = *ImGuiNative.igGetIO();

                ImGuiNative.igSetWindowPos(
                    ImGui.GetWindowPosition() + new Vector2(io.MouseDelta.X, 0f), SetCondition.Always);
                ImGui.SetWindowSize(ImGui.GetWindowSize() + new Vector2(-io.MouseDelta.X, io.MouseDelta.Y));

                if (!ImGui.IsMouseDragging(0, 0))
                    m_IsResizingWE = false;
            }
            else
            {
                TryResizeNS();
                TryResizeWE();
            }
        }

        protected unsafe void TryResizeNS()
        {
            if (m_IsResizingNS)
            {
                Cursor.Current = Cursors.SizeNS;

                var io = *ImGuiNative.igGetIO();

                var mouseDelta = new Vector2(0f, io.MouseDelta.Y);
                ImGui.SetWindowSize(ImGui.GetWindowSize() + mouseDelta);

                if (!ImGui.IsMouseDragging(0, 0))
                    m_IsResizingNS = false;
            }

            var min = ImGui.GetWindowPosition() + new Vector2(0f, ImGui.GetWindowHeight() - 5f);
            var max = min + new Vector2(ImGui.GetWindowWidth(), 10f);
            if (ImGui.IsMouseHoveringRect(min, max, false))
            {
                if (!m_IsHoveringWE)
                    Cursor.Current = Cursors.SizeNS;

                m_IsHoveringNS = true;
                if (ImGui.IsMouseDragging(0, 0))
                    m_IsResizingNS = true;
            }
            else if (!m_IsResizingNS)
                m_IsHoveringNS = false;
        }
        protected void TryResizeWE()
        {
            var center =
                new Vector3(
                    ImGui.GetWindowPosition().X,
                    ImGui.GetWindowPosition().Y + ImGui.GetWindowHeight() / 2f, 0f);
            var size = new Vector3(10, ImGui.GetWindowHeight(), 0f);
            TryResizeDirection(
                new Vector2(1f, 0f),
                new Vector2(-1f, 0f),
                new Bounds(center, size),
                ref m_IsHoveringWE,
                ref m_IsResizingWE,
                Cursors.SizeWE);
        }

        protected static unsafe void TryResizeDirection(
            Vector2 posDirection,
            Vector2 sizeDirection,
            Bounds bounds,
            ref bool isHovering,
            ref bool isDragging,
            Cursor cursor)
        {
            if (isDragging)
            {
                Cursor.Current = cursor;

                var io = *ImGuiNative.igGetIO();

                var posDelta = io.MouseDelta * posDirection;
                var sizeDelta = io.MouseDelta * sizeDirection;

                ImGuiNative.igSetWindowPos(ImGui.GetWindowPosition() + posDelta, SetCondition.Always);
                ImGui.SetWindowSize(ImGui.GetWindowSize() + sizeDelta);

                if (!ImGui.IsMouseDragging(0, 0))
                    isDragging = false;
            }

            var min = new Vector2(bounds.min.X, bounds.min.Y);
            var max = new Vector2(bounds.max.X, bounds.max.Y);
            if (ImGui.IsMouseHoveringRect(min, max, false))
            {
                Gizmos.DrawRectangle(bounds.min.Xy, bounds.max.Xy, Color4.White, Color4.White);

                Cursor.Current = cursor;

                isHovering = true;
                if (ImGui.IsMouseDragging(0, 0))
                    isDragging = true;
            }
            else if (!isDragging)
                isHovering = false;
        }
    }
}
