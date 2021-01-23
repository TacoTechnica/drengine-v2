using System;
using System.Collections.Generic;
using GameEngine.Game;
using GameEngine.Game.Collision;
using GameEngine.Game.Input;
using GameEngine.Game.Objects;
using GameEngine.Game.Objects.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Math = GameEngine.Util.Math;

namespace DREngine.Game.CoreScenes.SceneEditor
{
    public class TransformTranslator : GameObjectRender3D
    {

        public readonly TransformArrow XArrow;
        public readonly TransformArrow YArrow;
        public readonly TransformArrow ZArrow;

        public GameObjectRender3D Target;

        public bool Selected => XArrow.Selected || YArrow.Selected || ZArrow.Selected;

        public TransformTranslator(GamePlus game, Vector3 position) : base(game, position, Quaternion.Identity)
        {
            XArrow = new TransformArrow(game, position, Math.FromEuler(0, 90, 0), Color.DarkRed, Color.Multiply(Color.Red, 2.0f));
            YArrow = new TransformArrow(game, position, Math.FromEuler(-90, 0, 0), Color.DarkGreen, Color.LimeGreen);
            ZArrow = new TransformArrow(game, position, Math.FromEuler(0, 0, 0), Color.DarkBlue, Color.SkyBlue);
            AddChild(XArrow);
            AddChild(YArrow);
            AddChild(ZArrow);

            XArrow.SetActive(false);
            YArrow.SetActive(false);
            ZArrow.SetActive(false);
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            // Only post draw.
        }

        public override void PostDraw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {

            if (Target == null) return;

            Transform.Position = Target.Transform.Position;

            float distanceToCam = cam.GetFlatDistanceTo(Transform.Position);

            Vector3 scale = Vector3.One * distanceToCam * 0.05f;

            Vector2 mousePos = RawInput.GetMousePosition();
            bool clicking = RawInput.MousePressing(MouseButton.Left);

            Run(XArrow, YArrow, ZArrow);

            void Run(params TransformArrow[] arrows)
            {
                foreach (TransformArrow arrow in arrows)
                {
                    arrow.Draw(cam, g, arrow.Transform);
                    arrow.UpdateTransformAndSelect(Transform.Position, scale, cam);
                    arrow.UpdateSelected(cam, mousePos, clicking);
                    arrow.UpdateDrag(cam, mousePos, clicking);
                    //arrow.DrawDebugAxis(cam, mousePos);
                }
            }
        }

        public class TransformArrow : SimpleMeshRenderer<VertexPositionColor>
        {

            public Action<Vector3> Dragged;
            
            private static int Detail = 15;
            private static float PoleRadius = 0.2f;
            private static float PoleHeight = 5;
            private static float TipRadius = 1;
            private static float TipHeight = 1.5f;

            private static Vector3[] Verts = null;

            private BoxCollider _collider;

            private Color _regularColor, _selectColor;
            public bool Selected { get; private set; }= false;
            private bool _prevClicking = false;

            private Vector3 _dragPrev;
            private GamePlus _game;

            public TransformArrow(GamePlus game, Vector3 position, Quaternion rotation, Color color, Color selectColor) : base(game, position, rotation)
            {
                _game = game;

                PrimitiveType = PrimitiveType.TriangleList;
                this.IgnoreDepth = true;

                _regularColor = color;
                _selectColor = selectColor;

                _collider = new BoxCollider(this, Vector3.Zero, Vector3.Zero);

                if (Verts == null) {
                    // 6 triangles per detail.
                    List<Vector3> verts = new List<Vector3>(6 * Detail * 3);

                    float angleDelta = (float) Math.PI * 2 / (float) Detail;
                    
                    for (int i = 0; i < Detail; ++i)
                    {
                        float angle = (float)Math.PI * 2 * i / (float) Detail;
                        // Bottom
                        double pole0X = PoleRadius * Math.Cos(angle),
                            pole0Y = PoleRadius * Math.Sin(angle),
                            pole1X = PoleRadius * Math.Cos(angle + angleDelta),
                            pole1Y = PoleRadius * Math.Sin(angle + angleDelta);
                        double top0X = TipRadius * Math.Cos(angle),
                            top0Y = TipRadius * Math.Sin(angle),
                            top1X = TipRadius * Math.Cos(angle + angleDelta),
                            top1Y = TipRadius * Math.Sin(angle + angleDelta);
                        // Bottom
                        T(
                            pole0X, pole0Y, 0,
                            0, 0, 0,
                            pole1X, pole1Y, 0
                        );
                        // Pole Height
                        T(
                            pole0X, pole0Y, PoleHeight,
                            pole0X, pole0Y, 0,
                            pole1X, pole1Y, 0
                        );
                        T(
                            pole0X, pole0Y, PoleHeight,
                            pole1X, pole1Y, 0,
                            pole1X, pole1Y, PoleHeight
                        );

                        // Tip Flat
                        T(
                            top0X, top0Y, PoleHeight,
                            pole0X, pole0Y, PoleHeight,
                            pole1X, pole1Y, PoleHeight
                        );
                        T(
                            top0X, top0Y, PoleHeight,
                            pole1X, pole1Y, PoleHeight,
                            top1X, top1Y, PoleHeight
                        );
                        // Tip Point
                        T(
                            0, 0, PoleHeight + TipHeight,
                            top0X, top0Y, PoleHeight,
                            top1X, top1Y, PoleHeight
                        );
                    }

                    Verts = verts.ToArray();

                    void T(double x0, double y0, double z0, double x1, double y1, double z1, double x2, double y2,
                        double z2)
                    {
                        verts.Add(new Vector3((float)x0, (float)y0, (float)z0));
                        verts.Add(new Vector3((float)x1, (float)y1, (float)z1));
                        verts.Add(new Vector3((float)x2, (float)y2, (float)z2));
                    }
                }

                var v = new VertexPositionColor[Verts.Length];
                for (int i = 0; i < Verts.Length; ++i)
                {
                    v[i] = new VertexPositionColor(Verts[i], _regularColor);
                }

                Vertices = v;
            }

            public void UpdateTransformAndSelect(Vector3 position, Vector3 scale, Camera3D cam)
            {
                Transform.Position = position;
                Transform.Scale = scale;

                Vector3 center = Vector3.Backward * (PoleHeight + TipHeight / 2);

                _collider.Min = center - Vector3.One;
                _collider.Max = center + Vector3.One;
            }

            public void UpdateSelected(Camera3D cam, Vector2 mousePos, bool clicking)
            {
                bool selected = _collider.ContainsScreen(cam, mousePos);
                //  Stay selected on drag
                if (clicking)
                {
                    if (!Selected) selected = false;
                    if (!selected && Selected) selected = true;
                }
                if (Selected == selected) return;
                Selected = selected;

                Color target = selected ? _selectColor : _regularColor;

                var v = new VertexPositionColor[Verts.Length];
                for (int i = 0; i < Verts.Length; ++i)
                {
                    v[i] = new VertexPositionColor(Verts[i], target);
                }

                Vertices = v;
            }

            public void UpdateDrag(Camera3D cam, Vector2 mousePos, bool clicking)
            {
                if (Selected && clicking)
                {
                    Vector3 drag = GetClosestMouseAxis(cam, mousePos);
                    if (!_prevClicking)
                    {
                        _dragPrev = drag;
                    }
                    Vector3 dragDelta = drag - _dragPrev;
                    Dragged.Invoke(dragDelta);
                    _dragPrev = drag;
                }

                _prevClicking = clicking;
            }

            /*
            public void DrawDebugAxis(Camera3D cam, Vector2 mousePos)
            {
                Vector3 closest = GetClosestMouseAxis(cam, mousePos);
                DebugDrawer.DrawGizmo(_game, cam, closest);
            }
            */

            private Vector3 GetClosestMouseAxis(Camera3D cam, Vector2 mousePos)
            {
                Ray r = cam.GetScreenRay(mousePos);
                Vector3 mouseOrigin = r.Position;
                Vector3 mouseAxis = r.Direction;

                Vector3 origin = Transform.Position;
                Vector3 axis = -1 * Math.RotateVector(Vector3.Forward, Transform.Rotation);

                return GetClosestPointOnSecondAxis(mouseOrigin, mouseAxis, origin, axis);
            }

            /// <summary>
            /// Dang math is annoying.
            /// </summary>
            /// <param name="p1"> Origin of first line </param>
            /// <param name="v1"> Axis/Direction of first line </param>
            /// <param name="p2"> Origin of second line </param>
            /// <param name="v2"> Axis/Direction of second line </param>
            /// <returns> The closest point on the second line to the first line </returns>
            private static Vector3 GetClosestPointOnSecondAxis(Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2)
            {
                Vector3 deltaPos = p2 - p1;
                // Math from notebook.
                float
                    A = 2f * v1.LengthSquared(),
                    B = 2f * Vector3.Dot(v1, v2),
                    C = Vector3.Dot(v1, -1 * deltaPos),
                    D = 2f * v2.LengthSquared(),
                    // B again
                    E = Vector3.Dot(v2, deltaPos);

                float t2 = (((-2 * B * C) / A) - 2 * E) / (D - (B * B / A));

                return p2 + v2 * t2;
            }
        }
    }
}
