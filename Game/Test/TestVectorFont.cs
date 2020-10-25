
using System;
using System.Collections.Generic;
using System.Linq;
using DREngine.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poly2Tri;
using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay;
using Poly2Tri.Triangulation.Polygon;
using Poly2Tri.Utility;
using SharpFont;

namespace DREngine.Game
{
    public class TestVectorFont : IGameRunner
    {

        private GamePlus _game;

        private List<List<PolygonPoint>> points = new List<List<PolygonPoint>>();

        public List<PolygonPoint> lastPList
        {
            get
            {
                if (points.Count == 0) points.Add(new List<PolygonPoint>());
                return points[points.Count - 1];
            }
        }
        public Vector2 lastPoint
        {
            get
            {
                if (points.Count == 0 || lastPList.Count == 0) return Vector2.Zero;
                List<PolygonPoint> plist = points[points.Count - 1];
                PolygonPoint p = plist[plist.Count - 1];
                return new Vector2(p.Xf, p.Yf);
            }
        }

        // TODO: Be more clever here
        private int bezierDivisions = 2;

        private PolyMeshRenderer obj;

        private Camera3D _cam;


        public void Initialize(GamePlus game)
        {
            _game = game;

            Library library = new Library();

            Face f = new Face(library, new EnginePath("default_resources/Fonts/SourceSansPro/SourceSansPro-Bold.ttf"));

            f.SetPixelSizes(32, 0);

            uint g;

            f.LoadChar('&', LoadFlags.Default, LoadTarget.Normal);

            points.Add(new List<PolygonPoint>());

            OutlineFuncs outlineFuncs = new OutlineFuncs(MoveTo, LineTo, ConicTo, CubicTo, 0, 0);

            f.Glyph.Outline.Decompose(outlineFuncs,IntPtr.Zero );


            Polygon poly = null;
            foreach(List<PolygonPoint> plist in points) {
                Debug.Log($"PSIZE: {plist.Count}");
                if (plist.Count < 3) continue;

                if (poly == null)
                {
                    poly = new Polygon(plist);
                }
                else
                {
                    Polygon cutPoly = new Polygon(plist);
                    P2T.Triangulate(cutPoly);
                    if (poly.Bounds.Intersects(cutPoly.Bounds))
                    {
                        poly.AddHole(cutPoly);
                    }
                    else
                    {
                        poly.AddRange(cutPoly);
                    }
                }

            }
            P2T.Triangulate(poly);

            obj = new PolyMeshRenderer(game, poly, Vector3.Zero, Quaternion.Identity);
            obj.Transform.Scale = Vector3.One * 1000;

            _cam = new Camera3D(game, Vector3.Backward * 40);
        }

        private int CubicTo(ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr user)
        {
            Debug.Log("OK CUBIC");
            //lastPList.Add(new PolygonPoint(control1.X, control1.Y));
            //lastPList.Add(new PolygonPoint(control2.X, control2.Y));
            lastPList.Add(new PolygonPoint(to.X, to.Y));

            return 0;
        }

        private int ConicTo(ref FTVector control, ref FTVector to, IntPtr user)
        {

            Vector2 start = lastPoint,
                    mid = new Vector2( (float)control.X, (float)control.Y),
                    end = new Vector2((float)to.X, (float)to.Y);
            for (int i = 0; i < bezierDivisions; ++i)
            {
                float prog = (float) i / bezierDivisions;
                Vector2 leftProg = start + (prog) * (mid - start);
                Vector2 rightProg = mid + (prog) * (end - mid);
                Vector2 pos = (1 - prog) * leftProg + prog * rightProg;
                lastPList.Add(new PolygonPoint(pos.X, pos.Y));
            }
            lastPList.Add(new PolygonPoint(to.X, to.Y));

            Debug.Log("OK CONIC");
            return 0;
        }

        private int LineTo(ref FTVector to, IntPtr user)
        {
            PolygonPoint next = new PolygonPoint(to.X, to.Y);
            lastPList.Add(next);
            return 0;
        }

        private int MoveTo(ref FTVector to, IntPtr user)
        {
            //Debug.Log($"MOVE: {to.X}, {to.Y}");
            // TODO: Figure out what this means.
            points.Add(new List<PolygonPoint>());
            LineTo(ref to, user);
            return 0;
        }


        public void Update(float deltaTime)
        {
            Vector3 r = Math.ToEuler(obj.Transform.Rotation);
            if (RawInput.KeyPressing(Keys.Right))
            {
                r.Y += 120 * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Left))
            {
                r.Y -= 120 * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Up))
            {
                r.X += 120 * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Down))
            {
                r.X -= 120 * deltaTime;
            }
            obj.Transform.Rotation = Math.FromEuler(r);
        }

        public void Draw()
        {

            float SCALE = 1000;

            foreach (List<PolygonPoint> plist in points)
            {
                PolygonPoint prev = null;
                foreach (PolygonPoint p in plist)
                {
                    if (prev == null)
                    {
                        prev = p;
                        continue;
                    }
                    Vector3 prevV = new Vector3(prev.Xf, prev.Yf, 0) * SCALE,
                            nextV = new Vector3(p.Xf, p.Yf, 0) * SCALE;
                    DebugDrawer.DrawLine3D(_game, _cam, prevV, nextV, Color.Red, Color.Red);

                    prev = p;
                }
            }
        }


        class PolyMeshRenderer : SimpleMeshRenderer<VertexPositionColor>
        {
            public PolyMeshRenderer(GamePlus game, Polygon poly, Vector3 position, Quaternion rotation) : base(game, position, rotation)
            {
                CullingEnabled = false;

                List<VertexPositionColor> verts = new List<VertexPositionColor>();
                foreach (DelaunayTriangle t in poly.Triangles)
                {
                    Debug.Log("FACE");
                    foreach (TriangulationPoint p in t.Points)
                    {
                        Vector3 v = new Vector3(p.Xf, p.Yf, 0);
                        Debug.Log($"    {v} ");
                        verts.Add(new VertexPositionColor(v, Color.Green));
                    }
                }

                Vertices = verts.ToArray();
            }
        }
    }
}
