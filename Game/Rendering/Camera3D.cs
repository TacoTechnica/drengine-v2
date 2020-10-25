using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        private float _fov = 0;

        private bool _needToUpdateProjection = true;

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
        private ObjectContainerNode<Camera3D> _camListReference = null;

        public Matrix ProjectionMatrix => _projectionMat;
        public Matrix ViewMatrix => _viewMat;

        public Camera3D(GamePlus game, Vector3 pos, Quaternion rotation, float fov=90f) : base(game)
        {
            Position = pos;
            Rotation = rotation;
            Fov = fov;

            Debug.Log("ADDED CAMERA");
            _camListReference = _game.SceneManager.Cameras.Add(this);
        }

        public Camera3D(GamePlus game, Vector3 pos = default(Vector3)) : this(game, pos, Math.FromEuler(0,0,0), 90f) { }

        public Vector2 WorldCoordToScreenCoord(Vector3 worldCoord)
        {
            Vector3 screen =
                _game.GraphicsDevice.Viewport.Project(worldCoord, _projectionMat, _viewMat, Matrix.Identity);
            return new Vector2(screen.X, screen.Y);
        }

        public Vector3 ScreenCoordToWorldCoord(Vector2 screenCord, float distanceFromCamera)
        {
            Vector3 screenPos = new Vector3(screenCord.X, screenCord.Y, distanceFromCamera);
            return _game.GraphicsDevice.Viewport.Unproject(screenPos, _projectionMat, _viewMat, Matrix.Identity);
        }

        public Ray GetScreenRay(Vector2 screenPos)
        {
            Vector3 delta = (ScreenCoordToWorldCoord(screenPos, 1) - Position);
            delta.Normalize();
            return new Ray(Position, delta);
        }

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
        internal override void RunOnDisable(ObjectContainerNode<GameObject> newNode)
        {
            _camListReference = _game.SceneManager.Cameras.DisableImmediate(_camListReference);
            base.RunOnDisable(newNode);
        }

        internal override void RunOnEnable(ObjectContainerNode<GameObject> newNode)
        {
            _camListReference = _game.SceneManager.Cameras.EnableImmediate(_camListReference);
            base.RunOnEnable(newNode);
        }

        public override void Draw(GraphicsDevice g)
        {
            // If we're in debug mode, render colliders from this camera.
            if (_game.DebugDrawColliders)
            {
                _game.CollisionManager.DrawDebugColliders(_game, this);
            }
        }
    }
}
