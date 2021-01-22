using System;
using System.Reflection;

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

        public override IFieldWidget GetOverrideWidget(DREditor editor, MemberInfo field)
        {
            return new FieldContainerWidget(editor);
        }
    }
}