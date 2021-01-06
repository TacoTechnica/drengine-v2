using System;
using System.Reflection;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public abstract class OverrideFieldAttribute : Attribute
    {
        public abstract IFieldWidget GetOverrideWidget(DREditor editor, FieldInfo field);
    }
}