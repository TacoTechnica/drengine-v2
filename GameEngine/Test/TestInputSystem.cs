using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Objects.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    public class TestInputSystem : IGameTester
    {
        protected GamePlus Game;

        private ExampleControls _controls;

        private ExampleTriangleObject _triangle;

        public void Initialize(GamePlus game)
        {
            Game = game;
            _controls = new ExampleControls(Game);

            _controls.Explode.Pressed += OnExplodePressed;
            _controls.Read.Held += OnReadingHeld;
            _controls.Read.Released += OnReadingReleased;

            Game.CurrentCursor.SetOverride(_controls.Movement, 100);

            _triangle = new ExampleTriangleObject(Game, Vector3.Zero, Quaternion.Identity);
            new Camera3D(Game, Vector3.Backward * 20);

            Debug.Log("Input System Test #0: Starto!");
        }

        private void OnReadingReleased(InputActionButton obj)
        {
            Debug.Log("(Stopped Reading)");
        }

        private void OnReadingHeld(InputActionButton obj)
        {
            Debug.Log($"Value: {_controls.Movement.Value}, {_controls.Twiddler.Value}");
        }

        private void OnExplodePressed(InputActionButton obj)
        {
            _controls.Movement.Active = !_controls.Movement.Active;
            Debug.Log($"BOOM: Movement Active = {_controls.Movement.Active}");
        }

        public void Update(float deltaTime)
        {
            // Note that when inactive, the controls will be stuck!! We must check for this.
            if (_controls.Movement.Active)
            {
                // Move using the input system
                Vector2 movement = _controls.Movement.Value;
                _triangle.Transform.Position += 10 * new Vector3(movement.X, movement.Y, 0) * deltaTime;
            }

            if (_controls.Twiddler.Active)
            {
                // Rotate maybe as well...
            }
        }

        public void Draw() { }

        /// <summary>
        /// Controls example.
        ///
        /// All you gotta do is set them up, and make the input actions public.
        /// If you do, you can use them.
        /// </summary>
        class ExampleControls : Controls
        {
            public InputActionButton Explode;
            public InputActionButton Read;
            public InputActionAxis1D Twiddler;
            public InputActionAxis2D Movement;

            public ExampleControls(GamePlus game) : base(game)
            {
                // Buttons are pretty easy, just a list of possible buttons.
                Explode = new InputActionButton(this, Keys.A, Keys.Space, MouseButton.Left);
                // If we create explode twice the destructor should take care of un-assigning itself.
                Explode = new InputActionButton(this, Keys.B, Keys.Space, MouseButton.Left);
                Read = new InputActionButton(this, Keys.Space, Keys.R, Buttons.A);

                // Axis are a bit more complex but not by too much.
                Movement = new InputActionAxis2D(this){MaxMagnitude = 1f}
                    .Up(Keys.Up, Keys.W)
                    .Down(Keys.Down, Keys.S)
                    .Left(Keys.Left, Keys.A)
                    .Right(Keys.Right, Keys.D)
                    .XAxis(GamepadAxis.LStickX)
                    .YAxis(GamepadAxis.LStickY);
                Twiddler = new InputActionAxis1D(this, Keys.Up, Keys.Down, InputActionAxis1D.AxisMode.MaxMagnitude);
                // You can also use this. This should deallocate the previous twiddler and not cause any problems.
                Twiddler = new InputActionAxis1D(this).Positive(Keys.Up).Negative(Keys.Down);
            }
        }
    }
}
