using System;
using System.Collections.Generic;
using System.IO;
using GameEngine;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class OverridablePathFieldWidget : AbstractFieldGameResource<OverridablePath>
    {
        private const string DEFAULT_NAME = "(default)";
        protected override OverridablePath ResourceData { get; set; }
        
        public OverridablePathFieldWidget(DREditor editor, Type type) : base(editor, type)
        {
        }

        protected override string ResourceToString(OverridablePath resource)
        {
            return resource.Overrided? resource.OverrideProjectPath : DEFAULT_NAME;
        }

        protected override void OnPathSelected(string path)
        {
            if (path == DEFAULT_NAME) path = null;
            Debug.Log($"SELECTED: {path}");
            ResourceData.OverrideProjectPath = path;
            
            // Trigger modification
            Data = ResourceData;
            OnModify();
        }

        protected override IEnumerable<string> GetExtraResults(Type t, string path)
        {
            yield return DEFAULT_NAME;
        }

        protected override bool AcceptPath(ProjectPath path)
        {
            return File.Exists(path.ToString()) || (path.RelativePath == DEFAULT_NAME);
        }
    }
}
