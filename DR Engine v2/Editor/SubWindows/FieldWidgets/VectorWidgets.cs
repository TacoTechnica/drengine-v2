using System;
using System.Globalization;
using System.Reflection;
using Gtk;
using Microsoft.Xna.Framework;
using Action = System.Action;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class Vector2Widget : FieldWidget<Vector2>
    {
        private FloatView _x;
        private FloatView _y;

        protected override Vector2 Data
        {
            get => new Vector2(_x.Value, _y.Value);
            set
            {
                _x.Value = value.X;
                _y.Value = value.Y;
            }
        }

        protected override void Initialize(MemberInfo field, HBox content)
        {
            _x = new FloatView();
            _y = new FloatView();
            content.PackStart(_x, true, true, 8);
            content.PackStart(_y, true, true, 8);
            _x.Show();
            _y.Show();

            _x.Modified += OnModify;
            _y.Modified += OnModify;
        }
    }
    public class Vector3Widget : FieldWidget<Vector3>
    {
        private FloatView _x;
        private FloatView _y;
        private FloatView _z;

        protected override Vector3 Data
        {
            get => new Vector3(_x.Value, _y.Value, _z.Value);
            set
            {
                _x.Value = value.X;
                _y.Value = value.Y;
                _z.Value = value.Z;
            }
        }

        protected override void Initialize(MemberInfo field, HBox content)
        {
            _x = new FloatView();
            _y = new FloatView();
            _z = new FloatView();
            content.PackStart(_x, true, true, 8);
            content.PackStart(_y, true, true, 8);
            content.PackStart(_z, true, true, 8);
            _x.Show();
            _y.Show();
            _z.Show();

            _x.Modified += OnModify;
            _y.Modified += OnModify;
            _z.Modified += OnModify;
        }
    }
}