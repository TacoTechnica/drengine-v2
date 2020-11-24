using System;
using GameEngine;
using Gtk;
using MonoGame.Framework.Utilities.Deflate;

namespace DREngine.Editor
{
    public class DREditorMainWindow : Gtk.Window
    {

        public DREditorMainWindow() : base(WindowType.Toplevel)
        {
            // Make window is called externally.
        }

#region Public Control

        public void MakeWindow(string title, int width, int height)
        {
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

            Widget resourceViewer = MakeResourceViewer();
            Widget contentView = MakeContentView();
            vertPane.Pack1(resourceViewer, false, false);
            resourceViewer.Show();
            vertPane.Pack2(contentView, true, false);
            contentView.Show();

            mainBox.PackStart(menuBar, false, true, 4);
            menuBar.Show();
            mainBox.PackStart(projectButtons, false, false, 4);
            projectButtons.Show();
            mainBox.PackStart(vertPane, true, true, 4);
            vertPane.Show();

            mainBox.Show();

            Add(mainBox);
        }

        /// <summary>
        /// Set the theme of the window. Must be a valid gtk.css file.
        /// </summary>
        public void SetTheme(string path)
        {
            bool failed = false;
            try
            {
                Gtk.CssProvider css_provider = new Gtk.CssProvider();
                if (!css_provider.LoadFromPath(path))
                {
                    failed = true;
                }
                else
                {
                    Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, css_provider, 800);
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
            Debug.LogError($"Problem: {message}");
            MessageDialog popup = new MessageDialog(
                this,
                DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Ok,
                message
            );
            popup.Title = title;
            popup.WindowPosition = WindowPosition.Center;
            popup.ButtonReleaseEvent += (o, args) =>
            {
                popup.Dispose();
            };
            popup.Show();
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
            // TODO: Add more options
            MenuItem fileItem = new MenuItem("File");
            menuBar.Add(fileItem);
            fileItem.Show();
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
            // TODO: Standardize so we don't have to manually do this.
            Button file = new Button();
            file.Label = "F";
            Button open = new Button();
            open.Label = "O";
            Button save = new Button();
            save.Label = "S";
            Separator s = new HSeparator();
            Button export = new Button();
            export.Label = "E";
            b.PackStart(file, false, false, 0);
            file.Show();
            b.PackStart(open, false, false, 0);
            open.Show();
            b.PackStart(save, false, false, 0);
            save.Show();
            b.PackStart(s, false, false, 10);
            s.Show();
            b.PackStart(export, false, false, 0);
            export.Show();

            return b;
        }

        /// <summary>
        ///    Contains the resource tree.
        /// </summary>
        private Widget MakeResourceViewer()
        {
            Box b = new VBox();
            TreeView tree = new TreeView
            {
                //EnableSearch = true,
                //HeadersClickable = false,
                //HeadersVisible = false
            };

            // Set up generic tree stuff
            TreeViewColumn fileName = new TreeViewColumn {Title = "filename", Visible = true};
            CellRendererText fileNameCell = new Gtk.CellRendererText ();
            fileName.PackStart(fileNameCell, false);
            tree.AppendColumn(fileName);
            fileName.AddAttribute(fileNameCell, "text", 0);

            TreeStore store = new TreeStore(typeof(string));

            // TODO: Standardize into functions and turn into a more defined class.
            TreeIter iter = store.AppendValues("Rooms");
            store.AppendValues(iter, "floor0.room");
            store.AppendValues(iter, "my_room.room");
            store.AppendValues(iter, "trial.room");

            iter = store.AppendValues("Characters");
            store.AppendValues(iter, "billy");
            store.AppendValues(iter, "you");

            iter = store.AppendValues("Sprites");
            store.AppendValues(iter, "billy_sad");
            store.AppendValues(iter, "billy_happy");
            TreeIter nagitoSub = store.AppendValues(iter, "nagito");
            store.AppendValues(nagitoSub, "nagito1");
            store.AppendValues(nagitoSub, "nagito2");
            store.AppendValues(iter, "obama");

            tree.Model = store;

            Label bottom = new Label() {Text = "TODO: Add search here"};

            b.PackStart(tree, true, true, 4);
            tree.Show();
            //b.PackEnd(bottom, false, false, 4);
            //bottom.Show();
            return b;

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
