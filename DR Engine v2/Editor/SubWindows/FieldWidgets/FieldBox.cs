using System;
using System.Collections.Generic;
using System.Reflection;
using GameEngine.Game.Resources;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FieldBox : VBox
    {
        private readonly List<IFieldWidget> _fields = new List<IFieldWidget>();

        public Action Modified;

        public FieldBox(DREditor editor, Type type, bool autoApply = false)
        {
            foreach (var f in type.GetFields())
            {
                if (!f.IsPublic || f.IsStatic) continue;
                if (!ShouldSerialize(f)) continue;
                if (f.GetCustomAttribute<FieldIgnoreAttribute>() != null) continue;


                var widget = FieldWidgetFactory.CreateField(editor, f);
                _fields.Add(widget);

                widget.Modified += o =>
                {
                    if (autoApply) SaveFields();
                    Modified?.Invoke();
                };

                if (widget is Widget w)
                {
                    //Debug.Log($"ADDED WIDGET: {widget} => {f.Name}");
                    PackStart(w, true, true, 4);
                    w.Show();
                }
                else
                {
                    throw new InvalidOperationException("Tried creating field widget that's not a UI widget. How?");
                }
            }
        }

        public object Target { get; private set; }

        public void LoadTarget(object target)
        {
            Target = target;
            foreach (var field in _fields) field.Load(target);
        }

        public void SaveFields()
        {
            foreach (var field in _fields) field.Apply();
        }

        protected virtual bool ShouldSerialize(FieldInfo f)
        {
            return true;
        }
    }

    public class ExtraDataFieldBox : FieldBox
    {
        public ExtraDataFieldBox(DREditor editor, Type type, bool autoApply = false) : base(editor, type, autoApply)
        {
        }

        protected override bool ShouldSerialize(FieldInfo f)
        {
            return f.GetCustomAttribute<ExtraDataAttribute>() != null;
        }
    }
}