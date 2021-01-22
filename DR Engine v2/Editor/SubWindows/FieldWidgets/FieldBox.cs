using System;
using System.Collections.Generic;
using System.Reflection;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game.Resources;
using Gtk;
using Microsoft.VisualBasic.FileIO;
using Action = System.Action;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FieldBox : VBox
    {
        private readonly List<IFieldWidget> _fields = new List<IFieldWidget>();

        public Action<string, object> Modified;

        public bool AutoApply = false;

        public Type Type;

        private bool _internalModifyFlag = false;
        
        public FieldBox(DREditor editor, Type type, bool includeParentFields = true)
        {
            Type = type;
            List<UniFieldInfo> fields = new List<UniFieldInfo>(type.GetUniFields());
            // put BASE fields up top and NEWER fields lower
            fields.Sort((left, right) =>
            {
                //Debug.Log($"{left.Name}: {GetDepth(left)} vs {right.Name}: {GetDepth(right)}");
                return GetDepth(right) - GetDepth(left);
                // How "deep" the field is declared in its sub types.
                int GetDepth(UniFieldInfo field)
                {
                    int depth = 0;
                    Type check = type;
                    while (check != null && !TypeHasField(check, field))
                    {
                        check = check.BaseType;
                        depth++;
                    }

                    bool TypeHasField(Type type, UniFieldInfo field)
                    {
                        return field.DeclaringType == type;
                    }
                    return depth;
                }
            });
            foreach (var f in fields)
            {

                if (f.IsStatic) continue;
                if (!includeParentFields && f.DeclaringType != type) continue;
                if (!f.HasGetter || !f.HasSetter) continue;

                // ReSharper disable once VirtualMemberCallInConstructor
                if (!ShouldSerialize(f)) continue;
                if (f.GetCustomAttribute<FieldIgnoreAttribute>() != null) continue;

                var widget = FieldWidgetFactory.CreateField(editor, f);
                _fields.Add(widget);

                widget.Modified += o =>
                {
                    if (AutoApply) SaveFields();
                    if (_internalModifyFlag) return;
                    Modified?.Invoke(f.Name, o);
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
            _internalModifyFlag = true;
            Target = target;
            foreach (var field in _fields) field.Load(target);
            _internalModifyFlag = false;
        }

        public void SaveFields()
        {
            foreach (var field in _fields) field.Apply();
        }

        protected virtual bool ShouldSerialize(UniFieldInfo f)
        {
            return true;
        }
    }

    public class ExtraDataFieldBox : FieldBox
    {
        public ExtraDataFieldBox(DREditor editor, Type type, bool includeParentFields = true) : base(editor, type, includeParentFields)
        {
        }

        protected override bool ShouldSerialize(UniFieldInfo f)
        {
            return f.GetCustomAttribute<ExtraDataAttribute>() != null;
        }
    }
}