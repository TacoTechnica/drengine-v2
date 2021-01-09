using System;
using System.Collections.Generic;
using System.Reflection;
using GameEngine;
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
            List<FieldInfo> fields = new List<FieldInfo>(type.GetFields());
            // put BASE fields up top and NEWER fields lower
            fields.Sort((left, right) =>
            {
                //Debug.Log($"{left.Name}: {GetDepth(left)} vs {right.Name}: {GetDepth(right)}");
                return GetDepth(right) - GetDepth(left);
                // How "deep" the field is declared in its sub types.
                int GetDepth(FieldInfo field)
                {
                    int depth = 0;
                    Type check = type;
                    while (check != null && !TypeHasField(check, field))
                    {
                        check = check.BaseType;
                        depth++;
                    }

                    bool TypeHasField(Type type, FieldInfo field)
                    {
                        return field.DeclaringType == type;
                    }
                    return depth;
                }
            });
            foreach (var f in fields)
            {
                if (!f.IsPublic || f.IsStatic) continue;
                // ReSharper disable once VirtualMemberCallInConstructor
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