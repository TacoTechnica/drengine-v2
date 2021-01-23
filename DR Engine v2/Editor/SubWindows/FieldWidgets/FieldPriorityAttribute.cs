using System;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FieldPriorityAttribute : Attribute
    {
        public readonly int Priority;

        public FieldPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}