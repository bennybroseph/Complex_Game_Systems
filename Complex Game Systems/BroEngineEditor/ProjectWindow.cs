namespace BroEngineEditor
{
    using System.IO;
    using System.Numerics;
    using System.Reflection;
    using ImGuiNET;

    internal class ProjectWindow : ResizableWindow
    {
        public override string name => "Project";

        public DirectoryInfo selecteDirectoryInfo { get; set; }

        public ProjectWindow()
        {
            m_AllowResizeDirections = Direction.North | Direction.East;
        }

        protected override void PreTryResize()
        {
            ImGuiNative.igSetWindowPos(
                new Vector2(0f, MyGameWindow.main.Height - ImGui.GetWindowHeight()), SetCondition.Always);

            ImGui.SetWindowSize(
                new Vector2(MyGameWindow.main.Width * 0.4f, MyGameWindow.main.Height * 0.35f),
                SetCondition.Once);
        }

        protected override void DrawWindowElements()
        {
            ImGui.Columns(2, "Project Window Columns", true);

            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //ImGui.BeginChild()
            DrawFolders(path);

            ImGui.NextColumn();
            if(selecteDirectoryInfo != null)
                foreach (var fileInfo in selecteDirectoryInfo.GetFiles())
                    ImGui.Selectable(fileInfo.Name);
        }

        private void DrawFolders(string path)
        {
            ImGuiNative.igIndent();
            foreach (var directory in Directory.GetDirectories(path))
            {
                var directoryInfo = new DirectoryInfo(directory);
                if (ImGui.Selectable(directoryInfo.Name))
                    selecteDirectoryInfo = directoryInfo;

                DrawFolders(directory);
            }
            ImGuiNative.igUnindent();
        }
    }
}
