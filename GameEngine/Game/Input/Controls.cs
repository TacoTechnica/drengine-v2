using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Math = GameEngine.Util.Math;

namespace GameEngine.Game.Input
{
    /// <summary>
    ///     Represents a list of controls that you can subscribe to.
    /// </summary>
    public abstract class Controls
    {
        private readonly List<InputAction> _actions = new List<InputAction>();

        protected GamePlus Game;

        public bool Enabled = true;

        public Controls(GamePlus game)
        {
            Game = game;
            Game.AddControls(this);
        }

        ~Controls()
        {
            Game.RemoveControls(this);
        }

        public void DoUpdate()
        {
            if (!Enabled) return;
            foreach (var a in _actions)
                if (a.Active)
                    a.Update();
        }

        internal void AddAction(InputAction a)
        {
            _actions.Add(a);
        }

        internal void RemoveAction(InputAction a)
        {
            _actions.Remove(a);
        }
    }


    public class InputAxis
    {
        private readonly GamepadAxis _gamepadAxis;
        private readonly Buttons _gamepadButton;

        private readonly Keys _keys;
        private readonly MouseButton _mbutton;
        private readonly MouseAxis _mouseAxis;

        private readonly Type _type;

        public bool Inverted;

        public InputAxis(Keys keys)
        {
            _keys = keys;
            _type = Type.Keyboard;
        }

        public InputAxis(MouseButton mbutton)
        {
            _mbutton = mbutton;
            _type = Type.Mouse;
        }

        public InputAxis(GamepadAxis gamepadAxis)
        {
            _gamepadAxis = gamepadAxis;
            _type = Type.GamepadAxis;
        }

        public InputAxis(MouseAxis mouseAxis)
        {
            _mouseAxis = mouseAxis;
            _type = Type.MouseAxis;
        }

        public InputAxis(Buttons gamepadButton)
        {
            _gamepadButton = gamepadButton;
            _type = Type.GamepadButton;
        }

        public float ReadCurrentAxis()
        {
            switch (_type)
            {
                case Type.Keyboard:
                    return RawInput.KeyPressing(_keys) ? 1f : 0;
                case Type.Mouse:
                    return RawInput.MousePressing(_mbutton) ? 1f : 0;
                case Type.MouseAxis:
                    switch (_mouseAxis)
                    {
                        case MouseAxis.X:
                            return RawInput.GetMousePosition().X;
                        case MouseAxis.Y:
                            return RawInput.GetMousePosition().Y;
                        case MouseAxis.Dx:
                            return RawInput.GetMouseDelta().X;
                        case MouseAxis.Dy:
                            return RawInput.GetMouseDelta().Y;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case Type.GamepadButton:
                    return RawInput.GamepadPressing(_gamepadButton) ? 1f : 0;
                case Type.GamepadAxis:
                    return RawInput.GetGamepadAxis(_gamepadAxis);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append("[");
            switch (_type)
            {
                case Type.Keyboard:
                    b.Append("Key=").Append(_keys);
                    break;
                case Type.Mouse:
                    b.Append("MButton=").Append(_mbutton);
                    break;
                case Type.MouseAxis:
                    b.Append("MAxis=").Append(_mouseAxis);
                    break;
                case Type.GamepadButton:
                    b.Append("GButton=").Append(_gamepadButton);
                    break;
                case Type.GamepadAxis:
                    b.Append("GAxis=").Append(_gamepadAxis);
                    break;
                default:
                    return base.ToString();
            }

            b.Append("]");
            return b.ToString();
        }


        /// Converters for ease of use
        public static implicit operator InputAxis(Keys k)
        {
            return new InputAxis(k);
        }

        public static implicit operator InputAxis(MouseButton m)
        {
            return new InputAxis(m);
        }

        public static implicit operator InputAxis(MouseAxis m)
        {
            return new InputAxis(m);
        }

        public static implicit operator InputAxis(GamepadAxis a)
        {
            return new InputAxis(a);
        }

        public static implicit operator InputAxis(Buttons b)
        {
            return new InputAxis(b);
        }

        private enum Type
        {
            Keyboard,
            Mouse,
            MouseAxis,
            GamepadButton,
            GamepadAxis
        }
    }

    public abstract class InputAction
    {
        private readonly Controls _controls;

        public bool Active = true;

        public InputAction(Controls controls)
        {
            _controls = controls;
            _controls.AddAction(this);
        }

        public bool Pressing { get; protected set; }

        ~InputAction()
        {
            _controls.RemoveAction(this);
        }

        internal abstract void Update();
    }

    public class InputActionButton : InputAction
    {
        private readonly InputAxis[] _axis;

        private bool _prevPressing;
        public Action<InputActionButton> Held;

        public Action<InputActionButton> Pressed;
        public Action<InputActionButton> Released;
        public float Threshold;

        public InputActionButton(Controls controls, InputAxis[] axis, float threshold = 0.5f) : base(controls)
        {
            _axis = axis;
            Threshold = threshold;
        }

        public InputActionButton(Controls controls, params InputAxis[] axis) : this(controls, axis, 0.5f)
        {
        }

        internal override void Update()
        {
            if (!Active) return;

            Pressing = false;

            foreach (var a in _axis)
                //Debug.Log($"{a} : {a.ReadCurrentAxis()} > {Threshold}?, {a.ReadCurrentAxis() > Threshold}");
                if (Math.Abs(a.ReadCurrentAxis()) > Threshold)
                {
                    Pressing = true;
                    break;
                }

            if (Pressing) Held?.Invoke(this);
            if (Pressing && !_prevPressing)
                Pressed?.Invoke(this);
            else if (!Pressing && _prevPressing) Released?.Invoke(this);

            _prevPressing = Pressing;
        }
    }

    public abstract class InputActionAxis<T> : InputAction
    {
        public enum AxisMode
        {
            MaxMagnitude,
            Sum
        }

        protected AxisMode AxisSumMode;

        public float Scale = 1f;

        public bool SquareInput;

        public T Value;

        public InputActionAxis(Controls controls, AxisMode axisSumMode = AxisMode.Sum) : base(controls)
        {
            AxisSumMode = axisSumMode;
        }
    }

    public class InputActionAxis1D : InputActionAxis<float>
    {
        private readonly List<InputAxis> _negative;
        private readonly List<InputAxis> _positive;

        public InputActionAxis1D(Controls controls, InputAxis[] negative, InputAxis[] positive,
            AxisMode sumMode = AxisMode.Sum) : base(controls, sumMode)
        {
            _positive = new List<InputAxis>(positive);
            _negative = new List<InputAxis>(negative);
        }

        /// <summary>
        ///     Empty constructor. Pair with .Positive() and .Negative()
        /// </summary>
        public InputActionAxis1D(Controls controls, AxisMode mode = AxisMode.Sum) : this(controls, new InputAxis[0],
            new InputAxis[0], mode)
        {
        }

        public InputActionAxis1D(Controls controls, InputAxis negative, InputAxis positive,
            AxisMode sumMode = AxisMode.Sum) : this(
            controls,
            new[] {negative}, new[] {positive}, sumMode)
        {
        }

        public InputActionAxis1D(Controls controls, InputAxis positive, AxisMode mode = AxisMode.Sum) : this(
            controls,
            new InputAxis[0], new[] {positive}, mode)
        {
        }

        internal override void Update()
        {
            float value = 0;
            foreach (var p in _positive)
            {
                var val = p.ReadCurrentAxis();
                switch (AxisSumMode)
                {
                    case AxisMode.MaxMagnitude:
                        if (Math.Abs(val) > value) value = val;
                        break;
                    case AxisMode.Sum:
                        value += val;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var p in _negative)
            {
                var val = p.ReadCurrentAxis();
                switch (AxisSumMode)
                {
                    case AxisMode.MaxMagnitude:
                        if (Math.Abs(val) > value) value = val;
                        break;
                    case AxisMode.Sum:
                        value -= val;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Value = value * Scale;
            if (SquareInput) Value = Math.CopySign(Value * Value, Value);
        }

        public InputActionAxis1D Positive(params InputAxis[] toAdd)
        {
            _positive.AddRange(toAdd);
            return this;
        }

        public InputActionAxis1D Negative(params InputAxis[] toAdd)
        {
            _negative.AddRange(toAdd);
            return this;
        }

        public InputActionAxis1D Squared()
        {
            SquareInput = true;
            return this;
        }
    }

    public class InputActionAxis2D : InputActionAxis<Vector2>
    {
        private readonly List<InputAxis> _down;
        private readonly List<InputAxis> _left;
        private readonly List<InputAxis> _right;
        private readonly List<InputAxis> _up;

        public float MaxMagnitude = float.PositiveInfinity;

        public float Threshold = 0.15f;

        /// <summary>
        ///     Default Constructor, all inputs specified.
        /// </summary>
        public InputActionAxis2D(Controls controls, InputAxis[] up, InputAxis[] down, InputAxis[] left,
            InputAxis[] right, AxisMode sumMode = AxisMode.Sum) : base(controls, sumMode)
        {
            _up = new List<InputAxis>(up);
            _down = new List<InputAxis>(down);
            _left = new List<InputAxis>(left);
            _right = new List<InputAxis>(right);
        }

        /// <summary>
        ///     Two axis Constructor.
        /// </summary>
        public InputActionAxis2D(Controls controls, InputAxis[] axisY, InputAxis[] axisX,
            AxisMode mode = AxisMode.Sum) :
            this(controls, axisY, new InputAxis[] { }, new InputAxis[] { }, axisX, mode)
        {
        }

        public InputActionAxis2D(Controls controls, InputAxis axisY, InputAxis axisX, AxisMode mode = AxisMode.Sum) :
            this(controls, new[] {axisY}, new[] {axisX}, mode)
        {
        }

        /// <summary>
        ///     Empty Constructor
        /// </summary>
        public InputActionAxis2D(Controls controls, AxisMode mode = AxisMode.Sum) : this(controls, new InputAxis[0],
            new InputAxis[0], new InputAxis[0], new InputAxis[0], mode)
        {
        }

        /// <summary>
        ///     Single mapping constructor
        /// </summary>
        public InputActionAxis2D(Controls controls, InputAxis up, InputAxis down, InputAxis left, InputAxis right,
            AxisMode sumMode = AxisMode.Sum) : this(
            controls,
            new[] {up}, new[] {down}, new[] {left}, new[] {right}, sumMode)
        {
        }

        internal override void Update()
        {
            Value = Vector2.Zero
                    + Apply(_up, Vector2.UnitY)
                    - Apply(_down, Vector2.UnitY)
                    - Apply(_left, Vector2.UnitX)
                    + Apply(_right, Vector2.UnitX);
            switch (AxisSumMode)
            {
                case AxisMode.Sum:
                    break;
                case AxisMode.MaxMagnitude:
                    Value.Normalize();
                    break;
            }

            if (SquareInput)
            {
                Value.X = Math.CopySign(Value.X * Value.X, Value.X);
                Value.Y = Math.CopySign(Value.Y * Value.Y, Value.Y);
            }

            Value = Math.ClampMagnitude(Value * Scale, MaxMagnitude);
        }

        private Vector2 Apply(IEnumerable<InputAxis> sideAxis, Vector2 sideDirection)
        {
            var result = Vector2.Zero;
            var max = Vector2.Zero;
            foreach (var p in sideAxis)
            {
                var val = p.ReadCurrentAxis();
                var pInput = Math.Threshold(val, Threshold) * sideDirection;
                switch (AxisSumMode)
                {
                    case AxisMode.MaxMagnitude:
                        if (pInput.LengthSquared() > max.LengthSquared()) max = pInput;
                        break;
                    case AxisMode.Sum:
                        result += pInput;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (AxisSumMode)
            {
                case AxisMode.MaxMagnitude:
                    return max;
                case AxisMode.Sum:
                    return result;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public InputActionAxis2D Up(params InputAxis[] toAdd)
        {
            _up.AddRange(toAdd);
            return this;
        }

        public InputActionAxis2D Down(params InputAxis[] toAdd)
        {
            _down.AddRange(toAdd);
            return this;
        }

        public InputActionAxis2D Left(params InputAxis[] toAdd)
        {
            _left.AddRange(toAdd);
            return this;
        }

        public InputActionAxis2D Right(params InputAxis[] toAdd)
        {
            _right.AddRange(toAdd);
            return this;
        }

        public InputActionAxis2D XAxis(params InputAxis[] toAdd)
        {
            return Right(toAdd);
        }

        public InputActionAxis2D YAxis(params InputAxis[] toAdd)
        {
            return Up(toAdd);
        }

        public InputActionAxis2D Squared()
        {
            SquareInput = true;
            return this;
        }
    }
}