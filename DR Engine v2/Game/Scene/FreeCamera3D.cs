using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Objects.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game.Scene
{
    public class FreeCamera3D : Camera3D
    {
        private Vector3 _velocity;
        private Quaternion _targetLook;
        private CamControls _controls;

        public float LookStrength = 16f;
        public float MoveAcceleration = 30f;
        public float MoveDamping = 10f;

        private bool _focused = false;

        public FreeCamera3D(GamePlus game, Vector3 pos, Quaternion rotation, float fov = 90) : base(game, pos, rotation, fov)
        {
            _controls = new CamControls(game);

            _controls.Focus.Pressed += OnFocus;
            _controls.UnFocus.Pressed += OnUnfocus;

            _targetLook = rotation;

            if (game.DebugConsole != null)
            {
                game.DebugConsole.OnOpened += OnDebugOpened;
                game.DebugConsole.OnClosed += OnDebugClosed;
            }
        }


        private void OnDebugOpened()
        {
            RawInput.SetMouseLock(false);
            _controls.Enabled = false;
        }
        private void OnDebugClosed()
        {
            if (_focused)
            {
                RawInput.SetMouseLock(true);
            }
            _controls.Enabled = true;
        }

        private void OnUnfocus(InputActionButton obj)
        {
            _focused = false;
            RawInput.SetMouseLock(false);
        }

        private void OnFocus(InputActionButton obj)
        {
            _focused = true;
            RawInput.SetMouseLock(true);
        }

        public override void Update(float dt)
        {
            HandleMove(dt);
            HandleLook(dt);
            base.Update(dt);
        }

        void HandleLook(float dt)
        {
            // Only move if we're locked with the mouse. Otherwise we might be doing other things.
            if (_focused && _controls.Enabled)
            {
                Vector2 input = _controls.MouseLook.Value;
                Vector3 targetEuler = Math.ToEuler(_targetLook);
                Vector3 impulse = (LookStrength * dt) * new Vector3(-input.Y, -input.X, 0);
                targetEuler.X += impulse.X;// TODO: Math.AddAngleAndClamp(targetEuler.X, impulse.X, -89, 89);
                targetEuler.Y += impulse.Y;
                //Debug.Log($"{targetEuler.X}");
                _targetLook = Math.FromEuler(targetEuler);


                // Move towards target asymptotically
                Rotation = Quaternion.Lerp(Rotation, _targetLook, dt * 30);
            }
        }

        void HandleMove(float dt)
        {
            if (_focused && _controls.Enabled)
            {

                Vector3 input = Vector3.Forward * _controls.ForwardBack.Value
                                + Vector3.Right * _controls.RightLeft.Value
                                + Vector3.Up * _controls.UpDown.Value;
                input = Math.RotateVector(input, Rotation);
                _velocity += input * (MoveAcceleration * dt);
            }

            _velocity -= _velocity * (MoveDamping * dt);
            Position += _velocity * dt;
        }

        class CamControls : GameEngine.Game.Input.Controls
        {
            public InputActionAxis1D ForwardBack;
            public InputActionAxis1D RightLeft;
            public InputActionAxis1D UpDown;
            public InputActionAxis2D MouseLook;

            public InputActionButton UnFocus;
            public InputActionButton Focus;

            public CamControls(GamePlus game) : base(game)
            {
                ForwardBack = new InputActionAxis1D(this, Keys.S, Keys.W);
                RightLeft = new InputActionAxis1D(this, Keys.A, Keys.D);
                UpDown = new InputActionAxis1D(this, Keys.Q, Keys.E);
                MouseLook = new InputActionAxis2D(this, MouseAxis.DY, MouseAxis.DX);

                Focus = new InputActionButton(this, MouseButton.Left);
                UnFocus = new InputActionButton(this, Keys.Escape);
            }
        }
    }
}
