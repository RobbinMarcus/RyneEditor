using System;

namespace Ryne.Gui.GuiElements
{
    class InputFloat2GuiElement : InputGuiElement
    {
        public Float2 Float2Value;

        public InputFloat2GuiElement(string label, Float2 defaultValue) : base(label)
        {
            Float2Value = defaultValue;
        }

        public override ValueType Value => Float2Value;

        public override unsafe void RenderContents()
        {
            fixed (Float2* float2 = &Float2Value)
            {
                if (Window.Gui.InputFloat2(Label, float2))
                {
                    Changed = true;
                }
            }

            base.RenderContents();
        }
    }
}
