using System;
using System.Linq;
using System.Reflection;
using GameEngine;
using GameEngine.Game;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public static class FieldWidgetFactory
    {
        public static IFieldWidget CreateField(DREditor editor, FieldInfo field)
        {
            IFieldWidget widget = GetNewField(editor, field);
            widget.InitializeField(field);
            return widget;
        }
        
        private static IFieldWidget GetNewField(DREditor editor, FieldInfo field)
        {
            Type type = field.FieldType;
            
            // Check for OVERRIDES
            
            // Container attribute
            OverrideFieldAttribute fieldOverride = field.GetCustomAttribute<OverrideFieldAttribute>();
            if (fieldOverride != null)
            {
                return fieldOverride.GetOverrideWidget(editor, field);
            }
            
            // STRING FIELD
            if (IsType(type, typeof(string)))
            {
                return new StringTextFieldWidget();
            }

            // DEFAULT NUMBERS
            if (IsType(type, typeof(int)))
            {
                return new IntegerTextFieldWidget();
            }
            if (IsType(type, typeof(float)))
            {
                return new FloatTextFieldWidget();
            }

            // MISC
            if (IsType(type, typeof(OverridablePath)))
            {
                ResourceTypeAttribute attrib = field.GetCustomAttribute<ResourceTypeAttribute>();
                if (attrib == null)
                {
                    throw new InvalidOperationException($"You forgot to add an attribute for {type.Name}.");
                }
                Type subType = attrib.Type;
                return new OverridablePathFieldWidget(editor, subType);
            }


            return new UnknownFieldWidget($"Unsupported type: {type}.");
        }

        private static bool IsType(Type typeToCheck, Type type)
        {
            // type is the parent here
            return type.IsAssignableFrom(typeToCheck);
        }

        private static bool IsTypeGeneric(Type typeToCheck, Type generic)
        {
            try
            {
                return IsType(typeToCheck.GetGenericTypeDefinition(), generic);
            }
            catch (Exception)
            {
                // If this is not a generic it will throw an exception.
                // If that happens, we're OK since we know one thing!
                return false;
            }
        }
    }
}