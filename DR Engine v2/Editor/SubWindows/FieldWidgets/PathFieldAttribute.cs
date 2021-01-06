using System.Reflection;
using GameEngine.Game;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public abstract class PathFieldAttribute : OverrideFieldAttribute
    {
        public string Filter;
        public string FilterName;
        public string Title;

        public PathFieldAttribute(string title = "Choose Path", string filter = null, string filterName = null)
        {
            Title = title;
            Filter = filter;
            FilterName = filterName;
        }

        protected abstract string GetStatPath(DREditor editor);
    }

    public class DirectoryFieldAttribute : PathFieldAttribute
    {
        public DirectoryFieldAttribute(string title = "Choose Directory", string filter = null,
            string filterName = null) : base(title, filter, filterName)
        {
        }

        public override IFieldWidget GetOverrideWidget(DREditor editor, FieldInfo field)
        {
            return new StringPathWidget(editor, Title, GetStatPath(editor), true, Filter, FilterName);
        }

        protected override string GetStatPath(DREditor editor)
        {
            return null;
        }
    }

    public class FileFieldAttribute : PathFieldAttribute
    {
        public FileFieldAttribute(string title = "Choose File", string filter = null, string filterName = null) : base(
            title, filter, filterName)
        {
        }

        public override IFieldWidget GetOverrideWidget(DREditor editor, FieldInfo field)
        {
            return new StringPathWidget(editor, Title, GetStatPath(editor), false, Filter, FilterName);
        }

        protected override string GetStatPath(DREditor editor)
        {
            return null;
        }
    }

    public class EngineFileFieldAttribute : FileFieldAttribute
    {
        public string RelativePath;

        public EngineFileFieldAttribute(string relativePath, string title = "Choose File", string filter = null,
            string filterName = null) : base(title, filter, filterName)
        {
            RelativePath = relativePath;
        }

        protected override string GetStatPath(DREditor editor)
        {
            return new EnginePath(RelativePath);
        }
    }

    public class EngineDirectoryFieldAttribute : DirectoryFieldAttribute
    {
        public string RelativePath;

        public EngineDirectoryFieldAttribute(string relativePath, string title = "Choose File", string filter = null,
            string filterName = null) : base(title, filter, filterName)
        {
            RelativePath = relativePath;
        }

        protected override string GetStatPath(DREditor editor)
        {
            return new EnginePath(RelativePath);
        }
    }
}