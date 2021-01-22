using System.IO;
using System.Reflection;
using DREngine.Editor.Components;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class StringPathWidget : AbstractPathWidget<string>
    {
        public StringPathWidget(DREditor editor, string title, string startPath = null, bool requireDirectory = false,
            string filter = null, string filterName = null) : base(editor, title, startPath, requireDirectory, filter,
            filterName)
        {
        }

        protected override void OnFilePicked(string file)
        {
            Data = file;
            OnModify();
        }
    }

    public abstract class AbstractPathWidget<T> : FieldWidget<T>
    {
        private readonly DREditor _editor;
        private readonly string _filter;
        private readonly string _filterName;
        private readonly bool _requireDirectory;
        private string _startPath;
        private readonly string _title;

        public AbstractPathWidget(DREditor editor, string title, string startPath = null, bool requireDirectory = false,
            string filter = null, string filterName = null)
        {
            _editor = editor;
            _title = title;
            _startPath = startPath;
            _requireDirectory = requireDirectory;
            _filter = filter;
            _filterName = filterName;
        }

        protected override T Data { get; set; }

        protected override void Initialize(MemberInfo field, HBox content)
        {
            var choose = new Button("(empty)");

            FileChooserDialog dialog = null;

            choose.Pressed += (sender, args) =>
            {
                if (dialog != null) return;
                using (dialog = new FileChooserDialog(_title, _editor.Window, FileChooserAction.Open, "Open",
                    ResponseType.Accept, "Cancel", ResponseType.Cancel))
                {
                    if (_startPath != null) dialog.SetCurrentFolder(_startPath);

                    if (_filter != null)
                    {
                        var filter = new FileFilter();
                        if (_filterName != null) filter.Name = _filterName;
                        filter.AddPattern(_filter);
                        dialog.AddFilter(filter);
                    }

                    var validPick = false;

                    while (!validPick)
                    {
                        var picked = (ResponseType) dialog.Run() == ResponseType.Accept;
                        validPick = true;

                        if (picked)
                        {
                            var path = dialog.Filename;

                            validPick = _requireDirectory && Directory.Exists(path) ||
                                        !_requireDirectory && File.Exists(path);

                            if (validPick)
                            {
                                choose.Label = path;
                                choose.Image = new ImageIcon(path, 64, 64);
                                OnFilePicked(path);
                            }
                        }
                    }

                    dialog.Dispose();
                    dialog = null;
                }
            };

            content.PackStart(choose, false, false, 4);
            choose.Show();
        }

        protected abstract void OnFilePicked(string file);
    }
}