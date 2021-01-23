using DREngine.ResourceLoading;
using GameEngine.Util;
using Gtk;
using Microsoft.Xna.Framework;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class QuaternionEulerFieldWidget : FieldWidget<Quaternion>
    {
        private FloatView _x;
        private FloatView _y;
        private FloatView _z;

        protected override Quaternion Data
        {
            get => Math.FromEuler(_x.Value, _y.Value, _z.Value);
            set
            {
                Vector3 euler = Math.ToEuler(value);
                _x.Value = euler.X;
                _y.Value = euler.Y;
                _z.Value = euler.Z;
            }
        }

        protected override void Initialize(UniFieldInfo field, HBox content)
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