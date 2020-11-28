using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Game.Input
{
    /// <summary>
    /// Represents a list of controls that you can subscribe to.
    /// </summary>
    public abstract class Controls
    {
        private List<InputAction> _actions = new List<InputAction>();

        public bool Enabled = true;

        protected GamePlus _game;

        public Controls(GamePlus game)
        {
            _game = game;
            _game.AddControls(this);
        }

        ~Controls()
        {
            _game.RemoveControls(this);
        }

        public void DoUpdate()
        {
            if (!Enabled) return;
            foreach (InputAction a in _actions)
            {
                if (a.Active) a.Update();
            }
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
        enum Type
        {
            Keyboard,
            Mouse,
            MouseAxis,
            GamepadButton,
            GamepadAxis
        }

        private Type _type;

        private Keys _keys;
        private MouseButton _mbutton;
        private MouseAxis _mouseAxis;
        private Buttons _gamepadButton;
        private GamepadAxis _gamepadAxis;

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
                    return RawInput.KeyPressing(_keys)? 1f : 0;
                case Type.Mouse:
                    return RawInput.MousePressing(_mbutton)? 1f : 0;
                case Type.MouseAxis:
                    switch (_mouseAxis)
                    {
                        case MouseAxis.X:
                            return RawInput.GetMousePosition().X;
                        case MouseAxis.Y:
                            return RawInput.GetMousePosition().Y;
                        case MouseAxis.DX:
                            return RawInput.GetMouseDelta().X;
                        case MouseAxis.DY:
                            return RawInput.GetMouseDelta().Y;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case Type.GamepadButton:
                    return RawInput.GamepadPressing(_gamepadButton)? 1f : 0;
                case Type.GamepadAxis:
                    return RawInput.GetGamepadAxis(_gamepadAxis);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
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
    }

    public abstract class InputAction
    {

        public bool Active = true;
        public bool Pressing { get; protected set; }

        private Controls _controls;

        public InputAction(Controls controls)
        {
            _controls = controls;
            _controls.AddAction(this);
        }

        ~InputAction()
        {
            _controls.RemoveAction(this);
        }

        internal abstract void Update();
    }

    public class InputActionButton : InputAction
    {
        private InputAxis[] _axis;

        private bool _prevPressing = false;
        public float Threshold;

        public Action<InputActionButton> Pressed;
        public Action<InputActionButton> Held;
        public Action<InputActionButton> Released;

        public InputActionButton(Controls controls, InputAxis[] axis, float threshold=0.5f) : base(controls)
        {
            _axis = axis;
            Threshold = threshold;
        }

        public InputActionButton(Controls controls, params InputAxis[] axis) : this(controls, axis, 0.5f) {}

        internal override void Update()
        {
            if (!Active) return;

            Pressing = false;

            foreach (InputAxis a in _axis)
            {
                //Debug.Log($"{a} : {a.ReadCurrentAxis()} > {Threshold}?, {a.ReadCurrentAxis() > Threshold}");
                if (Math.Abs(a.ReadCurrentAxis()) > Threshold)
                {
                    Pressing = true;
                    break;
                }
            }

            if (Pressing)
            {
                Held?.Invoke(this);
            }
            if (Pressing && !_prevPressing)
            {
                Pressed?.Invoke(this);
            }
            else if (!Pressing && _prevPressing)
            {
                Released?.Invoke(this);
            }

            _prevPressing = Pressing;
        }
    }

    public abstract class InputActionAxis<T> : InputAction {

        public enum AxisMode
        {
            MaxMagnitude,
            Sum
        }

        public float Scale = 1f;

        protected AxisMode _axisMode;

        public bool SquareInput = false;

        public T Value;

        public InputActionAxis( Controls controls, AxisMode axisMode = AxisMode.Sum) : base(controls)
        {
            _axisMode = axisMode;
        }
    }

    public class InputActionAxis1D : InputActionAxis<float>
    {
        private List<InputAxis> _positive;
        private List<InputAxis> _negative;

        public InputActionAxis1D(Controls controls, InputAxis[] negative, InputAxis[] positive, AxisMode mode = AxisMode.Sum) : base(controls, mode)
        {
            _positive = new List<InputAxis>(positive);
            _negative = new List<InputAxis>(negative);
        }

        /// <summary>
        ///     Empty constructor. Pair with .Positive() and .Negative()
        /// </summary>
        public InputActionAxis1D(Controls controls, AxisMode mode = AxisMode.Sum) : this(controls, new InputAxis[0],
            new InputAxis[0], mode) { }
        public InputActionAxis1D(Controls controls, InputAxis negative, InputAxis positive, AxisMode mode = AxisMode.Sum) : this(
            controls,
            new[]{negative}, new[]{positive}, mode ) {}
        public InputActionAxis1D(Controls controls, InputAxis positive, AxisMode mode = AxisMode.Sum) : this(
            controls,
            new InputAxis[0], new[]{positive}, mode ) {}

        internal override void Update()
        {
            float value = 0;
            foreach (InputAxis p in _positive)
            {
                float val = p.ReadCurrentAxis();
                switch (_axisMode)
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
            foreach (InputAxis p in _negative)
            {
                float val = p.ReadCurrentAxis();
                switch (_axisMode)
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
            if (SquareInput)
            {
                Value = Math.CopySign(Value * Value, Value);
            }
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
        private List<InputAxis> _up;
        private List<InputAxis> _down;
        private List<InputAxis> _left;
        private List<InputAxis> _right;

        public float Threshold = 0.15f;

        public float MaxMagnitude = float.PositiveInfinity;

        /// <summary>
        /// Default Constructor, all inputs specified.
        /// </summary>
        public InputActionAxis2D(Controls controls, InputAxis[] up, InputAxis[] down, InputAxis[] left, InputAxis[] right, AxisMode mode = AxisMode.Sum) : base(controls, mode)
        {
            _up = new List<InputAxis>(up);
            _down = new List<InputAxis>(down);
            _left = new List<InputAxis>(left);
            _right = new List<InputAxis>(right);
        }
        /// <summary>
        /// Two axis Constructor.
        /// </summary>
        public InputActionAxis2D(Controls controls, InputAxis[] axisY, InputAxis[] axisX, AxisMode mode = AxisMode.Sum) :
            this(controls, axisY, new InputAxis[]{}, new InputAxis[]{}, axisX, mode ) {}
        public InputActionAxis2D(Controls controls, InputAxis axisY, InputAxis axisX, AxisMode mode = AxisMode.Sum) :
            this(controls, new []{axisY}, new []{axisX}, mode) {}
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public InputActionAxis2D(Controls controls, AxisMode mode = AxisMode.Sum) : this(controls, new InputAxis[0], new InputAxis[0], new InputAxis[0], new InputAxis[0], mode  ) {}
        /// <summary>
        /// Single mapping constructor
        /// </summary>
        public InputActionAxis2D(Controls controls, InputAxis up, InputAxis down, InputAxis left, InputAxis right, AxisMode mode = AxisMode.Sum) : this(
            controls,
            new InputAxis[]{up}, new InputAxis[]{down}, new InputAxis[]{left}, new InputAxis[]{right}, mode ) {}

        internal override void Update()
        {
            Value = Vector2.Zero
                            + Apply(_up, Vector2.UnitY)
                            - Apply(_down, Vector2.UnitY)
                            - Apply(_left, Vector2.UnitX)
                            + Apply(_right, Vector2.UnitX);
            switch (_axisMode)
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
            Vector2 result = Vector2.Zero;
            Vector2 max = Vector2.Zero;
            foreach (InputAxis p in sideAxis)
            {
                float val = p.ReadCurrentAxis();
                Vector2 pInput = Math.Threshold(val, Threshold) * sideDirection;
                switch (_axisMode)
                {
                    case AxisMode.MaxMagnitude:
                        if (pInput.LengthSquared() > max.LengthSquared())
                        {
                            max = pInput;
                        }
                        break;
                    case AxisMode.Sum:
                        result += pInput;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (_axisMode)
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
