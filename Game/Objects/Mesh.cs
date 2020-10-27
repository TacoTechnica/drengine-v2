using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class Mesh<VType> where VType : struct, IVertexType
    {
        private GamePlus _game;

        public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;

        public VertexBuffer VertexBuffer { get; private set; } = null;

        private VType[] _vertices = new VType[0];
        public VType[] Vertices
        {
            get => _vertices;
            set
            {
                _vertices = value;
                if (VertexBuffer == null)
                {
                    VertexBuffer = new VertexBuffer(_game.GraphicsDevice, typeof(VType),
                        Vertices.Length, BufferUsage.WriteOnly);
                }
                VertexBuffer.SetData<VType>(Vertices);
            }
        }

        public Mesh(GamePlus game)
        {
            _game = game;
        }
    }
}
