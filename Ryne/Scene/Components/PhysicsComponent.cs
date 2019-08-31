using System.Runtime.InteropServices;
using Ryne.Entities;
using Ryne.Gui;
using MessagePack;

namespace Ryne.Scene.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PhysicsComponent
    {
        public Float4 Velocity;
        public Float4 Acceleration;

        // TODO: Don't apply some forces when we're on a surface
        [IgnoreMember]
        public bool OnSurface;

        public unsafe void RenderGui(ImGuiWrapper gui, Entity owner)
        {
            if (gui.CollapsingHeader("Physics component", true))
            {
                var velocity = new Float3(Velocity);
                if (gui.InputFloat3("Velocity", &velocity))
                {
                }

                var acceleration = new Float3(Acceleration);
                if (gui.InputFloat3("Acceleration", &acceleration))
                {
                }

                gui.InputCheckBox("OnSurface", ref OnSurface);
            }
        }

        public void PostDeserialize()
        {

        }
    }
}
