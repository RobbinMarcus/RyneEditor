using System.Runtime.InteropServices;
using Ryne.Entities;
using Ryne.Gui;
using Ryne.Utility.Math;
using MessagePack;

namespace Ryne.Scene.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TransformComponent
    {
        public Float4 Position;
        public Float4 Pivot;
        public Float4 Scale;
        public Quaternion Rotation;

        // TODO: this is not necessary for TransformComponent functionality, define it only where we need it.
        [IgnoreMember]
        public Float4 PreviousPosition;

        // Location of pivot
        public Float4 GetLocation()
        {
            return Position + Pivot;
        }

        public void SetLocation(Float4 newLocation)
        {
            newLocation -= Pivot;
            Position = newLocation;
        }

        public Float4 ToWorldSpace(Float4 location)
        {
            location *= Scale;
            location = Rotation.RotateVectorAroundPivot(location, Pivot);
            location += Position;
            return location;
        }

        public RyneTransformComponent ToRenderComponent()
        {
            RyneTransformComponent result = new RyneTransformComponent
            {
                Position = Position,
                RelativePivot = Pivot,
                Scale = Scale,
                Rotation = Rotation.ToFloat4()
            };
            return result;
        }

        public unsafe void RenderGui(ImGuiWrapper gui, Entity owner)
        {
            bool changed = false;

            if (gui.CollapsingHeader("Transform component", true))
            {
                var position = new Float3(Position);
                if (gui.InputFloat3("Position", &position))
                {
                    if ((new Float3(Position)- position).Length() > Constants.Epsilon)
                    {
                        Position = new Float4(position);
                        changed = true;
                    }
                }

                var pivot = new Float3(Pivot);
                if (gui.InputFloat3("Pivot", &pivot))
                {
                    if ((new Float3(Pivot) - pivot).Length() > Constants.Epsilon)
                    {
                        Pivot = new Float4(pivot);
                        changed = true;
                    }
                }

                var scale = new Float3(Scale);
                if (gui.InputFloat3("Scale", &scale))
                {
                    if ((new Float3(Scale) - scale).Length() > Constants.Epsilon)
                    {
                        Scale = new Float4(scale);
                        changed = true;
                    }
                }

                var rotation = Rotation.ToRotator().ToFloat3();
                if (gui.InputFloat3("Rotation", &rotation))
                {
                    var oldRotation = Rotation.ToRotator().ToFloat3();
                    var newRotation = new Rotator(rotation).ToQuaternion().ToRotator().ToFloat3();
                    if ((oldRotation - newRotation).Length() > Constants.Epsilon)
                    {
                        Rotation = new Rotator(rotation).ToQuaternion();
                        changed = true;
                    }
                }
            }

            if (changed && gui is SceneEditorGui sceneGui)
            {
                if (owner.RenderId > -1)
                {
                    sceneGui.SceneData.UpdateTransform(owner.RenderId, ToRenderComponent());
                }
            }
        }

        public void SetDefaults()
        {
            Position = new Float4(0.0f);
            Pivot = new Float4(0.5f);
            Scale = new Float4(1.0f);
            Rotation = Quaternion.Default;
        }

        public void PostDeserialize()
        {

        }
    }
}
