using Ryne.Input;
using Ryne.Utility.Math;
using Math = System.Math;

namespace Ryne.Utility.Actions
{
    public class InputAction : IAction
    {
        public Controller Controller;

        public InputAction()
        {
            Controller = null;
        }

        public virtual bool Execute()
        {
            return true;
        }

        public bool Undo()
        {
            Logger.Warning("Called Undo on an InputAction");
            return false;
        }
    }


    public class FovInputAction : InputAction
    {
        public float Delta { get; set; }

        public FovInputAction(float delta)
        {
            Delta = delta;
        }

        public override bool Execute()
        {
            if (!Controller.CaptureMouse)
            {
                return false;
            }

            Controller.FovDelta += Delta;
            return true;
        }
    }

    // struct that contains a direction and length
    public class VectorInputAction : InputAction
    {
        public bool Relative { get; set; }
        public Float3 Direction { get; set; }
        public float Length { get; set; }

        public VectorInputAction(Float3 direction, float length)
        {
            Direction = direction;
            Length = length;
        }

        public virtual Float4 GetDirection()
        {
            Float4 dir = new Float4(Direction);
            if (!Relative || Controller.ControlledEntity == null)
            {
                return dir;
            }

            return Controller.ControlledEntity.Transform.Rotation.RotateVector(dir);
        }

        // Return the vector of this action
        public virtual Float4 GetVector()
        {
            return GetDirection() * Length;
        }
    }


    class InputMovementAction : VectorInputAction
    {
        // Whether this input will be normalized with other inputs in the Controller
        public bool Normalize { get; }

        public InputMovementAction(Float3 direction, float length, bool relative = true, bool normalize = true) : base(direction, length)
        {
            Relative = relative;
            Normalize = normalize;
        }

        public override bool Execute()
        {
            if (!Controller.CaptureMouse)
            {
                return false;
            }

            var vector = GetVector();
            Controller.MovementRawDelta += vector;
            if (Normalize)
            {
                Controller.MovementNormalizedLength = System.Math.Max(vector.Length(), Controller.MovementNormalizedLength);
            }
            else
            {
                Controller.MovementDirectDelta += vector;
            }

            return true;
        }
    }


    class InputRotationAction : VectorInputAction
    {
        public InputRotationAction(Float3 direction, float length, bool relative = false) : base(direction, length)
        {
            Relative = relative;

            // Reorient direction from input rotator [Yaw, Pitch, Roll] to quaternion [Roll, Pitch, Yaw]
            Direction = new Float3(direction.Z, direction.Y, direction.X);
        }

        public Float4 GetRotation()
        {
            // Reorient direction from quaternion [Roll, Pitch, Yaw] to rotator [Yaw, Pitch, Roll] 
            Float4 dir = new Float4(Direction.Z, Direction.Y, Direction.X, 0);
            return dir * Length;
        }

        public Quaternion GetQuaternion(float sizeMultiplier)
        {
            return new Quaternion(new Float3(GetDirection()), RyneMath.DegreesToRadians(Length) * sizeMultiplier);
        }

        public override bool Execute()
        {
            if (!Controller.CaptureMouse)
            {
                return false;
            }
            Controller.RotationDelta += GetVector();
            return true;
        }
    }


    class InputScalingAction : VectorInputAction
    {
        public InputScalingAction(Float3 direction, float length) : base(direction, length)
        {
        }

        public override bool Execute()
        {
            if (!Controller.CaptureMouse)
            {
                return false;
            }
            Controller.ScaleDelta += GetVector();
            return true;
        }
    }


    public delegate void InputControllerActionCallback(Controller controller);
    class InputControllerAction : InputAction
    {
        public InputControllerActionCallback Function;

        public InputControllerAction(InputControllerActionCallback function)
        {
            Function = function;
        }

        public override bool Execute()
        {
            Function?.Invoke(Controller);
            return true;
        }
    }


    class InputDelegateAction : InputAction
    {
        public RyneNotifyFunction Function;

        public InputDelegateAction(RyneNotifyFunction function)
        {
            Function = function;
        }

        public override bool Execute()
        {
            Function?.Invoke();
            return true;
        }
    }
}
