using System;
using System.IO;
using GameEngine.Game;
using GameEngine.Game.Audio;
using GameEngine.Game.Resources;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.Components
{
    public class ResourceView : GenericTreeView
    {

        public Action<string> OnNewFolder;
        public Action<string> OnRename;
        public Action<string> OnMove;

        public Action<string, Type> OnNewResource;

        public ResourceView()
        {
            OnFileRightClicked += (projectPath, fullPath) =>
            {
                string projectDir = projectPath;
                bool isFile = File.Exists(fullPath);
                if (isFile)
                {
                    int lastSlash = projectPath.LastIndexOf("/", StringComparison.Ordinal);
                    if (lastSlash == -1)
                    {
                        projectDir = "";
                    }
                    else
                    {
                        projectDir = projectDir.Substring(0, lastSlash);
                    }
                }

                // Create popup with options.
                Menu menu = new Menu();

                if (isFile)
                {
                    Add("Rename", () =>
                    {
                        OnRename?.Invoke(projectPath);
                    });
                    Add("Move", () =>
                    {
                        OnMove?.Invoke(projectPath);
                    });

                    Separator();
                }

                Add("New Folder", () =>
                {
                    OnNewFolder?.Invoke(projectDir);
                });
                
                Separator();

                Add("New Sprite", () =>
                {
                    OnNewResource?.Invoke(projectDir, typeof(Sprite));
                });
                Add("New AudioClip", () =>
                {
                    OnNewResource?.Invoke(projectDir, typeof(AudioClip));
                });
                Add("New Font", () =>
                {
                    OnNewResource?.Invoke(projectDir, typeof(Font));
                });
                
                menu.Show();
                menu.Popup();

                void Add(string name, Action onClick)
                {
                    MenuItem item = new MenuItem(name);
                    item.Activated += (sender, args) =>
                    {
                        onClick?.Invoke();
                    };
                    menu.Append(item);
                    item.Show();
                }

                void Separator()
                {
                    SeparatorMenuItem s = new SeparatorMenuItem();
                    menu.Append(s);
                    s.Show();
                }

            };
        }
        
    }
}
