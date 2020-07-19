using System;

namespace Ryne.Gui.GuiElements
{
    class InputFloatSliderGuiElement : InputGuiElement
    {
        public float FloatValue;
        public float MinValue;
        public float MaxValue;

        public override ValueType Value => FloatValue;

        public InputFloatSliderGuiElement(string label, float minValue, float maxValue, float defaultValue = 0.0f) : base(label)
        {
            FloatValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override void RenderContents()
        {
            if (Window.Gui.InputSliderFloat(Label, ref FloatValue, MinValue, MaxValue))
            {
                Changed = true;
            }
            base.RenderContents();
        }
    }
}