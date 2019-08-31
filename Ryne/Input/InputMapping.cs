using System;
using System.Collections.Generic;
using System.Linq;
using Ryne.Utility;
using Ryne.Utility.Actions;

namespace Ryne.Input
{
    public enum InputType
    {
        Pressed,
        Released,
        Down
    }

    public struct KeyInput
    {
        public RyneKey Key;
        public InputType Type;
        public InputAction Action;

        public bool Valid()
        {
            return (int)Key >= 32 && (int)Key <= 348 && Action != null;
        }
    }

    public struct ComboKeyInput
    {
        public RyneKey Key1;
        public InputType Type1;
        public RyneKey Key2;
        public InputType Type2;
        public InputAction Action;
        public bool ConsumeInput;

        public bool Valid()
        {
            return (int)Key1 >= 32 && (int)Key1 <= 348 && (int)Key2 >= 32 && (int)Key2 <= 348 && Action != null;
        }
    }

    public struct MouseInput
    {
        public RyneMouse Key;
        public InputType Type;
        public InputAction Action;

        public bool Valid()
        {
            return (int)Key >= 0 && (int)Key <= 7 && Action != null;
        }
    }

    public struct MouseScrollInput
    {
        public RyneMouseWheel Key;
        public InputAction Action;

        public bool Valid()
        {
            return (int)Key >= 0 && (int)Key <= 7 && Action != null;
        }
    }


    public struct MouseAxisInput
    {
        public Float2 Axis;
        public InputAction Action;

        public bool Valid()
        {
            return Axis.Length() > 0.0f && Action != null;
        }
    }

    public class InputMapping
    {
        public List<KeyInput> KeyInputs;
        public List<ComboKeyInput> ComboKeyInputs;

        public List<MouseInput> MouseInputs;
        public List<MouseScrollInput> ScrollInputs;
        public List<MouseAxisInput> MouseAxisInputs;

        public bool CaptureMouse;

        public InputMapping()
        {
            KeyInputs = new List<KeyInput>();
            ComboKeyInputs = new List<ComboKeyInput>();
            MouseInputs = new List<MouseInput>();
            ScrollInputs = new List<MouseScrollInput>();
            MouseAxisInputs = new List<MouseAxisInput>();
            CaptureMouse = false;
        }

        public void AddInput(KeyInput input)
        {
            if (!input.Valid())
            {
                Logger.Error("Invalid KeyInput: action not stored");
                return;
            }

            if (KeyInputs.Any(x => x.Key == input.Key && x.Type == input.Type))
            {
                Logger.Warning($"Added duplicate KeyInput {input.Key}, {input.Type}");
            }

            KeyInputs.Add(input);
        }

        public void AddInput(ComboKeyInput input)
        {
            if (!input.Valid())
            {
                Logger.Error("Invalid ComboKeyInput: action not stored");
                return;
            }

            ComboKeyInputs.Add(input);
        }

        public void AddInput(MouseInput input)
        {
            if (!input.Valid())
            {
                Logger.Error("Invalid MouseAxisInput: action not stored");
                return;
            }

            if (MouseInputs.Any(x => x.Key == input.Key && x.Type == input.Type))
            {
                Logger.Warning($"Added duplicate MouseInput {input.Key}, {input.Type}");
            }

            MouseInputs.Add(input);
        }

        public void AddInput(MouseScrollInput input)
        {
            if (!input.Valid())
            {
                Logger.Error("Invalid KeyInput: action not stored");
                return;
            }

            if (ScrollInputs.Any(x => x.Key == input.Key))
            {
                Logger.Warning($"Added duplicate ScrollInput {input.Key}");
            }

            ScrollInputs.Add(input);
        }

        public void AddInput(MouseAxisInput input)
        {
            if (!input.Valid())
            {
                Logger.Error("Invalid MouseAxisInput: action not stored");
                return;
            }

            MouseAxisInputs.Add(input);
        }
    }
}
