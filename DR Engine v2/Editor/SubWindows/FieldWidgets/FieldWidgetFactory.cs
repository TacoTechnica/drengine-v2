using System;
using System.Reflection;
using GameEngine;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public static class FieldWidgetFactory
    {
        public static IFieldWidget CreateField(FieldInfo field)
        {
            IFieldWidget widget = GetNewField(field);
            widget.InitializeField(field);
            return widget;
        }
        
        private static IFieldWidget GetNewField(FieldInfo field)
        {
            Type type = field.FieldType;
            if (IsType(type, typeof(string)))
            {
                return new StringTextFieldWidget();
            }

            if (IsType(type, typeof(int)))
            {
                return new IntegerTextFieldWidget();
            }
            if (IsType(type, typeof(float)))
            {
                return new FloatTextFieldWidget();
            }

            return new UnknownFieldWidget($"Unsupported type: {type}.");
        }

        private static bool IsType(Type typeToCheck, Type type)
        {
            // type is the parent here
            return type.IsAssignableFrom(typeToCheck);
        }
    }
}