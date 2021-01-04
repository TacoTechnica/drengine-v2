using System;
using GameEngine;
using Gtk;

namespace DREngine.Editor.SubWindows
{
    public class UnknownResourceWindow : SubWindow
    {
        private ProjectPath _path;

        private string _extension;

        public UnknownResourceWindow(DREditor editor, ProjectPath path, string extension) : base(editor, $"Unknown Resource: {path.RelativePath}")
        {
            _path = path;
            _extension = extension;
        }

        protected override void OnInitialize()
        {
            Text label = new Text(
                $"Cannot open resource at \"{_path.ToString()}\" because it has unknown extension \"{_extension}\".\n\n" +
                "If this is truly a valid resource that should be openable, rename the file to match a valid extension.\n" +
                "Otherwise, you must use a different editor that can open this file.\n\nSorry!");
            label.Show();
            Add(label);
        }
    }
}