using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FieldBox : VBox
    {
        public object Target { get; private set; }

        private List<IFieldWidget> _fields = new List<IFieldWidget>();

        public Action Modified;

        public FieldBox(Type type)
        {
            foreach (FieldInfo f in type.GetFields().Where(f => f.IsPublic && !f.IsStatic))
            {
                IFieldWidget widget = FieldWidgetFactory.CreateField(f); 
                _fields.Add(widget);

                widget.Modified += o =>
                {
                    Modified.Invoke();
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
    }
}