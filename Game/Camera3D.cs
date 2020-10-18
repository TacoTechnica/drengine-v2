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
    ///     - Add a simple "layer" system where you can create cameras that render layers
    ///     - Whenever you add a GameObjectRender, it can have a layer. Thus it subscribes to be rendered
    ///     - by all of the cameras in those layers.
    /// </summary>
    public class Camera3D : GameObjectRender
    {
        public Vector3 Position;
        public Quaternion Rotation;

        // Camera projection matrix
        private Matrix _projectionMat = Matrix.Identity;

        // Camera view matrix
        private Matrix _viewMat;

        private bool _needToUpdateProjection = true;
        private float _fov = 0;
        public float Fov
        {
            get
            {
                return _fov;
            }
            set
            {
                if (value != _fov) _needToUpdateProjection = true;
                _fov = value;
            }
        }

        // For use in GamePlus
        private LinkedListNode<Camera3D> _camListReference = null;

        public Matrix ProjectionMatrix => _projectionMat;
        public Matrix ViewMatrix => _viewMat;

        public Camera3D(GamePlus game, Vector3 pos=default(Vector3), Quaternion rotation=default(Quaternion), float fov = 90) : base(game)
        {
            Position = pos;
            Rotation = rotation;
            Fov = fov;

            Debug.Log("ADDED CAMERA");
            _camListReference = _game.SceneManager.Cameras.Add(this);
        }

        public Camera3D(GamePlus game) : this(game, Vector3.Zero, Quaternion.Identity) { }

        public override void Start()
        {
            Debug.Log("Cam view projection SET");
            UpdateViewAndProjection();
        }

        public override void Update(float dt)
        {
            UpdateViewAndProjection();
        }

        private void UpdateViewAndProjection()
        {
            if (_needToUpdateProjection)
            {
                _projectionMat = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(Fov),
                    _game.GraphicsDevice.Viewport.AspectRatio,
                    1f, 1000f
                );
                _needToUpdateProjection = false;
            }
            // TODO: Use a custom matrix so we don't do this math every damn frame,
            //     instead just using the rotation directly.
            Vector3 target = Position + Math.RotateVector(Vector3.Forward, Rotation);
            Vector3 up = Math.RotateVector(Vector3.Up, Rotation);
            _viewMat = Matrix.CreateLookAt(Position, target, up);
        }

        public override void OnDestroy()
        {
            Debug.Log("deleted CAMERA");
            _camListReference = _game.SceneManager.Cameras.RemoveImmediate(_camListReference);
        }

        public override void Draw(GraphicsDevice g)
        {
            // NOTE: We are put here so that we can call start immediately.
        }
    }
}
