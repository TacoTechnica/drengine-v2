using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace GameEngine.Game.UI
{
    /// <summary>
    ///     This sets how a UI element will be positioned, using margins, anchors and pivots.
    ///     It does NOT represent the state of the UI itself, but rather its layout strategy. This is best kept fixed.
    ///     Thus this will NOT store things like rotation, scale, translations that might animate.
    /// </summary>
    public class Layout
    {
        // unwrapped enums
        /// <summary>
        ///     Corner
        /// </summary>
        public const int
            TopLeft = 4,
            TopRight = 5,
            BottomLeft = 6,
            BottomRight = 7;

        /// <summary>
        ///     Side
        /// </summary>
        public const int
            Top = 0,
            Bottom = 1,
            Left = 2,
            Right = 3;

        public Vector2 AnchorMin = Vector2.Zero, AnchorMax = Vector2.One;
        public Margin Margin = new Margin();
        public Vector2 Pivot = 0.5f * Vector2.One;

        // Empty constructor
        public Layout()
        {
        }

        public Layout(Layout toCopy) : this()
        {
            Margin = new Margin(toCopy.Margin);
            AnchorMin = toCopy.AnchorMin;
            AnchorMax = toCopy.AnchorMax;
        }

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

        public Layout OffsetBy(Vector2 offset)
        {
            return OffsetBy(offset.X, offset.Y);
        }

        public Layout WithPivot(float x, float y)
        {
            Pivot = new Vector2(x, y);
            return this;
        }

        /// <summary>
        ///     Give a layout that's fullscreen.
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

        public static Layout FullscreenLayout(float padding)
        {
            return FullscreenLayout(padding, padding, padding, padding);
        }

        public static Layout FullscreenLayout()
        {
            return FullscreenLayout(Vector2.Zero, Vector2.Zero);
        }

        /// <summary>
        ///     Give an empty layout that will create a rect of size zero
        ///     on the top left corner.
        /// </summary>
        public static Layout EmptyLayout()
        {
            return new Layout();
        }

        public static Layout CustomLayout(Vector2 anchorMin, Vector2 anchorMax, Margin margin)
        {
            return new Layout
            {
                AnchorMin = anchorMin,
                AnchorMax = anchorMax,
                Margin = margin
            };
        }

        public static Layout CustomLayout(float anchorMinX = 0, float anchorMinY = 0, float anchorMaxX = 1,
            float anchorMaxY = 1, float marginLeft = 0, float marginTop = 0, float marginRight = 0, float marginBot = 0)
        {
            return CustomLayout(new Vector2(anchorMinX, anchorMinY), new Vector2(anchorMaxX, anchorMaxY),
                new Margin(marginLeft, marginTop, marginRight, marginBot));
        }

        /// <summary>
        ///     Give a layout that's scaled.
        /// </summary>
        public static Layout ScaledLayout(float leftPercent, float topPercent, float rightPercent, float botPercent)
        {
            return new Layout
            {
                AnchorMin = new Vector2(leftPercent, topPercent),
                AnchorMax = new Vector2(rightPercent, botPercent)
            };
        }

        public static Layout CenteredLayout(float width = 0, float height = 0)
        {
            return new Layout
            {
                AnchorMin = Vector2.One / 2f,
                AnchorMax = Vector2.One / 2f,
                Margin = new Margin(-width / 2, -height / 2, -width / 2, -height / 2)
            };
        }

        public static Layout CornerLayout(int corner, float width = 0, float height = 0)
        {
            var anchor = Vector2.Zero;
            var delta = Vector2.Zero;
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
                case Top:
                    anchor.X = 0.5f;
                    delta.X = -width / 2;
                    break;
                case Bottom:
                    anchor.X = 0.5f;
                    anchor.Y = 1f;
                    delta.X = -width / 2;
                    delta.Y = -height;
                    break;
                case Left:
                    anchor.Y = 0.5f;
                    delta.Y = -height / 2;
                    break;
                case Right:
                    anchor.X = 1f;
                    anchor.Y = 0.5f;
                    delta.X = -width;
                    delta.Y = -height / 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
            }

            return new Layout
            {
                AnchorMin = anchor,
                AnchorMax = anchor,
                Margin = new Margin(delta.X, delta.Y, -delta.X - width, -delta.Y - height)
            };
        }

        public static Layout SideStretchLayout(int side, float size = 0, float padding = 0, float offset = 0)
        {
            var result = new Layout {Margin = new Margin(padding, padding, padding, padding)};
            switch (side)
            {
                case Top:
                    result.AnchorMin = Vector2.Zero;
                    result.AnchorMax = Vector2.UnitX;
                    result.Margin.Bottom = -size - offset;
                    result.Margin.Top = offset;
                    break;
                case Bottom:
                    result.AnchorMin = Vector2.UnitY;
                    result.AnchorMax = Vector2.One;
                    result.Margin.Bottom = offset;
                    result.Margin.Top = -size - offset;
                    break;
                case Left:
                    result.AnchorMin = Vector2.Zero;
                    result.AnchorMax = Vector2.UnitY;
                    result.Margin.Left = offset;
                    result.Margin.Right = -size - offset;
                    break;
                case Right:
                    result.AnchorMin = Vector2.UnitX;
                    result.AnchorMax = Vector2.One;
                    result.Margin.Left = -size - offset;
                    result.Margin.Right = offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            return result;
        }

        public Layout WithMargin(Margin margin)
        {
            Margin = margin;
            return this;
        }

        public override string ToString()
        {
            return $"Margin={Margin}, Anchors=({AnchorMin}, {AnchorMax}), Pivot={Pivot}";
        }
    }

    public class Margin
    {
        public float Bottom;
        public float Left;
        public float Right;
        public float Top;

        public Margin(float left, float top, float right, float bottom)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Margin(Vector2 min, Vector2 max) : this(min.X, min.Y, max.X, max.Y)
        {
            // Vector constructor
        }

        public Margin(float margin) : this(margin, margin, margin, margin)
        {
            // Single margin constructor
        }

        public Margin() : this(0)
        {
            // Empty constructor
        }

        // Copy constructor
        public Margin(Margin toCopy) : this(toCopy.Left, toCopy.Top, toCopy.Right, toCopy.Bottom)
        {
        }

        [JsonIgnore]
        public Vector2 Min
        {
            get => new Vector2(Left, Top);
            set
            {
                Left = value.X;
                Right = value.Y;
            }
        }

        [JsonIgnore]
        public Vector2 Max
        {
            get => new Vector2(Right, Bottom);
            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        public void Offset(float x, float y)
        {
            Top += y;
            Bottom -= y;
            Left += x;
            Right -= x;
        }

        public void Offset(Vector2 offset)
        {
            Offset(offset.X, offset.Y);
        }

        public void SetPosition(float x, float y)
        {
            SetX(x);
            SetY(y);
        }

        public void SetX(float x)
        {
            var deltaX = -x - Left;
            Left += deltaX;
            Right -= deltaX;
        }

        public void SetY(float y)
        {
            var deltaY = -y - Top;
            Top += deltaY;
            Bottom -= deltaY;
        }

        public override string ToString()
        {
            return $"({Top}, {Bottom}, {Left}, {Right})";
        }
    }
}