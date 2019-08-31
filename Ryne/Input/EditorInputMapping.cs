using System.IO;
using Ryne.GameStates;
using Ryne.Gui;
using Ryne.Utility.Actions;

namespace Ryne.Input
{
    class EditorInputMapping : InputMapping
    {
        public EditorInputMapping(SceneEditor editor, float moveSpeed = 10.0f, float mouseSensitivity = 10.0f)
        {
            // Movement controls
            AddInput(new KeyInput { Key = RyneKey.W, Type = InputType.Down, Action = new InputMovementAction(new Float3(1, 0, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.S, Type = InputType.Down, Action = new InputMovementAction(new Float3(-1, 0, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.A, Type = InputType.Down, Action = new InputMovementAction(new Float3(0, -1, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.D, Type = InputType.Down, Action = new InputMovementAction(new Float3(0, 1, 0), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.Q, Type = InputType.Down, Action = new InputMovementAction(new Float3(0, 0, -1), moveSpeed) });
            AddInput(new KeyInput { Key = RyneKey.E, Type = InputType.Down, Action = new InputMovementAction(new Float3(0, 0, 1), moveSpeed) });

            // View controls
            AddInput(new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputRotationAction(new Float3(1, 0, 0), mouseSensitivity) });
            AddInput(new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputRotationAction(new Float3(0, 1, 0), mouseSensitivity, true) });

            // FOV controls
            AddInput(new KeyInput { Key = RyneKey.C, Type = InputType.Down, Action = new FovInputAction(-10.0f) });
            AddInput(new KeyInput { Key = RyneKey.Z, Type = InputType.Down, Action = new FovInputAction(10.0f) });

            // Edit mode shortcuts
            AddInput(new KeyInput { Key = RyneKey.T, Type = InputType.Released, Action = new InputDelegateAction(() => editor.ChangeEditMode(EditMode.Translate)) });
            AddInput(new KeyInput { Key = RyneKey.R, Type = InputType.Released, Action = new InputDelegateAction(() => editor.ChangeEditMode(EditMode.Rotate)) });
            AddInput(new KeyInput { Key = RyneKey.S, Type = InputType.Released, Action = new InputDelegateAction(() => editor.ChangeEditMode(EditMode.Scale)) });

            // Edit axis shortcuts
            AddInput(new KeyInput { Key = RyneKey.X, Type = InputType.Released, Action = new InputDelegateAction(() => editor.ChangeEditAxis(EditAxis.X)) });
            AddInput(new KeyInput { Key = RyneKey.Y, Type = InputType.Released, Action = new InputDelegateAction(() => editor.ChangeEditAxis(EditAxis.Y)) });
            AddInput(new KeyInput { Key = RyneKey.Z, Type = InputType.Released, Action = new InputDelegateAction(() => editor.ChangeEditAxis(EditAxis.Z)) });

            // Scroll wheel changes movement speed
            AddInput(new MouseScrollInput
            {
                Key = RyneMouseWheel.WheelUp,
                Action = new InputDelegateAction(() =>
                {
                    foreach (var keyInput in KeyInputs)
                    {
                        if (keyInput.Action.GetType() == typeof(InputMovementAction))
                        {
                            ((InputMovementAction)keyInput.Action).Length *= 1.1f;
                        }
                    }
                })
            });
            AddInput(new MouseScrollInput
            {
                Key = RyneMouseWheel.WheelDown,
                Action = new InputDelegateAction(() =>
                {
                    foreach (var keyInput in KeyInputs)
                    {
                        if (keyInput.Action.GetType() == typeof(InputMovementAction))
                        {
                            ((InputMovementAction)keyInput.Action).Length /= 1.1f;
                        }
                    }
                })
            });


            // Edit Gui hide
            AddInput(new KeyInput { Key = RyneKey.H, Type = InputType.Released, Action = new InputDelegateAction(editor.HideGui) });

            // Printscreen renders to image
            AddInput(new KeyInput { Key = RyneKey.PrintScreen, Type = InputType.Released
                , Action = new InputDelegateAction(() => editor.RenderScreenToImage("Renders/Render" + ++editor.Gui.RenderCount + ".tga")) });

            // Rename
            AddInput(new KeyInput { Key = RyneKey.F2, Type = InputType.Released, Action = new InputDelegateAction(editor.TryRenameEntity) });


            // Delete
            AddInput(new KeyInput { Key = RyneKey.Delete, Type = InputType.Pressed, Action = new InputDelegateAction(() => editor.DeleteActiveEntities()) });

            // Copy
            AddInput(new ComboKeyInput
            {
                Key1 = RyneKey.LeftControl, Key2 = RyneKey.C, Type1 = InputType.Down, Type2 = InputType.Pressed,
                Action = new InputDelegateAction(editor.CopySelectedEntities),
                ConsumeInput = true
            });
            // Paste
            AddInput(new ComboKeyInput
            {
                Key1 = RyneKey.LeftControl, Key2 = RyneKey.V, Type1 = InputType.Down, Type2 = InputType.Pressed,
                Action = new InputDelegateAction(editor.PasteSelectedEntities),
                ConsumeInput = true
            });
            // Undo
            AddInput(new ComboKeyInput
            {
                Key1 = RyneKey.LeftControl, Key2 = RyneKey.Z, Type1 = InputType.Down, Type2 = InputType.Pressed,
                Action = new InputDelegateAction(editor.UndoLastAction),
                ConsumeInput = true
            });
            // Save
            AddInput(new ComboKeyInput
            {
                Key1 = RyneKey.LeftControl, Key2 = RyneKey.S, Type1 = InputType.Down, Type2 = InputType.Pressed,
                Action = new InputDelegateAction(editor.SaveScene),
                ConsumeInput = true
            });
            // Open
            AddInput(new ComboKeyInput
            {
                Key1 = RyneKey.RightControl, Key2 = RyneKey.O, Type1 = InputType.Down, Type2 = InputType.Pressed,
                Action = new InputDelegateAction(() =>
                {
                    var window = new FileExplorerGui(editor.Gui, "Scenes", ".fls", "Load scene");
                    window.Callback += result =>
                    {
                        var fileName = ((FileExplorerGui)result).SelectedFile;
                        var sceneName = Path.GetFileNameWithoutExtension(fileName);
                        editor.LoadScene(sceneName);
                    };
                    editor.Gui.AddPopup(window);
                }),
                ConsumeInput = true
            });

            // Right mouse triggers the start and end of camera movement
            AddInput(new MouseInput
            {
                Key = RyneMouse.Button1,
                Type = InputType.Pressed,
                Action = new InputControllerAction(controller =>
                {
                    if (!editor.Gui.IsMouseInGui())
                        controller.SetCaptureMouse(true);
                })
            });
            AddInput(new MouseInput
            {
                Key = RyneMouse.Button1,
                Type = InputType.Released,
                Action = new InputControllerAction(controller => { controller.SetCaptureMouse(false); })
            });
        }
    }


    abstract class ObjectManipulationMapping : InputMapping
    {
        protected ObjectManipulationMapping(RyneCamera camera)
        {
            // Always have the mouse trigger the start and end of the manipulation
            AddInput(new MouseInput
            {
                Key = RyneMouse.Button0,
                Type = InputType.Pressed,
                Action = new InputControllerAction(controller =>
                {
                    if (controller is EditorGridController gridController)
                    {
                        gridController.StartEdit();
                    }
                    controller.SetCaptureMouse(true);
                })
            });
            AddInput(new MouseInput
            {
                Key = RyneMouse.Button0,
                Type = InputType.Released,
                Action = new InputControllerAction(controller =>
                {
                    if (controller is EditorGridController gridController)
                    {
                        gridController.EndEdit();
                    }
                    controller.SetCaptureMouse(false);
                })
            });
        }

        public abstract void UpdateMapping(RyneCamera camera, EditAxis axis);

        protected Float3 AxisToFloat3(EditAxis axis)
        {
            switch (axis)
            {
                case EditAxis.X: return new Float3(1, 0, 0);
                case EditAxis.Y: return new Float3(0, 1, 0);
                case EditAxis.Z: return new Float3(0, 0, 1);
                default: return new Float3(1);
            }
        }
    }

    class TranslationInputMapping : ObjectManipulationMapping
    {
        public float Speed { get; set; }

        public TranslationInputMapping(RyneCamera camera, EditAxis axis, float speed = 1.0f) : base(camera)
        {
            Speed = speed;

            AddInput(new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputMovementAction(new Float3(0.0f), Speed) });
            AddInput(new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputMovementAction(new Float3(0.0f), Speed) });
            UpdateMapping(camera, axis);
        }

        public sealed override void UpdateMapping(RyneCamera camera, EditAxis axis)
        {
            Float3 xAxis, yAxis;
            if (axis == EditAxis.All)
            {
                // Set axes from plane tangents relative to camera
                Float3 planeNormal = (camera.Focus - new Float3(camera.Position)).Normalize();
                xAxis = planeNormal.Cross(camera.Up).Normalize() * -1.0f;
                yAxis = xAxis.Cross(planeNormal).Normalize();
            }
            else
            {
                xAxis = AxisToFloat3(axis);
                yAxis = xAxis;
            }

            MouseAxisInputs[0] = new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputMovementAction(xAxis, Speed, false) };
            MouseAxisInputs[1] = new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputMovementAction(yAxis, Speed, false) };
        }
    }


    class RotationInputMapping : ObjectManipulationMapping
    {
        public float Speed { get; set; }

        public RotationInputMapping(RyneCamera camera, EditAxis axis, float speed = 50.0f) : base(camera)
        {
            Speed = speed;
            AddInput(new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputRotationAction(new Float3(1, 0, 0), Speed) });
            AddInput(new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputRotationAction(new Float3(0, 1, 0), Speed) });
        }

        public sealed override void UpdateMapping(RyneCamera camera, EditAxis axis)
        {
            Float3 xAxis, yAxis;
            if (axis == EditAxis.All)
            {
                xAxis = new Float3(1, 0, 0);
                yAxis = new Float3(0, 1, 0);
            }
            else
            {
                xAxis = AxisToFloat3(axis);
                yAxis = xAxis;
            }

            MouseAxisInputs[0] = new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputRotationAction(xAxis, Speed) };
            MouseAxisInputs[1] = new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputRotationAction(yAxis, Speed) };
        }
    }

    class ScaleInputMapping : ObjectManipulationMapping
    {
        public float Speed { get; set; }

        public ScaleInputMapping(RyneCamera camera, EditAxis axis, float speed = 1.0f) : base(camera)
        {
            Speed = speed;
            AddInput(new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputScalingAction(new Float3(1, 0, 0), Speed) });
            AddInput(new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputScalingAction(new Float3(0, 1, 0), Speed) });
        }

        public sealed override void UpdateMapping(RyneCamera camera, EditAxis axis)
        {
            Float3 xAxis, yAxis;
            if (axis == EditAxis.All)
            {
                xAxis = new Float3(1, 0, 0);
                yAxis = new Float3(0, 1, 0);
            }
            else
            {
                xAxis = AxisToFloat3(axis);
                yAxis = xAxis;
            }

            MouseAxisInputs[0] = new MouseAxisInput { Axis = new Float2(1, 0), Action = new InputScalingAction(xAxis, Speed) };
            MouseAxisInputs[1] = new MouseAxisInput { Axis = new Float2(0, 1), Action = new InputScalingAction(yAxis, Speed) };
        }
    }
}
