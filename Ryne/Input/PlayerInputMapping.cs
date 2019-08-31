using Ryne.Utility.Actions;

namespace Ryne.Input
{
    class PlayerInputMapping : InputMapping
    {
        public PlayerInputMapping(float moveSpeed = 100.0f, float jumpSpeed = 200.0f, float mouseSensitivity = 5.0f)
        {
            // Movement controls
            AddInput(new KeyInput { Key = RyneKey.W, Type = InputType.Down, Action = new InputMovementAction(new Float3(1, 0, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.S, Type = InputType.Down, Action = new InputMovementAction(new Float3(-1, 0, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.A, Type = InputType.Down, Action = new InputMovementAction(new Float3(0, -1, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.D, Type = InputType.Down, Action = new InputMovementAction(new Float3(0, 1, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.Space, Type = InputType.Pressed, Action = new InputMovementAction(new Float3(0, 0, 1), jumpSpeed, false, false) });

            // View controls
            AddInput(new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputRotationAction(new Float3(1, 0, 0), mouseSensitivity) });
            AddInput(new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputRotationAction(new Float3(0, 1, 0), mouseSensitivity, true) });

            CaptureMouse = true;
        }
    }
}
