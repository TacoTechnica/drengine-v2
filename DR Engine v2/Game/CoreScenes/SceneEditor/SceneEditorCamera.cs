using System;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Collision;
using GameEngine.Game.Input;
using GameEngine.Game.Objects.Rendering;
using GameEngine.Game.Tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Math = GameEngine.Util.Math;

namespace DREngine.Game.CoreScenes.SceneEditor
{
    public class SceneEditorCamera : Camera3D
    {
        private readonly CamControls _controls;

        private bool _focused;
        private Quaternion _targetLook;
        private Vector3 _velocity;

        public float LookStrength = 26f;
        public float MoveAcceleration = 50f;
        public float MoveDamping = 8f;

        public Action<ICollider> ColliderSelected;

        private Tween<Vector3> _moveTween;

        private DRGame _game;

        private Vector2 _prevMousePos;

        public SceneEditorCamera(DRGame game, Vector3 pos, Quaternion rotation, float fov = 90) : base(game, pos, rotation,
            fov)
        {
            _game = game;
            _controls = new CamControls(game);

            _controls.Focus.Pressed += OnFocus;
            _controls.Focus.Released += OnUnfocus;

            _targetLook = rotation;

            _controls.Select.Pressed += OnSelect;
            
            if (game.DebugConsole != null)
            {
                game.DebugConsole.OnOpened += OnDebugOpened;
                game.DebugConsole.OnClosed += OnDebugClosed;
            }
        }

        private void OnSelect(InputActionButton obj)
        {
            Vector2 pos = RawInput.GetMousePosition();
            ICollider c = _game.CollisionManager.ScreenCollisionCheckNearest(this, pos);
            if (c != null)
            {
                ColliderSelected?.Invoke(c);
            }
        }


        private void OnDebugOpened()
        {
            RawInput.SetMouseLock(false);
            _controls.Enabled = false;
        }

        private void OnDebugClosed()
        {
            if (_focused) RawInput.SetMouseLock(true);
            _controls.Enabled = true;
        }

        private void OnUnfocus(InputActionButton obj)
        {
            _focused = false;
            RawInput.SetMouseLock(false);
            RawInput.SetMousePos(_prevMousePos);
        }

        private void OnFocus(InputActionButton obj)
        {
            _focused = true;
            _prevMousePos = RawInput.GetMousePosition();
            RawInput.SetMouseLock(true);
        }

        public override void Update(float dt)
        {
            HandleMove(dt);
            HandleLook(dt);
            base.Update(dt);
        }

        public void LookAt(Vector3 target, float distance)
        {
            //this.Tweener.TweenValue()
            Vector3 delta = -1 * distance * Math.RotateVector(Vector3.Forward, Rotation);

            _moveTween?.Cancel();
            _moveTween = Tweener.TweenValue(Position, target + delta, value =>
            {
                Position = value;
            }, 0.5f)
            .SetEaseSineOut()
            .SetOnComplete(() =>
            {
                _moveTween = null;
            });
        }

        private void HandleLook(float dt)
        {
            // Only move if we're locked with the mouse. Otherwise we might be doing other things.
            if (_focused && _controls.Enabled && _moveTween == null)
            {
                var input = _controls.MouseLook.Value;
                var targetEuler = Math.ToEuler(_targetLook);
                var impulse = LookStrength * dt * new Vector3(-input.Y, -input.X, 0);
                targetEuler.X += impulse.X; // TODO: Math.AddAngleAndClamp(targetEuler.X, impulse.X, -89, 89);
                targetEuler.Y += impulse.Y;
                //Debug.Log($"{targetEuler.X}");
                _targetLook = Math.FromEuler(targetEuler);


                // Move towards target asymptotically
                Rotation = Quaternion.Lerp(Rotation, _targetLook, dt * 30);
            }

        }

        private void HandleMove(float dt)
        {
            if (_focused && _controls.Enabled)
            {
                var input = Vector3.Forward * _controls.ForwardBack.Value
                            + Vector3.Right * _controls.RightLeft.Value
                            + Vector3.Up * _controls.UpDown.Value;
                input = Math.RotateVector(input, Rotation);

                var accel = MoveAcceleration;
                if (_controls.Speed.Pressing)
                {
                    accel *= 4;
                }

                _velocity += input * (accel * dt);
            }

            _velocity -= _velocity * (MoveDamping * dt);
            Position += _velocity * dt;
        }

        private class CamControls : GameEngine.Game.Input.Controls
        {
            public readonly InputActionButton Focus;
            public readonly InputActionButton Speed;

            public readonly InputActionButton Select;

            public readonly InputActionAxis2D MouseLook;
            public readonly InputActionAxis1D ForwardBack;
            public readonly InputActionAxis1D RightLeft;
            public readonly InputActionAxis1D UpDown;


            public CamControls(GamePlus game) : base(game)
            {
                ForwardBack = new InputActionAxis1D(this, Keys.S, Keys.W);
                RightLeft = new InputActionAxis1D(this, Keys.A, Keys.D);
                UpDown = new InputActionAxis1D(this, Keys.Q, Keys.E);
                MouseLook = new InputActionAxis2D(this, MouseAxis.Dy, MouseAxis.Dx);

                Select = new InputActionButton(this, MouseButton.Left);
                Focus = new InputActionButton(this, MouseButton.Right);
                Speed = new InputActionButton(this, Keys.LeftShift);
            }
        }
    }
}