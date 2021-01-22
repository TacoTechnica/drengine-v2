using System;
using System.Reflection;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.Game.Resources;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game.Objects;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DREngine.Editor.SubWindows
{
    public static class FieldWidgetFactory
    {
        public static IFieldWidget CreateField(DREditor editor, MemberInfo field)
        {
            var widget = GetNewField(editor, field);
            widget.InitializeField(field);
            return widget;
        }

        private static IFieldWidget GetNewField(DREditor editor, MemberInfo field)
        {

            Type type = null;
            if (field is FieldInfo finfo)
            {
               type = finfo.FieldType;
            }
            if (field is PropertyInfo pinfo)
            {
                type = pinfo.PropertyType;
            }
            // Check for OVERRIDES

            // Container attribute
            var fieldOverride = field.GetCustomAttribute<OverrideFieldAttribute>();
            if (fieldOverride != null) return fieldOverride.GetOverrideWidget(editor, field);

            // STRING FIELD
            if (IsType(type, typeof(string))) return new StringTextFieldWidget();
            // BOOL
            if (IsType(type, typeof(bool))) return new BoolField();

            // DEFAULT NUMBERS
            if (IsType(type, typeof(int))) return new IntegerTextFieldWidget();
            if (IsType(type, typeof(float))) return new FloatTextFieldWidget();

            // Enum
            if (IsType(type, typeof(Enum))) return new GenericEnumWidget();

            // Vector2
            if (IsType(type, typeof(Vector2))) return new Vector2Widget();
            // Vector3
            if (IsType(type, typeof(Vector3))) return new Vector3Widget();

            // Transform
            if (IsType(type, typeof(Transform3D))) return new FieldWidgetAuto<Transform3D>(editor);

            // Project Resources
            if (IsType(type, typeof(IGameResource)))
            {
                JsonConverterAttribute resourceLoader;
                try
                {
                    resourceLoader = field.GetCustomAttribute<JsonConverterAttribute>();
                }
                catch (Exception)
                {
                    return new UnknownFieldWidget($"Project Resource of type {type?.Name} not explicitly set to convert to Project Resource. (Blame dev)");
                }

                // We're supposed to convert to a project resource path.
                if (IsType(resourceLoader.ConverterType, typeof(ProjectResourceConverter)))
                {
                    if (IsType(type, typeof(DRSprite))) return new SpriteResourceField(editor);
                    if (IsType(type, typeof(AudioClip))) return new AudioResourceField(editor);
                    if (IsType(type, typeof(Font))) return new FontResourceField(editor);
                }
            }

            // Misc simple types
            if (IsType(type, typeof(Rect))) return new RectFieldWidget();
            if (IsType(type, typeof(Margin))) return new MarginFieldWidget(editor);

            // MISC
            if (IsType(type, typeof(OverridablePath)))
            {
                var attrib = field.GetCustomAttribute<ResourceTypeAttribute>();
                if (attrib == null)
                    throw new InvalidOperationException($"You forgot to add an attribute for {type.Name}.");
                var subType = attrib.Type;
                return new OverridablePathFieldWidget(editor, subType);
            }


            return new UnknownFieldWidget($"Unsupported type: {type?.Name}.");
        }

        private static bool IsType(Type typeToCheck, Type type)
        {
            // type is the parent here
            return type.IsAssignableFrom(typeToCheck);
        }

        // ReSharper disable once UnusedMember.Local
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