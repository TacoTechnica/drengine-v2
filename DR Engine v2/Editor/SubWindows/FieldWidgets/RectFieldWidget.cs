using System.Reflection;
using GameEngine.Game.UI;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class RectFieldWidget : FieldWidget<Rect>
    {

        private FloatView _x;
        private FloatView _y;
        private FloatView _width;
        private FloatView _height;

        protected override Rect Data
        {
            get
            {
                return new Rect(_x.Value, _y.Value, _width.Value, _height.Value);
            }
            set
            {
                _x.Value = value.X;
                _y.Value = value.Y;
                _width.Value = value.Width;
                _height.Value = value.Height;
            }
        }

        protected override void Initialize(FieldInfo field, HBox content)
        {
            _x = new FloatView("X");
            _y = new FloatView("Y");
            _width = new FloatView("Width");
            _height = new FloatView("Height");

            VBox left = new VBox(),
                right = new VBox();

            left.PackStart(_x, true, true, 8);
            right.PackStart(_y, true, true, 8);
            left.PackStart(_width, true, true, 8);
            right.PackStart(_height, true, true, 8);

            content.PackStart(left, true, true, 16);
            content.PackStart(right, true, true, 16);
            
            _x.Show();
            _y.Show();
            _width.Show();
            _height.Show();

            _x.Modified += OnModify;
            _y.Modified += OnModify;
            _width.Modified += OnModify;
            _height.Modified += OnModify;
        }
    }
}