namespace BroEngineEditor
{
    //using System;
    using System.Windows.Forms;

    using BroEngine;

    using Geometry.Shapes;

    using ImGuiNET;

    using ImGuiUtility;

    using OpenTK;
    using OpenTK.Graphics;

    using Vector2 = System.Numerics.Vector2;

    internal abstract class ResizableWindow : Object
    {
        [System.Flags]
        internal enum Direction
        {
            None = 0,

            North = 1 << 0,
            South = 1 << 1,
            East = 1 << 2,
            West = 1 << 3,
        }

        protected WindowFlags m_WindowFlags = WindowFlags.NoMove | WindowFlags.NoResize;

        protected Direction m_AllowResizeDirections =
            Direction.North | Direction.South | Direction.East | Direction.West;

        protected Direction m_HoveringDirections;
        protected Direction m_ResizingDirections;

        protected Vector2 m_OffsetPos;

        public ResizableWindow()
        {
            ImGuiOpenTK.drawGui += DrawGui;
        }

        protected virtual void PreTryResize() { }
        protected virtual void DrawWindowElements() { }

        public void DrawGui()
        {
            if (ImGui.BeginWindow(name, m_WindowFlags))
            {
                PreTryResize();
                if (!ImGui.IsAnyItemActive())
                {
                    if (ImGui.IsMouseReleased(0))
                        m_ResizingDirections = Direction.None;

                    SetFlags();
                    SetCursor();

                    Resize();
                }

                DrawWindowElements();
            }
            ImGui.EndWindow();
        }

        #region SetFlags
        protected void SetFlags()
        {
            if (HasFlag(m_AllowResizeDirections, Direction.North))
                SetFlagNorth();
            if (HasFlag(m_AllowResizeDirections, Direction.South))
                SetFlagSouth();
            if (HasFlag(m_AllowResizeDirections, Direction.East))
                SetFlagEast();
            if (HasFlag(m_AllowResizeDirections, Direction.West))
                SetFlagWest();
        }

        protected void SetFlagNorth()
        {
            var center =
                new Vector3(
                    ImGui.GetWindowPosition().X + ImGui.GetWindowWidth() / 2f,
                    ImGui.GetWindowPosition().Y, 0f);
            var size = new Vector3(ImGui.GetWindowWidth(), 10f, 0f);

            SetDirectionFlag(new Bounds(center, size), Direction.North);
        }
        protected void SetFlagSouth()
        {
            var center =
                new Vector3(
                    ImGui.GetWindowPosition().X + ImGui.GetWindowWidth() / 2f,
                    ImGui.GetWindowPosition().Y + ImGui.GetWindowHeight(), 0f);
            var size = new Vector3(ImGui.GetWindowWidth(), 10f, 0f);

            SetDirectionFlag(new Bounds(center, size), Direction.South);
        }
        protected void SetFlagEast()
        {
            var center =
                new Vector3(
                    ImGui.GetWindowPosition().X + ImGui.GetWindowWidth(),
                    ImGui.GetWindowPosition().Y + ImGui.GetWindowHeight() / 2f, 0f);
            var size = new Vector3(10, ImGui.GetWindowHeight(), 0f);

            SetDirectionFlag(new Bounds(center, size), Direction.East);
        }
        protected void SetFlagWest()
        {
            var center =
                new Vector3(
                    ImGui.GetWindowPosition().X,
                    ImGui.GetWindowPosition().Y + ImGui.GetWindowHeight() / 2f, 0f);
            var size = new Vector3(10, ImGui.GetWindowHeight(), 0f);

            SetDirectionFlag(new Bounds(center, size), Direction.West);
        }
        #endregion

        protected void SetDirectionFlag(Bounds bounds, Direction direction)
        {
            var min = new Vector2(bounds.min.X, bounds.min.Y);
            var max = new Vector2(bounds.max.X, bounds.max.Y);
            if (ImGui.IsMouseHoveringRect(min, max, false))
            {
                SetFlag(ref m_HoveringDirections, direction);
                if (ImGui.IsMouseClicked(0))
                {
                    SetFlag(ref m_ResizingDirections, direction);
                    m_OffsetPos = ImGui.GetMousePos() - ImGui.GetWindowPosition();
                }
            }
            else
                UnsetFlag(ref m_HoveringDirections, direction);
        }

        protected void SetCursor()
        {
            var direction =
                m_ResizingDirections == Direction.None ? m_HoveringDirections : m_ResizingDirections;

            if (HasFlag(direction, Direction.North) || HasFlag(direction, Direction.South))
                Cursor.Current = Cursors.SizeNS;
            else if (HasFlag(direction, Direction.East) || HasFlag(direction, Direction.West))
                Cursor.Current = Cursors.SizeWE;

            if (HasFlag(direction, Direction.North) && HasFlag(direction, Direction.West) ||
                HasFlag(direction, Direction.South) && HasFlag(direction, Direction.East))
                Cursor.Current = Cursors.SizeNWSE;

            if (HasFlag(direction, Direction.North) && HasFlag(direction, Direction.East) ||
                HasFlag(direction, Direction.South) && HasFlag(direction, Direction.West))
                Cursor.Current = Cursors.SizeNESW;
        }

        #region Resize
        protected unsafe void Resize()
        {
            var io = *ImGuiNative.igGetIO();

            if (HasFlag(m_ResizingDirections, Direction.North) &&
                HasFlag(m_ResizingDirections, Direction.West))
                ResizeDirection(io, new Vector2(1, 1), new Vector2(-1, -1));
            else if (HasFlag(m_ResizingDirections, Direction.North) &&
                     HasFlag(m_ResizingDirections, Direction.East))
                ResizeDirection(io, new Vector2(0, 1), new Vector2(1, -1));
            else if (HasFlag(m_ResizingDirections, Direction.South) &&
                     HasFlag(m_ResizingDirections, Direction.West))
                ResizeDirection(io, new Vector2(1, 0), new Vector2(-1, 1));
            else if (HasFlag(m_ResizingDirections, Direction.South) &&
                     HasFlag(m_ResizingDirections, Direction.East))
                ResizeDirection(io, new Vector2(0, 0), new Vector2(1, 1));
            else
            {
                if (HasFlag(m_ResizingDirections, Direction.North))
                    ResizeDirection(io, new Vector2(0, 1f), new Vector2(0, -1));
                if (HasFlag(m_ResizingDirections, Direction.South))
                    ResizeDirection(io, new Vector2(0, 0), new Vector2(0, 1));
                if (HasFlag(m_ResizingDirections, Direction.East))
                    ResizeDirection(io, new Vector2(0, 0), new Vector2(1, 0));
                if (HasFlag(m_ResizingDirections, Direction.West))
                    ResizeDirection(io, new Vector2(1, 0), new Vector2(-1, 0));
            }
        }
        protected void ResizeDirection(NativeIO io, Vector2 posDirection, Vector2 sizeDirection)
        {
            ImGuiNative.igSetWindowPos(
                new Vector2(
                    posDirection.X == 1 ? io.MousePos.X - m_OffsetPos.X : ImGui.GetWindowPosition().X,
                    posDirection.Y == 1 ? io.MousePos.Y + m_OffsetPos.Y : ImGui.GetWindowPosition().Y),
                SetCondition.Always);

            ImGui.SetWindowSize(ImGui.GetWindowSize() + io.MouseDelta * sizeDirection, SetCondition.Always);
        }
        #endregion

        protected bool HasFlag(Direction direction, Direction flag) { return (direction & flag) == flag; }

        protected void SetFlag(ref Direction direction, Direction flag) { direction |= flag; }
        protected void UnsetFlag(ref Direction direction, Direction flag) { direction &= ~flag; }
    }
}
