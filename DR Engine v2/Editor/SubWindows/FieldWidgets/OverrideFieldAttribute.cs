using System;
using System.Reflection;
using DREngine.ResourceLoading;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public abstract class OverrideFieldAttribute : Attribute
    {
        public abstract IFieldWidget GetOverrideWidget(DREditor editor, UniFieldInfo field);
    }
}