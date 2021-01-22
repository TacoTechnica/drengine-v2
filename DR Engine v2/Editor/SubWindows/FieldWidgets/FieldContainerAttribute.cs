using System;
using System.Reflection;
using DREngine.ResourceLoading;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldContainerAttribute : OverrideFieldAttribute
    {
        public readonly string OverrideTitle;

        public FieldContainerAttribute(string overrideTitle = "")
        {
            OverrideTitle = overrideTitle;
        }

        public override IFieldWidget GetOverrideWidget(DREditor editor, UniFieldInfo field)
        {
            return new FieldContainerWidget(editor);
        }
    }
}