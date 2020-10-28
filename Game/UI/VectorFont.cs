using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poly2Tri;
using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay;
using Poly2Tri.Triangulation.Polygon;
using Poly2Tri.Utility;
using SharpFont;

namespace DREngine.Game.UI
{

    public class VectorFont
    {
        private static Library _library = null;

        private static Library Library
        {
            get
            {
                if (_library == null) _library = new Library();
                return _library;
            }
        }

        private GamePlus _game;

        private Dictionary<char, VectorCharacterModel> _charMap = new Dictionary<char, VectorCharacterModel>();

        private Face _face;

        private int _bezierAccuracy = 1;

        // It do be like that
        // 32: We pass that in SetPixelSize
        // 96: Windows standard...
        // 72: It is how it is.
        public float BullshitScale = 1f / (96*32f*64);//18 * 96f / 72f;

        public VectorFont(GamePlus game, Path path)
        {
            _game = game;
            LoadFont(path);
        }

        public void LoadFont(Path path, int bezierAccuracy=2)
        {
            _bezierAccuracy = bezierAccuracy;
            // TODO: Maybe unload each model to avoid GC collection
            _charMap.Clear();
            // TODO: Load data from file

            _face = new Face(Library, path);
            //_face.SetPixelSizes(32, 0);
            _face.SetCharSize(0, 32*64, 96, 96);

            _charMap['|'] = GetCharacterModel('|');
            VectorCharacterModel m = _charMap['|'];

            // TODO: Load a base set of characters (a-z, A-Z, !?$./':"\}{
        }

        public float GetKerningBetween(char left, char right)
        {
            return (float)_face.GetKerning(_face.GetCharIndex(left), _face.GetCharIndex(right), KerningMode.Default).X;
        }

        private void LoadChar(char c)
        {
            _face.LoadChar(c, LoadFlags.NoBitmap, LoadTarget.Normal);

            List<PolygonPointList> list = new List<PolygonPointList>(1);
            list.Add(new PolygonPointList(this));

            OutlineFuncs outlineFuncs = new OutlineFuncs(
                (ref FTVector to, IntPtr user) =>
                {
                    // Move To
                    list.Add(new PolygonPointList(this));
                    list.Last().Add(to);
                    return 0;
                },
                (ref FTVector to, IntPtr user) =>
                {
                    // Line To
                    list.Last().Add(to);
                    return 0;
                },
                (ref FTVector control, ref FTVector to, IntPtr user) =>
                {
                    PolygonPointList last = list.Last();
                    // Conic To (Bezier 1)
                    Vector2 start = last.LastPointOrZero(),
                        mid = new Vector2( (float)control.X.Value, (float)control.Y.Value),
                        end = new Vector2((float)to.X.Value, (float)to.Y.Value);
                    for (int i = 0; i < _bezierAccuracy; ++i)
                    {
                        float prog = (float) i / _bezierAccuracy;
                        Vector2 leftProg = start + (prog) * (mid - start);
                        Vector2 rightProg = mid + (prog) * (end - mid);
                        Vector2 pos = (1 - prog) * leftProg + prog * rightProg;
                        last.Add(pos);
                    }
                    last.Add(to);

                    return 0;
                },
                (ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr user) =>
                {
                    // Cubic To (Bezier 2?)
                    // TODO: Do the Ebic Cubic nay nay
                    Debug.Log("Bezier??");
                    list.Last().Add(to);

                    return 0;
                },
                0, 0
            );


            _face.Glyph.Outline.Decompose(outlineFuncs,IntPtr.Zero);

            List<Polygon> polygons = new List<Polygon>();

            PolygonPointList lastSolid = null;

            foreach(PolygonPointList plist in list) {
                Debug.Log($"PSIZE: {plist.Count}");
                if (plist.Count < 3) continue;


                if (polygons.Count == 0)
                {
                    polygons.Add(new Polygon(plist.Points));

                    lastSolid = plist;
                }
                else
                {
                    Polygon poly = polygons.Last();

                    if (plist.Intersects(lastSolid))
                    {
                        // We are inside the prev polygon, assume we're cutting
                        Polygon cutPoly = new Polygon(plist.Points);
                        Debug.Log("HOLE");
                        P2T.Triangulate(cutPoly);
                        poly.AddHole(cutPoly);
                    }
                    else
                    {
                        // We are Outside so don't cut.
                        Debug.Log("ADD");
                        // Generate prev last polygon and make a new one
                        P2T.Triangulate(polygons.Last());
                        polygons.Add(new Polygon(plist.Points));
                        //poly.AddRange(new Polygon(plist.Points));
                        lastSolid = plist;
                    }
                }
            }
            P2T.Triangulate(polygons.Last());

            // We now have our triangulated polygon lists, and we can create our model
            _charMap[c] = new VectorCharacterModel(_game, polygons);
        }

        public VectorCharacterModel GetCharacterModel(char c)
        {
            if (!_charMap.ContainsKey(c))
            {
                LoadChar(c);
            }
            return _charMap[c];
        }

        public Vector2 GetSize(string text)
        {
            // TODO: Use kerning, text width and height to calculate
            return Vector2.Zero;
        }

        public void DrawString(GraphicsDevice g, EffectPass pass, IEffectMatrices effect, float size, string text, bool flip = false)
        {
            Matrix rootWorld = effect.World;
            Vector2 p = Vector2.Zero;
            char prev = '\0';
            float prevDX = 0;
            float height = size;//0.955f * (float) _face.Height / (32f * 72f * 16f * 1f);//_face.Size.Metrics.Height.Value / (96f * 32f);
            float newlinePad = 0f;//0.009f; // TODO: What unit?
            float xScale = 1f;//TestVectorTextUI.TEMP_TEST;
            float arbitraryOffset = 0f;//TestVectorTextUI.TEMP_TEST2;
            /*
             * 0.9550006, 0.009199999
             *
             * This kind of works. Kind of.
             * xscale = 0.8830015, arbitraryOffset = 12.27184
             */
            foreach (char c in text)
            {
                _face.LoadChar(c, LoadFlags.NoBitmap, LoadTarget.Normal);

                switch (c)
                {
                    case ' ':
                    case '\t': // TODO: Tab properly?
                        float spaceWidth = (float)_face.Glyph.Metrics.HorizontalAdvance;
                        p.X +=  arbitraryOffset + xScale * spaceWidth;
                        break;
                    case '\n':
                        p.X = 0;
                        p.Y += height + newlinePad;
                        break;
                    // TODO: Other potential formatting characters?
                    default:
                        VectorCharacterModel m = GetCharacterModel(c);

                        // Apply kerning.

                        // Translate with the effect.
                        // 0.884 and 0.05 were found from experimentation. This is bullshit.
                        float scale = BullshitScale * (size + size*_face.Descender / _face.UnitsPerEM);
                        Matrix scaleMat = Matrix.CreateScale(scale, flip? -scale : scale, 1);
                        if (flip)
                        {
                            float dyh = size * (_face.Height / (64f*32f)) + size * _face.Descender / _face.UnitsPerEM; //height - (float)_face.Glyph.Metrics.HorizontalBearingY / 64f;
                            scaleMat = scaleMat * Matrix.CreateTranslation(0, dyh, 0);
                        }
                        effect.World = scaleMat * Matrix.CreateTranslation(p.X, p.Y, 0) * rootWorld;
                        pass.Apply();
                        // Render each text part
                        foreach (var mesh in m.Parts)
                        {
                            g.DrawUserPrimitives(mesh.PrimitiveType, mesh.Vertices, 0, mesh.Vertices.Length / 3);
                        }

                        if (_face.HasKerning)
                        {
                            float kerning = GetKerningBetween(prev, c);
                            Debug.Log($"K: {prev} => {c} = {kerning}");
                            p.X += kerning; //GetKerningBetween(prev, c) * BullshitScale;// + dx;
                        }
                        else
                        {
                            float toMove = (float)_face.Glyph.Metrics.HorizontalAdvance - (float)_face.Glyph.Metrics.HorizontalBearingX;
                            p.X += arbitraryOffset + xScale * toMove / (16f * 72f); //(96f / 72f) * size * toMove / BullshitScale; //size * dx / (BullshitScale);
                        }
                        break;
                }

                prev = c;
            }
        }
    }

    /// <summary>
    ///     Utility list for hanging on to polygon points.
    /// </summary>
    class PolygonPointList
    {
        public List<PolygonPoint> Points { get; private set; }= new List<PolygonPoint>();

        public float MinX { get; private set; } = float.PositiveInfinity;
        public float MinY { get; private set; } = float.PositiveInfinity;
        public float MaxX { get; private set; } = float.NegativeInfinity;
        public float MaxY { get; private set; } = float.NegativeInfinity;

        private VectorFont _baseParent;

        public PolygonPointList(VectorFont parent)
        {
            _baseParent = parent;
        }

        public void Add(PolygonPoint a)
        {
            //a.X *= _baseParent._bullshitScale;
            //a.Y *= _baseParent._bullshitScale;
            if (a.Xf < MinX)
                MinX = a.Xf;
            else if (a.Xf > MaxX)
                MaxX = a.Xf;
            if (a.Yf < MinY)
                MinY = a.Yf;
            else if (a.Yf > MaxY)
                MaxY = a.Yf;
            Points.Add(a);
        }

        public void Add(FTVector v)
        {
            Add(new PolygonPoint(v.X.Value, v.Y.Value));
        }

        public void Add(Vector2 p)
        {
            Add(new PolygonPoint(p.X, p.Y));
        }

        public Vector2 LastPointOrZero()
        {
            if (Count == 0)
            {
                return Vector2.Zero;
            }

            PolygonPoint p = Points.Last();
            return new Vector2(p.Xf, p.Yf);
        }

        public bool Intersects(PolygonPointList parent)
        {
            return MinX < parent.MaxX && MaxX > parent.MinX && MinY < parent.MaxY && MaxY > parent.MinY;
        }

        public bool Inscribes(PolygonPointList parent)
        {
            return MinX > parent.MinX && MaxX < parent.MaxX && MinY > parent.MinY && MaxY < parent.MaxY;
        }

        public int Count => Points.Count;
    }

    public class VectorCharacterModel
    {
        private List<Mesh<VertexPositionColor>> _parts = new List<Mesh<VertexPositionColor>>();

        public Rect2D Bounds { get; private set; }

        public VectorCharacterModel(GamePlus _game, IEnumerable<Polygon> polygons)
        {
            Bounds = new Rect2D();

            foreach (Polygon poly in polygons)
            {
                Bounds.AddPoint(new Point2D(poly.MinX, poly.MinY));
                Bounds.AddPoint(new Point2D(poly.MaxX, poly.MaxY));
                List<VertexPositionColor> verts = new List<VertexPositionColor>();
                foreach (DelaunayTriangle t in poly.Triangles)
                {
                    //Debug.Log("FACE");
                    foreach (TriangulationPoint p in t.Points)
                    {
                        Vector3 v = new Vector3(p.Xf, p.Yf, 0);
                        //Debug.Log($"    {v} ");
                        verts.Add(new VertexPositionColor(v, Color.Green));
                    }
                }

                Mesh<VertexPositionColor> m = new Mesh<VertexPositionColor>(_game)
                {
                    Vertices = verts.ToArray()
                };
                _parts.Add(m);
            }
        }

        public IEnumerable<Mesh<VertexPositionColor>> Parts => _parts;
    }
}
