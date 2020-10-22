using System;
using Microsoft.Xna.Framework;

namespace DREngine.Game.UI
{
    public class Rect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y)
        {
        }

        public Rect(Rect toCopy)
        {
            X = toCopy.X;
            Y = toCopy.Y;
            Width = toCopy.Width;
            Height = toCopy.Height;
        }

        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set { X = value.X; Y = value. Y; }
        }

        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set { Width = value.X; Height = value.Y; }
        }

        public Vector2 Min
        {
            get => Position;
            set => Position = value;
        }

        public Vector2 Max
        {
            get => Position + Size;
            set => Size = (Max - Min);
        }

        public override string ToString()
        {
            return $"[Pos: ({X}, {Y}) Size: ({Width}, {Height})]";
        }
    }
}
