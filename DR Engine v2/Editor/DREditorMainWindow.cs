using System;
using System.IO;
using DREngine.Editor.Components;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Audio;
using GameEngine.Game.Resources;
using Gdk;
using GLib;
using Gtk;
using MonoGame.Framework.Utilities.Deflate;
using Action = System.Action;
using Menu = Gtk.Menu;
using MenuItem = Gtk.MenuItem;
using WindowType = Gtk.WindowType;
using Path = GameEngine.Game.Path;

namespace DREngine.Editor
{
    public class DREditorMainWindow : Gtk.Window
    {

        private DREditor _editor;

        private ResourceView _resourceView;
        private EditorLog _log;

        public Icons Icons;

        public Action<String, String> OnFileOpened
        {
            get => _resourceView.OnFileOpened; set => _resourceView.OnFileOpened = value;
        }

        public DREditorMainWindow(DREditor editor) : base(WindowType.Toplevel)
        {
            _editor = editor;
            // Make window is called externally.
        }

#region Public Control

        public void MakeWindow(string title, int width, int height)
        {

            _log = new EditorLog();
            Icons = new Icons();


            Title = title;
            Resize(width, height);
            this.Maximize();
            this.Resizable = true;

            VBox mainBox = new VBox();
            Widget menuBar = MakeMenuBar();
            Widget projectButtons = MakeProjectActionButtonBar();

            HPaned vertPane = new HPaned();
            vertPane.WideHandle = true; // Separate left and right side
            vertPane.Position = 256; // How wide the left side starts

            _resourceView = new ResourceView(Icons);
            HookupLog(_log);
            HookupResourceView(_resourceView);
            VPaned rightSide = new VPaned();
            rightSide.WideHandle = true;
            Widget emptyBox = new VBox();
            rightSide.Pack1(emptyBox, false, false);
            emptyBox.Show();
            rightSide.Pack2(_log, true, false);
            _log.Show();
            //rightSide.Position = 256;
            vertPane.Pack1(_resourceView, false, false);
            _resourceView.Show();
            vertPane.Pack2(rightSide, true, false);
            rightSide.Show();

            mainBox.PackStart(menuBar, false, true, 4);
            menuBar.Show();
            mainBox.PackStart(projectButtons, false, false, 4);
            projectButtons.Show();
            mainBox.PackStart(vertPane, true, true, 4);
            vertPane.Show();

            mainBox.Show();

            Add(mainBox);

        }

        private void HookupLog(EditorLog log)
        {
            Debug.OnLogDebug += log.PrintDebug;
            Debug.OnLogPrint += log.Print;
            Debug.OnLogWarning += log.PrintWarning;
            Debug.OnLogError += (message, trace) =>
            {
                log.PrintError($"{message}\n{trace}");
            };
            Debug.LogDebug("Editor Log Initialized");
        }

        private void HookupResourceView(ResourceView resourceView)
        {
            resourceView.OnNewFolder += projectDir =>
            {
                if (!AssertProjectLoaded()) return;

                using NewFolderDialog dialog = new NewFolderDialog(_editor, this, new ProjectPath(_editor, projectDir));
                if (dialog.RunUntilAccept())
                {
                    ProjectPath toAdd = dialog.GetTargetDirectory();
                    Directory.CreateDirectory(toAdd);
                    Debug.LogSilent($"New Folder: {toAdd.RelativePath}");
                    resourceView.AddFolder(toAdd.RelativePath, true);
                }
            };

            resourceView.OnNewResource += (projectDir, type) =>
            {
                if (!AssertProjectLoaded()) return;

                if (type == typeof(Sprite))
                {
                    using NewSpriteDialog dialog = new NewSpriteDialog(_editor, this, new ProjectPath(_editor, projectDir));
                    if (dialog.RunUntilAccept())
                    {
                        ProjectPath toAdd = dialog.GetTargetDirectory();
                        Path toCopy = dialog.ImageToCopy;
                        File.Copy(toCopy, toAdd);
                        Debug.LogSilent($"New Sprite: {toAdd.RelativePath}");
                        resourceView.AddFile(toAdd.RelativePath, true);
                    }
                } else if (type == typeof(Font))
                {
                    using NewFontDialog dialog = new NewFontDialog(_editor, this, new ProjectPath(_editor, projectDir));
                    if (dialog.RunUntilAccept())
                    {
                        ProjectPath toAdd = dialog.GetTargetDirectory();
                        Path toCopy = dialog.FontToCopy;
                        File.Copy(toCopy, toAdd);
                        Debug.LogSilent($"New Font: {toAdd.RelativePath}");
                        resourceView.AddFile(toAdd.RelativePath, true);
                    }
                } else if (type == typeof(AudioClip))
                {
                    using NewAudioClipDialog dialog = new NewAudioClipDialog(_editor, this, new ProjectPath(_editor, projectDir));
                    if (dialog.RunUntilAccept())
                    {
                        ProjectPath toAdd = dialog.GetTargetDirectory();
                        Path toCopy = dialog.AudioToCopy;
                        File.Copy(toCopy, toAdd);
                        Debug.LogSilent($"New Audio Clip: {toAdd.RelativePath}");
                        
                        // Load and save extra file...
                        AudioClip clip = new AudioClip();
                        clip.Type = dialog.ImportType;
                        clip.Save(toAdd);

                        resourceView.AddFile(toAdd.RelativePath, true);
                    }                    
                }
                else
                {
                    AlertProblem($"Resource creation not implemented yet: {type}. Sorry!");
                }
            };
        }

        private bool AssertProjectLoaded()
        {
            if (!_editor.ProjectLoaded)
            {
                AlertProblem("Project not loaded.");
                return false;
            }

            return true;
        }


        public void EmptyProject()
        {
            _resourceView.Clear();
        }
        public void LoadProject(ProjectData data, string fullPath, Action<string> OnFileLoad = null)
        {
            if (fullPath.EndsWith("project.json"))
            {
                fullPath = fullPath.Substring(0, fullPath.Length - "project.json".Length);
            }
            
            _resourceView.Clear();
            _resourceView.LoadDirectory(fullPath,
                dir =>
                {
                    // We don't really care about directories, do we?
                },
                file =>
                {
                    string relative = System.IO.Path.GetRelativePath(fullPath, file);
                    OnFileLoad?.Invoke(relative);
                }
            );
        }

        /// <summary>
        /// Set the theme of the window. Must be a valid gtk.css file.
        /// </summary>
        public void SetTheme(string path)
        {
            bool failed = false;
            try
            {
                Gtk.CssProvider cssProvider = new Gtk.CssProvider();
                if (!cssProvider.LoadFromPath(path))
                {
                    failed = true;
                }
                else
                {
                    Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);
                }
            }
            catch (GLib.GException)
            {
                failed = true;
            }

            if (failed) {
                AlertProblem("Failed to load theme",$"Invalid theme path: \"{path}\"\n\nMake sure the target is a valid gtk.css file!");
            }
        }

        public void AlertProblem(string title, string message)
        {
            Debug.LogWarning($"Problem: {message}");
            MessageDialog popup = new MessageDialog(
                this,
                DialogFlags.DestroyWithParent,
                MessageType.Error,
                ButtonsType.Ok,
                message
            ) {Title = title, WindowPosition = WindowPosition.Center};
            popup.Show();
            popup.Run();
            popup.Dispose();
        }

        public void AlertProblem(string message)
        {
            AlertProblem("Problem!", message);
        }

#endregion

#region Widget/UI Construction

        /// <summary>
        /// File, Edit, View, Build, Help, etc...
        /// </summary>
        private Widget MakeMenuBar()
        {
            MenuBar menuBar = new MenuBar();

            Menu fileMenu = new Menu();

            MenuItem file = new MenuItem("File");
            file.Submenu = fileMenu;
            
            MenuItem newProject = new MenuItem("New Project");
            newProject.Activated += (sender, args) => NewProjectPressed(); 
            fileMenu.Append(newProject);
            MenuItem openProject = new MenuItem("Open Project");
            openProject.Activated += (sender, args) => OpenProjectPressed(); 
            fileMenu.Append(openProject);
            MenuItem reloadProject = new MenuItem("Reload Project");
            reloadProject.Activated += (sender, args) => _editor.ReloadCurrentProject(); 
            fileMenu.Append(reloadProject);

            menuBar.Append(file);
            menuBar.ShowAll();

            return menuBar;
        }

        /// <summary>
        ///    Contains the run button and other useful buttons for the project.
        /// </summary>
        private Widget MakeProjectActionButtonBar()
        {
            Box b = new HBox();
            b.HeightRequest = 16;

            // Make the action buttons
            Button newProj = NewButton("New Empty Project", Icons.New, NewProjectPressed);
            Button open = NewButton("Open Project", Icons.Open, OpenProjectPressed);
            Separator s1 = new HSeparator();
            Button export = NewButton("Export Project", Icons.Export, ExportProjectPressed);
            Separator s2 = new HSeparator();
            Button run = new RunProjectButton(_editor);//NewButton("Run Project", Icons.Play, RunProjectPressed);
            s2.Hexpand = true;
            b.PackStart(newProj, false, false, 0);
            newProj.Show();
            b.PackStart(open, false, false, 0);
            open.Show();
            b.PackStart(s1, false, false, 10);
            s1.Show();
            b.PackStart(export, false, false, 0);
            export.Show();
            b.PackStart(s2, false, false, 10);
            s2.Show();
            b.PackStart(run, false, false, 0);
            run.Show();

            return b;
        }

        private void NewProjectPressed()
        {

            if (!EnsureNobodyDirty()) return;

            using NewProjectDialog dialog = new NewProjectDialog(_editor, this, "New Project");

            if (dialog.RunUntilAccept())
            {
                // Create project. No problems found.
                _editor.ResourceWindowManager.ForceCloseAllWindows();

                // Create path
                Path folderToCreate = dialog.GetTargetPath();
                Directory.CreateDirectory(folderToCreate);

                Path projectPath = folderToCreate + "/project.json";

                ProjectData newProject = new ProjectData();
                newProject.Name = dialog.ProjectTitle;
                newProject.Author = dialog.Author;

                // Create json.txt
                ProjectData.WriteToFile(projectPath, newProject);

                // Create icon if we specified one.
                if (dialog.IconPath != null)
                {
                    File.Copy(dialog.IconPath, folderToCreate + "/icon.png");
                }

                // We did it! Now Load.
                _editor.LoadProject(projectPath);

            }

        }

        private void OpenProjectPressed()
        {
            if (!EnsureNobodyDirty()) return;

            using FileChooserDialog chooser = new FileChooserDialog("Open Project", this, FileChooserAction.Open,
                "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
            chooser.Filter = new FileFilter {Name = "DR Project File"};
            chooser.Filter.AddPattern("project.json");

            chooser.SetCurrentFolder(new EnginePath("projects"));

            if ((ResponseType) chooser.Run() == ResponseType.Accept)
            {
                _editor.ResourceWindowManager.ForceCloseAllWindows();

                _editor.LoadProject(chooser.Filename);
            }
        }

        private void ExportProjectPressed()
        {
            AlertProblem("This feature is not implemented yet. Sorry!");
        }

        private Button NewButton(string name, Pixbuf icon, Action OnPress = null)
        {
            Button result = new Button();
            result.TooltipText = name;

            result.Pressed += (sender, args) =>
            {
                OnPress?.Invoke();
            };
            //file.Label = "F";
            if (icon == null)
            {
                result.Label = name;
            } else {
                result.Image = new Image(icon);
            }

            return result;
        }


        /// <summary>
        ///    Contains the content windows.
        /// </summary>
        private Widget MakeContentView()
        {
            Box b = new HBox();
            Button button = new Button {Label = "Right side"};
            b.PackStart(button, false, false, 0);
            button.Show();
            return b;
        }

        private bool EnsureNobodyDirty()
        {
            if (_editor.ResourceWindowManager.AnyWindowDirty())
            {
                string message = "Some open resources have unsaved changes, Load anyway and discard changes?";

                Dialog dialogue = new AreYouSureDialog(this, "Unsaved Changes", message, "Load and Discard Changes", "Cancel");
                //MessageDialog dialogue = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Question,
                //    ButtonsType.OkCancel, false, message);
                bool ok = (ResponseType) dialogue.Run() == ResponseType.Accept;
                return ok;
            }

            return true;
        }

#endregion

#region Garbage stuff

        public DREditorMainWindow(IntPtr raw) : base(raw)
        {
            Debug.LogError("Invalid constructor.");
        }

        public DREditorMainWindow(int nothing) : base("Invalid")
        {
            Debug.LogError("Invalid constructor.");
        }

#endregion
    }
}
