using System;
using System.Reflection;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.Game.Scene;
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
            if (_fields != null)
            {
                this.Remove(_fields);
                _fields.Dispose();
            }
            _fields = new SceneObjectFieldBox(_editor, sceneObject.GetType())  {AutoApply = true};
            _fields.LoadTarget(sceneObject);
            _fields.Modified += FieldModified;

            _fields.Show();
            this.PackStart(_fields, false, true, 16);
        }


        class SceneObjectFieldBox : FieldBox
        {

            public SceneObjectFieldBox(DREditor editor, Type type) : base(editor, type, true)
            {
            }

            protected override bool ShouldSerialize(MemberInfo f)
            {
                bool parent = false;
                if (f is FieldInfo finfo)
                {
                    parent = (finfo.DeclaringType != Type);
                } else if (f is PropertyInfo pinfo)
                {
                    parent = (pinfo.DeclaringType != Type);
                }

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
        }
        
    }
}
