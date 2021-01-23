using System;
using System.Reflection;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.Game.Scene;
using DREngine.ResourceLoading;
using GameEngine;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneObjectFields : VBox
    {
        private DREditor _editor;

        public Action<string, object> FieldModified;

        private FieldBox _fields;

        public SceneObjectFields(DREditor editor)
        {
            _editor = editor;
        }

        public void LoadObject(ISceneObject sceneObject)
        {
            if (_fields == null || _fields.Target == null || sceneObject.GetType() != _fields.Target.GetType())
            {
                if (_fields != null)
                {
                    this.Remove(_fields);
                    _fields.Dispose();
                    foreach (Widget w in _fields.Children)
                    {
                        w.Dispose();
                    }
                }

                _fields = new SceneObjectFieldBox(_editor, sceneObject.GetType())  {AutoApply = true};
                _fields.Modified += FieldModified;
                _fields.Show();
                PackStart(_fields, false, true, 16);
            }
            _fields.LoadTarget(sceneObject);
        }


        class SceneObjectFieldBox : FieldBox
        {

            public SceneObjectFieldBox(DREditor editor, Type type) : base(editor, type, true)
            {
            }

            protected override bool ShouldSerialize(UniFieldInfo f)
            {
                bool parent = f.DeclaringType != Type;

                if (parent)
                {
                    // Whitelist
                    switch (f.Name)
                    {
                        case "Transform":
                            return true;
                    }

                    return false;
                }
                
                return base.ShouldSerialize(f);
            }

            protected override bool GetOverridePriority(UniFieldInfo field, ref int priority)
            {
                switch (field.Name)
                {
                    case "Name":
                        priority = 1000000; // Name goes on top.
                        return true;
                    default:
                        return false;
                }
            }
        }
        
    }
}
