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
        private Vector3 _target;
        private Vector3 _position;

        // Camera projection matrix
        private Matrix _projectionMat = Matrix.Identity;

        // Camera view matrix
        private Matrix _viewMat;

        private float _fov = 90;

        // For use in GamePlus
        private LinkedListNode<Camera3D> _camListReference = null;

        // TODO: Remove test code?
        private bool _orbit = true;

        public Matrix ProjectionMatrix => _projectionMat;
        public Matrix ViewMatrix => _viewMat;

        public Camera3D(GamePlus game, Vector3 pos, Vector3 target) : base(game)
        {
            _position = pos;
            _target = target;

        }

        public Camera3D(GamePlus game) : this(game, Vector3.Forward * 100, Vector3.Zero) { }

        public override void Start()
        {
            _projectionMat = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(_fov),
                _game.GraphicsDevice.Viewport.AspectRatio,
                1f, 1000f
            );

            _viewMat = Matrix.CreateLookAt(_position, _target, Vector3.Up);

            _camListReference = _game.AddInternalCamera(this);
        }

        public override void Update(float dt)
        {
            int inX = (Keyboard.GetState().IsKeyDown(Keys.Right) ? 1 :
                0) - (Keyboard.GetState().IsKeyDown(Keys.Left) ? 1 : 0);
            int inY = (Keyboard.GetState().IsKeyDown(Keys.Down) ? 1 :
                0) - (Keyboard.GetState().IsKeyDown(Keys.Up) ? 1 : 0);
            int inZ = (Keyboard.GetState().IsKeyDown(Keys.OemPlus) ? 1 : 0) -
                      (Keyboard.GetState().IsKeyDown(Keys.OemMinus) ? 1 : 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _orbit = !_orbit;
            }

            Vector3 moveInput = 30f * new Vector3(inX, inY, inZ);

            _position += moveInput * dt;
            _target += moveInput * dt;

            // Rotate camera
            if (_orbit)
            {
                // Rotate a bit around the origin
                Matrix rotationMat = Matrix.CreateRotationY(MathHelper.ToRadians(60) * dt);
                _position = Vector3.Transform(_position, rotationMat);
            }

            // Update matrices
            _viewMat = Matrix.CreateLookAt(_position, _target, Vector3.Up);
        }

        public override void OnDestroy()
        {
            Debug.Log("Camera DESTROY!");
            _game.RemoveInternalCamera(_camListReference);
        }
    }
}
