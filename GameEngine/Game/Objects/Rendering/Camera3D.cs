using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.Objects.Rendering
{
    /// <summary>
    ///     TODO:
    ///     - Add a simple "layer" system where you can create cameras that render layers
    ///     - Whenever you add a GameObjectRender, it can have a layer. Thus it subscribes to be rendered
    ///     - by all of the cameras in those layers.
    /// </summary>
    public class Camera3D : GameObjectRender
    {
        // For use in GamePlus
        private ObjectContainerNode<Camera3D> _camListReference;

        private float _fov;

        private bool _needToUpdateProjection = true;

        // Camera projection matrix

        // Camera view matrix
        public Vector3 Position;
        public Quaternion Rotation;

        public Camera3D(GamePlus game, Vector3 pos, Quaternion rotation, float fov = 90f) : base(game)
        {
            Position = pos;
            Rotation = rotation;
            Fov = fov;

            _camListReference = Game.SceneManager.Cameras.Add(this);
        }

        public Camera3D(GamePlus game, Vector3 pos = default) : this(game, pos, Math.FromEuler(0, 0, 0))
        {
        }

        public float Fov
        {
            get => _fov;
            set
            {
                if (System.Math.Abs(value - _fov) > 0.1f) _needToUpdateProjection = true;
                _fov = value;
            }
        }

        public Matrix ProjectionMatrix { get; private set; } = Matrix.Identity;

        public Matrix ViewMatrix { get; private set; }

        public Vector2 WorldCoordToScreenCoord(Vector3 worldCoord)
        {
            var screen =
                Game.GraphicsDevice.Viewport.Project(worldCoord, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            return new Vector2(screen.X, screen.Y);
        }

        public Vector3 ScreenCoordToWorldCoord(Vector2 screenCord, float distanceFromCamera)
        {
            var screenPos = new Vector3(screenCord.X, screenCord.Y, distanceFromCamera);
            return Game.GraphicsDevice.Viewport.Unproject(screenPos, ProjectionMatrix, ViewMatrix, Matrix.Identity);
        }

        public Ray GetScreenRay(Vector2 screenPos)
        {
            var delta = ScreenCoordToWorldCoord(screenPos, 1) - Position;
            delta.Normalize();
            return new Ray(Position, delta);
        }

        public float GetFlatDistanceTo(Vector3 pos)
        {
            Vector3 relative = Vector3.Transform(pos, ViewMatrix);
            return -1 * relative.Z;
        }

        public override void Start()
        {
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
                ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(Fov),
                    Game.GraphicsDevice.Viewport.AspectRatio,
                    1f, 1000f
                );
                _needToUpdateProjection = false;
            }

            // TODO: Use a custom matrix so we don't do this math every damn frame,
            //     instead just using the rotation directly.
            var target = Position + Math.RotateVector(Vector3.Forward, Rotation);
            var up = Math.RotateVector(Vector3.Up, Rotation);
            ViewMatrix = Matrix.CreateLookAt(Position, target, up);
        }

        public override void OnDestroy()
        {
            Debug.Log("deleted CAMERA");
            _camListReference = Game.SceneManager.Cameras.RemoveImmediate(_camListReference);
        }

        internal override void RunOnDisable(ObjectContainerNode<GameObject> newNode)
        {
            _camListReference = Game.SceneManager.Cameras.DisableImmediate(_camListReference);
            base.RunOnDisable(newNode);
        }

        internal override void RunOnEnable(ObjectContainerNode<GameObject> newNode)
        {
            _camListReference = Game.SceneManager.Cameras.EnableImmediate(_camListReference);
            base.RunOnEnable(newNode);
        }

        public override void Draw(GraphicsDevice g)
        {
            // If we're in debug mode, render colliders from this camera.
            if (Game.DebugDrawColliders) Game.CollisionManager.DrawDebugColliders(Game, this);
        }
    }
}