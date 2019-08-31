using System.Collections.Generic;
using System.Linq;
using Ryne.Entities;
using Ryne.Scene.Components;
using Ryne.Utility;
using Ryne.Utility.Actions;
using Ryne.Utility.Math;
using Math = System.Math;

namespace Ryne.Input
{
    public class Controller : IUpdatable
    {
        public Float4 MovementRawDelta;
        // Normalized length input added the acceleration
        public Float4 MovementNormalizedDelta;
        // Input added directly to the velocity instead of acceleration
        public Float4 MovementDirectDelta;
        public Float4 ScaleDelta;
        public Float4 RotationDelta;
        public Quaternion RotationQuaternionDelta;

        public float FovDelta;

        public float MovementNormalizedLength;

        public bool CaptureMouse { get; private set; }
        public bool CenterMouse { get; private set; }
        public bool ClampVerticalRotation { get; set; }

        public InputMapping ActiveMapping { get; private set; }
        public List<Entity> ControlledEntities { get; }

        public Entity ControlledEntity => ControlledEntities.Count > 0 ? ControlledEntities[0] : null;

        public RyneNotifyFunction OnEntitiesChanged;

        // Add movement changes as force to the controlled entity its physics component, default true
        public bool AddMovementAsForce;

        private readonly RyneApplication Application;
        private Float2 SavedCursorPosition;

        public Controller()
        {
            Application = Global.Application;

            ControlledEntities = new List<Entity>();
            ActiveMapping = null;

            CaptureMouse = false;
            CenterMouse = false;
            ClampVerticalRotation = false;
        }

        public void Initialize()
        {

        }

        public void Update(float dt)
        {
            // Reset deltas 
            MovementNormalizedDelta = new Float4(0.0f);
            MovementRawDelta = new Float4(0.0f);
            MovementDirectDelta = new Float4(0.0f);
            RotationDelta = new Float4(0.0f);
            RotationQuaternionDelta = Quaternion.Default;
            ScaleDelta = new Float4(0.0f);
            FovDelta = 0.0f;
            MovementNormalizedLength = 0.0f;

            if (ActiveMapping != null)
            {
                foreach (var input in ActiveMapping.KeyInputs)
                {
                    if (CheckKeyInput(input.Key, input.Type))
                    {
                        // Skip single keys if there is a combo key input active
                        bool skipKey = false;
                        foreach (var comboKeyInput in ActiveMapping.ComboKeyInputs.Where(x => x.ConsumeInput && (x.Key1 == input.Key || x.Key2 == input.Key)))
                        {
                            var key = comboKeyInput.Key1 == input.Key ? comboKeyInput.Key2 : comboKeyInput.Key1;
                            var type = comboKeyInput.Key1 == input.Key ? comboKeyInput.Type2 : comboKeyInput.Type1;
                            if (CheckKeyInput(key, type))
                            {
                                skipKey = true;
                                break;
                            }
                        }

                        if (skipKey)
                        {
                            continue;
                        }

                        input.Action.Execute();
                    }
                }

                foreach (var input in ActiveMapping.ComboKeyInputs)
                {
                    if (CheckKeyInput(input.Key1, input.Type1) && CheckKeyInput(input.Key2, input.Type2))
                    {
                        input.Action.Execute();
                    }
                }

                foreach (var input in ActiveMapping.MouseInputs)
                {
                    if (CheckMouseInput(input.Key, input.Type))
                    {
                        input.Action.Execute();
                    }
                }

                foreach (var input in ActiveMapping.ScrollInputs)
                {
                    if (CheckScrollInput(input.Key))
                    {
                        input.Action.Execute();
                    }
                }

                if (CaptureMouse)
                {
                    Float2 mouseDiff = Application.GetCursorDiff(true);

                    foreach (var input in ActiveMapping.MouseAxisInputs)
                    {
                        float length = input.Axis.Dot(mouseDiff);
                        if (Math.Abs(length) > Constants.Epsilon)
                        {
                            if (input.Action is InputMovementAction movementAction)
                            {
                                // Copy of the InputMovementAction.Execute function but multiplied with length for mouse input
                                var vector = movementAction.GetVector() * length;
                                MovementRawDelta += vector;
                                if (movementAction.Normalize)
                                {
                                    MovementNormalizedLength = Math.Max(vector.Length(), MovementNormalizedLength);
                                }
                                else
                                {
                                    MovementDirectDelta += vector;
                                }
                            }
                            else if (input.Action is InputRotationAction rotationAction)
                            {
                                RotationDelta += rotationAction.GetRotation() * length;
                                // TODO: move dt to UpdateEntities
                                RotationQuaternionDelta *= rotationAction.GetQuaternion(length * dt);
                            }
                            else if (input.Action is InputScalingAction scaleAction)
                            {
                                ScaleDelta += scaleAction.GetVector() * length;
                            }
                            else
                            {
                                input.Action.Execute();
                            }
                        }
                    }
                }
            }

            MovementNormalizedDelta += MovementRawDelta.Normalize() * MovementNormalizedLength;

            UpdateEntities(dt);
        }

        public void Destroy()
        {
            if (CaptureMouse)
            {
                SetCaptureMouse(false);
            }
        }

        public virtual void UpdateEntities(float dt)
        {
            if (!Changed())
            {
                return;
            }

            foreach (var entity in ControlledEntities)
            {
                // Update entity transform
                ref var transform = ref entity.Transform;

                if (RotationDelta.Length() > Constants.Epsilon)
                {
                    var rotation = transform.Rotation.ToRotator().ToFloat4();
                    rotation += RotationDelta * dt;
                    if (ClampVerticalRotation)
                    {
                        rotation.Y = RyneMath.Clamp(rotation.Y, -89.0f, 89.0f);
                    }

                    transform.Rotation = new Rotator(rotation).ToQuaternion();
                }

                if (MovementRawDelta.Length() > Constants.Epsilon)
                {
                    if (AddMovementAsForce)
                    {
                        ref var physics = ref entity.Physics;
                        physics.Acceleration += MovementNormalizedDelta * dt;
                        physics.Velocity += MovementDirectDelta * dt;
                    }
                    else
                    {
                        transform.Position += MovementRawDelta * dt;
                    }
                }

                if (ScaleDelta.Length() > Constants.Epsilon)
                {
                    transform.Scale += ScaleDelta * dt;
                }

                if (Math.Abs(FovDelta) > Constants.Epsilon)
                {
                }
            }

            OnEntitiesChanged?.Invoke();
        }

        public void SetInputMapping(InputMapping value)
        {
            foreach (var input in value.KeyInputs)
            {
                input.Action.Controller = this;
            }
            foreach (var input in value.ComboKeyInputs)
            {
                input.Action.Controller = this;
            }
            foreach (var input in value.MouseInputs)
            {
                input.Action.Controller = this;
            }
            foreach (var input in value.MouseAxisInputs)
            {
                input.Action.Controller = this;
            }

            ActiveMapping = value;
            SetCaptureMouse(ActiveMapping.CaptureMouse);
        }

        // Control a single entity, clears the active controlled entities
        public void ControlEntity(Entity entity)
        {
            ControlledEntities.Clear();
            ControlledEntities.Add(entity);

            AddMovementAsForce = entity.HasComponent<PhysicsComponent>();
        }

        // Add entity to the list of controlled entities
        public void AddEntity(Entity entity)
        {
            ControlledEntities.Add(entity);
        }

        public void SetCaptureMouse(bool captureMouse)
        {
            if (CaptureMouse != captureMouse)
            {
                CaptureMouse = captureMouse;

                if (!CaptureMouse)
                {
                    // First unhide then restore cursor position
                    Application.SetCursorHidden(CaptureMouse);
                    Application.SetCursorPosition(SavedCursorPosition);
                }
                else
                {
                    // Save position then hide the cursor
                    if (CenterMouse)
                    {
                        Int2 screenSize = Application.GetScreenSize();
                        SavedCursorPosition = new Float2(screenSize.X, screenSize.Y) * 0.5f;
                    }
                    else
                    {
                        SavedCursorPosition = Application.GetCursorPosition(false);
                    }
                    Application.SetCursorHidden(CaptureMouse);
                }
            }
        }

        public void SetMouseCentered(bool centered)
        {
            CenterMouse = centered;
            if (CaptureMouse)
            {
                Application.SetCursorHidden(false);
                Int2 screenSize = Application.GetScreenSize();
                Application.SetCursorPosition(new Float2(screenSize.X, screenSize.Y) * 0.5f);
                Application.SetCursorHidden(true);
            }
        }

        public virtual bool Changed()
        {
            return MovementRawDelta.Length() > Constants.Epsilon
                   || RotationDelta.Length() > Constants.Epsilon
                   || ScaleDelta.Length() > Constants.Epsilon
                   || Math.Abs(FovDelta) > Constants.Epsilon;
        }

        // Clear selected entities
        public void ClearSelection()
        {
            ControlledEntities.Clear();
        }

        private bool CheckKeyInput(RyneKey key, InputType type)
        {
            switch (type)
            {
                case InputType.Pressed: return Application.KeyPressed((int)key);
                case InputType.Released: return Application.KeyReleased((int)key);
                case InputType.Down: return Application.KeyDown((int)key);
                default: return false;
            }
        }

        private bool CheckMouseInput(RyneMouse key, InputType type)
        {
            switch (type)
            {
                case InputType.Pressed: return Application.MousePressed((int)key);
                case InputType.Released: return Application.MouseReleased((int)key);
                case InputType.Down: return Application.MouseDown((int)key);
                default: return false;
            }
        }

        private bool CheckScrollInput(RyneMouseWheel key)
        {
            return Application.MouseWheelScroll((int)key);
        }
    }
}
