using System;

namespace Ryne.Gui.GuiElements
{
    class InputFloat4GuiElement : InputGuiElement
    {
        public Float4 Float4Value;

        public InputFloat4GuiElement(string label, Float4 defaultValue) : base(label)
        {
            Float4Value = defaultValue;
        }

        public override ValueType Value => Float4Value;

        public override unsafe void RenderContents()
        {
            fixed (Float4* float4 = &Float4Value)
            {
                if (Window.Gui.InputFloat4(Label, float4))
                {
                    Changed = true;
                }
            }

            base.RenderContents();
        }
    }
}
