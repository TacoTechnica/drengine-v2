using System;
using System.Collections.Generic;
using System.Reflection;
using DREngine.Editor.Components;
using DREngine.Game.Resources;
using DREngine.ResourceLoading;
using Gtk;
using Microsoft.Xna.Framework;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    // Specify that this dictionary is a Pin field
    public class FieldPinEditorAttribute : OverrideFieldAttribute
    {
        public override IFieldWidget GetOverrideWidget(DREditor editor, UniFieldInfo field)
        {
            return new FieldPinEditorWidget(editor);
        }
    }

    public class FieldPinEditorWidget : FieldWidget<Dictionary<DRSprite.PinType, Vector2>>
    {
        protected override Dictionary<DRSprite.PinType, Vector2> Data
        {
            get
            {
                return _pins;
            }
            set
            {
                _pins.Clear();

                // Delete children
                List<Widget> toDelete = new List<Widget>(_list.Children);
                foreach (Widget child in toDelete)
                {
                    _list.Remove(child);
                }                

                // Re-Add
                foreach (DRSprite.PinType key in value.Keys)
                {
                    AddPinField(key, value[key]);
                }
                
            }
        }

        private Dictionary<DRSprite.PinType, Vector2> _pins = new Dictionary<DRSprite.PinType, Vector2>();

        private VBox _list;

        private DREditor _editor;

        private IChooser _currentChoice;

        public FieldPinEditorWidget(DREditor editor)
        {
            _editor = editor;
        }

        protected override void Initialize(UniFieldInfo field, HBox content)
        {
            
            
            _list = new VBox();
            content.PackStart(_list, true, true, 16);
            _list.Show();

            Button newPin = new Button(new Image(_editor.Icons.Add));

            newPin.Pressed += (sender, args) =>
            {
                // Open up a dropdown of options based on what we have left to add.
                _currentChoice?.Cancel();
                _currentChoice = new DropdownChooser();
                _currentChoice.MakeChoice(GetUnusedPinTypes(), type =>
                {
                    AddPinField(type, Vector2.Zero);
                });
            };

            content.PackEnd(newPin, true, false, 16);
            newPin.Show();
        }

        private IEnumerable<DRSprite.PinType> GetUnusedPinTypes()
        {
            foreach (DRSprite.PinType type in Enum.GetValues(typeof(DRSprite.PinType)))
            {
                if (!_pins.ContainsKey(type)) yield return type;
            }
        }

        private void AddPinField(DRSprite.PinType type, Vector2 value)
        {
            HBox line = new HBox();
            string title = type.ToString();
            line.PackStart(new Label(title), false, false, 16 );

            DRSprite sprite = (DRSprite) GetFieldParent();

            var x = new FloatSlider("X", 0, sprite.Width);//new FloatView("X");
            var y = new FloatSlider("Y", 0, sprite.Height);//new FloatView("Y");
            x.Value = value.X;
            y.Value = value.Y;
            x.Modified += () =>
            {
                var copy = _pins[type]; 
                copy.X = x.Value;
                _pins[type] = copy;
                OnModify();
            };
            y.Modified += () =>
            {
                var copy = _pins[type]; 
                copy.Y = y.Value;
                _pins[type] = copy;
                OnModify();
            };

            line.PackStart(x, true, true, 0);
            line.PackStart(y, true, true, 0);

            Button removeButton = new Button(new Image(_editor.Icons.Remove));

            removeButton.Pressed += (sender, args) =>
            {
                if (AreYouSureDialog.Run(_editor.Window, $"Delete Pin {type}?"))
                {
                    _list.Remove(line);
                    _pins.Remove(type);
                    OnModify();
                }
            };

            _list.PackStart(line, true, false, 16);
            line.ShowAll();
            _list.Show();

            _pins[type] = value;
        }

        protected override void OnDestroyed()
        {
            _currentChoice?.Cancel();
            base.OnDestroyed();
        }
    }
}
