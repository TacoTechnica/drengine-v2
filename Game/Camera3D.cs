using System.Collections.Generic;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Viewport = Gtk.Viewport;

namespace DREngine.Game
{
    /// <summary>
    /// TODO:
    ///     - Use rotation direction instead of target
    ///     - Add camera controls (set fov, position and rotation)
    ///
    ///
    ///     - Add a simple "layer" system where you can create cameras that render layers
    ///     - Whenever you add a GameObjectRender, it can have a layer. Thus it subscribes to be rendered
    ///     - by all of the cameras in those layers.
    /// </summary>
    public class Camera3D : GameObject
    {
        public Vector3 Position;
        public Quaternion Rotation;

        // Camera projection matrix
        private Matrix _projectionMat = Matrix.Identity;

        // Camera view matrix
        private Matrix _viewMat;

        private float _fov = 90;

        // For use in GamePlus
        private LinkedListNode<Camera3D> _camListReference = null;

        public Matrix ProjectionMatrix => _projectionMat;
        public Matrix ViewMatrix => _viewMat;

        public Camera3D(GamePlus game, Vector3 pos, Quaternion rotation) : base(game)
        {
            Position = pos;
            Rotation = rotation;
        }

        public Camera3D(GamePlus game) : this(game, Vector3.Zero, Quaternion.Identity) { }

        public override void Start()
        {
            _projectionMat = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(_fov),
                _game.GraphicsDevice.Viewport.AspectRatio,
                1f, 1000f
            );

            _camListReference = _game.SceneManager.Cameras.Add(this);
        }

        public override void Update(float dt)
        {
            // TODO: Use a custom matrix so we don't do this math every damn frame,
            //     instead just using the rotation directly.
            Vector3 target = Position + Math.RotateVector(Vector3.Forward, Rotation);
            Vector3 up = Math.RotateVector(Vector3.Up, Rotation);
            _viewMat = Matrix.CreateLookAt(Position, target, up);

        }

        public override void OnDestroy()
        {
            Debug.Log("Camera DESTROY!");
            _camListReference = _game.SceneManager.Cameras.RemoveImmediate(_camListReference);
        }
    }
}
