using System;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldContainerAttribute : Attribute
    {
        public string OverrideTitle;

        public FieldContainerAttribute(string overrideTitle = "")
        {
            OverrideTitle = overrideTitle;
        }
    }
}