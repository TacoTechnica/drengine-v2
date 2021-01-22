using System;
using System.Collections.Generic;
using System.Reflection;
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
        
        public FieldBox(DREditor editor, Type type, bool includeParentFields = true)
        {
            Type = type;
            List<MemberInfo> fields = new List<MemberInfo>(type.GetFields());
            fields.AddRange(type.GetProperties());
            // put BASE fields up top and NEWER fields lower
            fields.Sort((left, right) =>
            {
                //Debug.Log($"{left.Name}: {GetDepth(left)} vs {right.Name}: {GetDepth(right)}");
                return GetDepth(right) - GetDepth(left);
                // How "deep" the field is declared in its sub types.
                int GetDepth(object field)
                {
                    int depth = 0;
                    Type check = type;
                    while (check != null && !TypeHasField(check, field))
                    {
                        check = check.BaseType;
                        depth++;
                    }

                    bool TypeHasField(Type type, object field)
                    {
                        if (field is FieldInfo finfo)
                        {
                            return finfo.DeclaringType == type;
                        } else if (field is PropertyInfo pinfo)
                        {
                            return pinfo.DeclaringType == type;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return depth;
                }
            });
            foreach (var f in fields)
            {

                if (f is FieldInfo finfo)
                {
                    if (!finfo.IsPublic || finfo.IsStatic) continue;
                    if (!includeParentFields && finfo.DeclaringType != type) continue;
                } else if (f is PropertyInfo pinfo)
                {
                    if (!includeParentFields && pinfo.DeclaringType != type) continue;
                }

                // ReSharper disable once VirtualMemberCallInConstructor
                if (!ShouldSerialize(f)) continue;
                if (f.GetCustomAttribute<FieldIgnoreAttribute>() != null) continue;

                var widget = FieldWidgetFactory.CreateField(editor, f);
                _fields.Add(widget);

                widget.Modified += o =>
                {
                    if (AutoApply) SaveFields();
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
            Target = target;
            foreach (var field in _fields) field.Load(target);
        }

        public void SaveFields()
        {
            foreach (var field in _fields) field.Apply();
        }

        protected virtual bool ShouldSerialize(MemberInfo f)
        {
            return true;
        }
    }

    public class ExtraDataFieldBox : FieldBox
    {
        public ExtraDataFieldBox(DREditor editor, Type type, bool includeParentFields = true) : base(editor, type, includeParentFields)
        {
        }

        protected override bool ShouldSerialize(MemberInfo f)
        {
            return f.GetCustomAttribute<ExtraDataAttribute>() != null;
        }
    }
}