using System;
using System.Timers;
using Cairo;
using DREngine.Editor.Components;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.Game.Resources;
using DREngine.ResourceLoading;
using GameEngine.Game.Resources;
using Gdk;
using Gtk;
using Color = Cairo.Color;

namespace DREngine.Editor.SubWindows.Resources
{
    public class SpriteResourceWindow : ResourceWindow<DRSprite>
    {
        private readonly DREditor _editor;

        private FieldBox _fields;

        private Image _image;
        private Text _label;

        private Timer _renderTimer;
        private float _dt;

        public SpriteResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
        }

        protected override void OnInitialize(Box container)
        {
            var scroll = new ScrolledWindow();
            scroll.MinContentWidth = 300 + 16 * 2;
            scroll.MinContentHeight = 300 + 16 * 2;
            _image = new Image();
            _image.Xpad = 16;
            _image.Ypad = 16;
            scroll.Add(_image);
            _label = new Text("Nothing loaded");
            _fields = new ExtraDataFieldBox(_editor, typeof(DRSprite), true);

            container.PackStart(scroll, true, true, 16);
            //container.PackStart(_image, true, true, 4);
            container.PackEnd(_label, false, false, 16);
            container.PackEnd(_fields, false, true, 16);
            scroll.Show();
            _image.Show();
            _label.Show();
            _fields.Show();

            _renderTimer = new Timer();
            _renderTimer.Interval = 250; // milliseconds
            _renderTimer.Elapsed += (sender, args) =>
            {
                _dt++;
                _image.QueueDraw();
            };
            _renderTimer.Start();

            _image.Drawn += (o, args) => {
                // Draw border around image and allocation
                DRSprite spr = CurrentResource;
                // AllocatedWidth and Height is the TOTAL SIZE of the widget (counting all padding)
                float realWidth = _image.Pixbuf.Width,
                    realHeight = _image.Pixbuf.Height;
                float realX = (_image.AllocatedWidth - realWidth) / 2, realY = (_image.AllocatedHeight - realHeight) / 2;
                if (spr == null) return;
                Context g = args.Cr;

                // Draw Pivot
                DrawPivot(spr.Pivot.X, spr.Pivot.Y);

                // Draw image outline
                Rect(0, 0, spr.Width, spr.Height, new Color(0.5, 1, 0.5, 0.6), 2, false );

                // Draw margin border
                RectDoubleDashed(spr.ScaleMargin.Left, spr.ScaleMargin.Top, spr.Width - (spr.ScaleMargin.Right + spr.ScaleMargin.Left),(spr.Height - (spr.ScaleMargin.Top + spr.ScaleMargin.Bottom)),
                    new Color(1, 0, 1), new Color(0.2, 0.6, 0.2), 1);

                
                void DrawPivot(float x, float y)
                {
                    float px = realX + realWidth * x,
                        py = realY + realHeight * y;
                    float length = 10;
                    g.LineJoin = LineJoin.Round;
                    g.LineWidth = 3;
                    g.SetSourceColor(new Color(0, 0, 0, 1));
                    g.MoveTo(px, py);
                    g.LineTo(px + length, py);
                    g.MoveTo(px, py);
                    g.LineTo(px - length, py);
                    g.MoveTo(px, py);
                    g.LineTo(px, py + length);
                    g.MoveTo(px, py);
                    g.LineTo(px, py - length);
                    g.Stroke();
                    g.LineWidth = 2;
                    g.SetSourceColor(new Color(1, 1, 1, 1));
                    g.MoveTo(px + length, py);
                    g.LineTo(px, py + length);
                    g.LineTo(px - length, py);
                    g.LineTo(px, py - length);
                    g.LineTo(px + length, py);
                    g.Stroke();
                }

                void RectDoubleDashed(float x, float y, float w, float h, Color cIn, Color cOut, float lineWidth)
                {
                    Rect(x, y, w, h, cIn, lineWidth, true, true);
                    Rect(x, y, w, h, cOut, lineWidth, true, false);
                }

                void Rect(float x, float y, float w, float h, Color c, float lineWidth, bool dashed, bool dashOffs=true)
                {
                    if (dashed)
                    {
                        double onLength = 10,
                               offLength = 5;
                        double offset = _dt * 7;
                        if (dashOffs)
                        {
                            g.SetDash(new[] {onLength, offLength}, offset);
                        }
                        else
                        {
                            g.SetDash(new[] {offLength, onLength}, offset + offLength);
                        }
                    }

                    g.SetSourceColor(c);
                    g.LineWidth = lineWidth;
                    g.LineJoin = LineJoin.Round;
                    float innerX = realX + realWidth * (x / spr.Width);
                    float innerY = realY + realHeight * (y / spr.Height);
                    float innerW = realWidth * (w / spr.Width);
                    float innerH = realHeight * (h / spr.Height);
                    g.Rectangle( innerX, innerY, innerW, innerH);
                    g.Stroke();
                }
            };

            _fields.Modified += OnModify;
        }

        private void OnModify()
        {
            _image.QueueDraw();
            
            MarkDirty();
        }

        protected override void OnOpen(DRSprite resource, Box container)
        {
            // Load sprite
            _image.File = resource.Path;

            // Scale
            var old = _image.Pixbuf;
            _image.Pixbuf = _editor.Icons.ScaleToRegularSize(_image.Pixbuf, 300);
            old.Dispose();

            _label.Text = $"{resource.Width} x {resource.Height} Sprite";
            _fields.LoadTarget(resource);
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            // display error
            _image.Clear();
            _label.Text = $"ERROR: {exception}";
        }

        protected override void OnClose()
        {
            if (Dirty) CurrentResource?.Load(_editor.ResourceLoaderData);
            if (_renderTimer != null)
            {
                _renderTimer.Dispose();
                _renderTimer = null;
            }
        }
    }
}