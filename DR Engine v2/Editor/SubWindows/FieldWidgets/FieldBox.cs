using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;
using GameEngine.Game.Resources;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FieldBox : VBox
    {
        public object Target { get; private set; }

        private List<IFieldWidget> _fields = new List<IFieldWidget>();

        public Action Modified;

        public FieldBox(DREditor editor, Type type, bool autoApply = false)
        {
            foreach (FieldInfo f in type.GetFields().Where(f => f.IsPublic && !f.IsStatic && ShouldSerialize(f)))
            {
                IFieldWidget widget = FieldWidgetFactory.CreateField(editor, f); 
                _fields.Add(widget);

                widget.Modified += o =>
                {
                    if (autoApply)
                    {
                        SaveFields();
                    }
                    Modified?.Invoke();
                };

                if (widget is Widget w)
                {
                    PackStart(w, false, true, 4);
                    w.Show();
                }
                else
                {
                    throw new InvalidOperationException("Tried creating field widget that's not a UI widget. How?");
                }
            }
        }

        public void LoadTarget(object target)
        {
            Target = target;
            foreach (IFieldWidget field in _fields)
            {
                field.Load(target);
            }
        }

        public void SaveFields()
        {
            foreach (IFieldWidget field in _fields)
            {
                field.Apply();
            }            
        }

        protected virtual bool ShouldSerialize(FieldInfo f)
        {
            return true;
        }
    }

    public class ExtraDataFieldBox : FieldBox
    {
        public ExtraDataFieldBox(DREditor editor, Type type, bool autoApply = false) : base(editor, type, autoApply) { }
        protected override bool ShouldSerialize(FieldInfo f)
        {
            return f.GetCustomAttribute<ExtraDataAttribute>() != null;
        }
    }
}