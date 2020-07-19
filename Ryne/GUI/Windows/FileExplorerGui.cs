using System.Collections.Generic;
using System.IO;
using Ryne.Utility;

namespace Ryne.Gui.Windows
{
    class FileExplorerGui : PopupGui
    {
        private readonly string SearchDirectory;
        private readonly string FileExtensions;

        private readonly List<string> OpenFolders;

        // File with path
        public string SelectedFile { get; protected set; }
        // File name without path
        public string SelectedFileName => new FileInfo(SelectedFile).Name;

        /// <summary>
        /// Opens a menu to select a file
        /// </summary>
        /// <param name="directory">Specific folder inside the Global.Config.WorkingDirectory</param>
        public FileExplorerGui(ImGuiWrapper gui, string directory, string fileExtensions, string windowTitle = "FileExplorer") : base(gui, windowTitle, false)
        {
            SearchDirectory = directory;
            FileExtensions = fileExtensions;

            OpenFolders = new List<string>();
        }

        public override void RenderContents()
        {
            if (PopupActive)
            {
                string result = RenderDirectory(Global.Config.WorkingDirectory + SearchDirectory);
                if (!string.IsNullOrEmpty(result))
                {
                    SelectedFile = result;
                    Active = false;
                    ExecuteCallback = true;
                }

                base.RenderContents();
            }
        }

        private string RenderDirectory(string directory, int subLevel = 0)
        {
            Gui.PushStyleColor(RyneImGuiColor.ImGuiCol_Button, new Float4(0, 0, 0, 0));

            string indent = new string(' ', subLevel * 4);
            string openDirectoryText = indent + "v ";
            string closedDirectoryText = indent + "> ";

            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                var name = new DirectoryInfo(subDirectory).Name;
                bool isOpen = OpenFolders.Contains(subDirectory);

                string text = (isOpen ? openDirectoryText : closedDirectoryText) + name;

                Gui.PushStyleColor(RyneImGuiColor.ImGuiCol_Text, new Float4(1, 1, 0, 1));

                if (Gui.Button(text))
                {
                    if (isOpen)
                    {
                        OpenFolders.Remove(subDirectory);
                    }
                    else
                    {
                        OpenFolders.Add(subDirectory);
                    }
                }

                Gui.PopStyleColor();

                if (isOpen)
                {
                    var result = RenderDirectory(subDirectory, subLevel+1);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Gui.PopStyleColor();
                        return result;
                    }
                }
            }

            bool checkFileExtensions = FileExtensions != "*";
            var info = new DirectoryInfo(directory);
            foreach (var file in info.EnumerateFiles())
            {
                if (checkFileExtensions && (string.IsNullOrEmpty(file.Extension) || !FileExtensions.Contains(file.Extension)))
                {
                    continue;
                }

                if (Gui.Button(indent + "    " + file.Name))
                {
                    Gui.PopStyleColor();
                    var dir = directory.Substring(Global.Config.WorkingDirectory.Length).Replace('\\', '/');
                    return dir + "/" + file.Name;
                }
            }

            Gui.PopStyleColor();
            return "";
        }
    }
}
