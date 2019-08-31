using System;

namespace Ryne.Gui.GuiElements
{
    class InputFloat3GuiElement : InputGuiElement
    {
        public Float3 Float3Value;

        public InputFloat3GuiElement(string label, Float3 defaultValue) : base(label)
        {
            Float3Value = defaultValue;
        }

        public override ValueType Value => Float3Value;

        public override unsafe void RenderContents()
        {
            fixed (Float3* float3 = &Float3Value)
            {
                if (Window.Gui.InputFloat3(Label, float3))
                {
                    Changed = true;
                }
            }

            base.RenderContents();
        }
    }
}
