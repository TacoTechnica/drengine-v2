using System.Collections.Generic;
using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Objects.Rendering;
using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.CoreScenes.SceneEditor
{
    public class TransformTranslator : GameObjectRender3D
    {

        private TransformArrow _xArrow,
            _yArrow,
            _zArrow;

        public TransformTranslator(GamePlus game, Vector3 position) : base(game, position, Quaternion.Identity)
        {
            _xArrow = new TransformArrow(game, position, Math.FromEuler(0, 90, 0), Color.Red);
            _yArrow = new TransformArrow(game, position, Math.FromEuler(-90, 0, 0), Color.Green);
            _zArrow = new TransformArrow(game, position, Math.FromEuler(0, 0, 0), Color.Blue);
            AddChild(_xArrow);
            AddChild(_yArrow);
            AddChild(_zArrow);
            //_xArrow.SetActive(false);
            //_yArrow.SetActive(false);
            //_zArrow.SetActive(false);
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            float distanceToCam = (cam.Position - Transform.Position).Length();

            Vector3 scale = Vector3.One * distanceToCam * 0.05f;

            _xArrow.Transform.Scale = scale;
            _yArrow.Transform.Scale = scale;
            _zArrow.Transform.Scale = scale;

            _xArrow.Transform.Position = Transform.Position;
            _yArrow.Transform.Position = Transform.Position;
            _zArrow.Transform.Position = Transform.Position;
            /*
            _xArrow.Draw(cam, g, transform);
            _yArrow.Draw(cam, g, transform);
            _zArrow.Draw(cam, g, transform);
            */
            // Nothing, we have our children.
        }

        class TransformArrow : SimpleMeshRenderer<VertexPositionColor>
        {
            private static int Detail = 15;
            private static float PoleRadius = 0.2f;
            private static float PoleHeight = 5;
            private static float TipRadius = 1;
            private static float TipHeight = 1.5f;

            private static Vector3[] Verts = null;

            public TransformArrow(GamePlus game, Vector3 position, Quaternion rotation, Color color) : base(game, position, rotation)
            {
                PrimitiveType = PrimitiveType.TriangleList;
                this.IgnoreDepth = true;

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
                    v[i] = new VertexPositionColor(Verts[i], color);
                }

                Vertices = v;
            }
        }
    }
}