using System;
using System.Numerics;
using Pango;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DREngine.Game.UI
{

    /// <summary>
    /// This sets how a UI element will be positioned, using margins, anchors and pivots.
    /// It does NOT represent the state of the UI itself, but rather its layout strategy. This is best kept fixed.
    /// Thus this will NOT store things like rotation, scale, translations that might animate.
    /// </summary>
    public class Layout
    {
        public Margin Margin = new Margin();
        public Vector2 AnchorMin = Vector2.Zero, AnchorMax = Vector2.One;
        public Vector2 Pivot = 0.5f * Vector2.One;

        // unwrapped enums
        /// <summary>
        /// Corner
        /// </summary>
        public const int
            TopLeft = 4,
            TopRight = 5,
            BottomLeft = 6,
            BottomRight = 7;
        /// <summary>
        /// Side
        /// </summary>
        public const int
            Top = 0,
            Bottom = 1,
            Left = 2,
            Right = 3;

        public Rect GetTargetRect(Rect parent)
        {
            Vector2 anchorMinRelative = parent.Min + parent.Size * AnchorMin,
                    anchorMaxRelative = parent.Min + parent.Size * AnchorMax;
            Vector2 posMin = anchorMinRelative + Margin.Min,
                    posMax = anchorMaxRelative - Margin.Max;
            return new Rect(posMin, posMax - posMin);
        }

        public Layout OffsetBy(float x, float y)
        {
            Margin.Offset(x, y);
            return this;
        }

        public Layout WithPivot(float x, float y)
        {
            Pivot = new Vector2(x, y);
            return this;
        }

        /// <summary>
        ///    Give a layout that's fullscreen.
        ///     Optional offsets from the corners.
        /// </summary>
        public static Layout FullscreenLayout(Vector2 minOffset, Vector2 maxOffset)
        {
            return new Layout
            {
                AnchorMin = Vector2.Zero,
                AnchorMax = Vector2.One,
                Margin = new Margin(minOffset, maxOffset)
            };
        }

        public static Layout FullscreenLayout(float xMinOffset, float yMinOffset, float xMaxOffset, float yMaxOffset)
        {
            return FullscreenLayout(new Vector2(xMinOffset, yMinOffset), new Vector2(xMaxOffset, yMaxOffset));
        }
        public static Layout FullscreenLayout()
        {
            return FullscreenLayout(Vector2.Zero, Vector2.Zero);
        }

        /// <summary>
        ///    Give an empty layout that will create a rect of size zero
        ///     on the top left corner.
        /// </summary>
        public static Layout EmptyLayout()
        {
            return new Layout();
        }

        /// <summary>
        ///    Give a layout that's scaled.
        /// </summary>
        public static Layout ScaledLayout(float leftPercent, float topPercent, float rightPercent, float botPercent)
        {
            return new Layout
            {
                AnchorMin = new Vector2(leftPercent, topPercent),
                AnchorMax = new Vector2(rightPercent, botPercent)
            };
        }

        public static Layout CenteredLayout(float width, float height)
        {
            return new Layout
            {
                AnchorMin = Vector2.One / 2f,
                AnchorMax = Vector2.One / 2f,
                Margin = new Margin(-height/2, -height/2, -width/2, -width/2)
            };
        }

        public static Layout CornerLayout(int corner, float width, float height)
        {
            Vector2 anchor = Vector2.Zero;
            Vector2 delta = Vector2.Zero;
            switch (corner)
            {
                case TopLeft:
                    break;
                case TopRight:
                    anchor.X = 1;
                    delta.X = -width;
                    break;
                case BottomLeft:
                    anchor.Y = 1;
                    delta.Y = -height;
                    break;
                case BottomRight:
                    anchor = Vector2.One;
                    delta.X = -width;
                    delta.Y = -height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
            }
            return new Layout
            {
                AnchorMin = anchor,
                AnchorMax = anchor,
                Margin = new Margin(delta.Y, -delta.Y - height, delta.X, -delta.X - width)
            };
        }
        public static Layout SideLayout(int side, float size, float padding = 0)
        {
            Layout result = new Layout();
            result.Margin = new Margin(padding, padding, padding, padding);
            switch (side)
            {
                case Top:
                    result.AnchorMin = Vector2.Zero;
                    result.AnchorMax = Vector2.UnitX;
                    result.Margin.Bottom = -size;
                    break;
                case Bottom:
                    result.AnchorMin = Vector2.UnitY;
                    result.AnchorMax = Vector2.One;
                    result.Margin.Top = -size;
                    break;
                case Left:
                    result.AnchorMin = Vector2.Zero;
                    result.AnchorMax = Vector2.UnitY;
                    result.Margin.Right = -size;
                    break;
                case Right:
                    result.AnchorMin = Vector2.UnitX;
                    result.AnchorMax = Vector2.One;
                    result.Margin.Left = -size;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
            return result;
        }

        public override string ToString()
        {
            return $"Margin={Margin}, Anchors=({AnchorMin}, {AnchorMax}), Pivot={Pivot}";
        }
    }

    public class Margin
    {
        public float Top;
        public float Bottom;
        public float Left;
        public float Right;

        public Margin(float top, float bottom, float left, float right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Margin(Vector2 min, Vector2 max) : this(min.Y, max.Y, min.X, max.X)
        {
            // Vector constructor
        }

        public Margin() : this(0, 0, 0, 0)
        {
            // Empty constructor
        }

        public void Offset(float x, float y)
        {
            Top += y;
            Bottom -= y;
            Left += x;
            Right -= x;
        }

        public Vector2 Min
        {
            get => new Vector2(Left, Top);
            set
            {
                Left = value.X;
                Right = value.Y;
            }
        }
        public Vector2 Max
        {
            get => new Vector2(Right, Bottom);
            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        public override string ToString()
        {
            return $"({Top}, {Bottom}, {Left}, {Right}";
        }
    }
}
