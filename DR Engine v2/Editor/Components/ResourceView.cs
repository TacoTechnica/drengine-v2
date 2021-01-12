using System;
using System.IO;
using DREngine.Game.Resources;
using DREngine.Game.VN;
using GameEngine.Game.Resources;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.Components
{
    public class ResourceView : GenericTreeView
    {
        public Action<string> OnMove;

        public Action<string> OnNewFolder;

        public Action<string, Type> OnNewResource;
        public Action<string> OnRename;

        public ResourceView(Icons icons) : base(icons)
        {
            OnFileRightClicked += (projectPath, fullPath) =>
            {
                var projectDir = projectPath;
                var isFile = File.Exists(fullPath);
                if (isFile)
                {
                    var lastSlash = projectPath.LastIndexOf("/", StringComparison.Ordinal);
                    if (lastSlash == -1)
                        projectDir = "";
                    else
                        projectDir = projectDir.Substring(0, lastSlash);
                }

                // Create popup with options.
                var menu = new Menu();

                if (isFile)
                {
                    Append("Rename", () => { OnRename?.Invoke(projectPath); });
                    Append("Move", () => { OnMove?.Invoke(projectPath); });

                    Separator();
                }

                Append("New Folder", () => { OnNewFolder?.Invoke(projectDir); });

                Separator();

                Append("New Visual Novel Script", () => { OnNewResource?.Invoke(projectDir, typeof(VNScript)); });
                Append("New Sprite", () => { OnNewResource?.Invoke(projectDir, typeof(DRSprite)); });
                Append("New AudioClip", () => { OnNewResource?.Invoke(projectDir, typeof(AudioClip)); });
                Append("New Font", () => { OnNewResource?.Invoke(projectDir, typeof(Font)); });

                menu.Show();
                menu.Popup();

                void Append(string name, Action onClick)
                {
                    var item = new MenuItem(name);
                    item.Activated += (sender, args) => { onClick?.Invoke(); };
                    menu.Append(item);
                    item.Show();
                }

                void Separator()
                {
                    var s = new SeparatorMenuItem();
                    menu.Append(s);
                    s.Show();
                }
            };
        }
    }
}