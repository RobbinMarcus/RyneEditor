using System;

namespace Ryne.Gui.GuiElements
{
    class InputFloatGuiElement : InputGuiElement
    {
        public float FloatValue;

        public InputFloatGuiElement(string label, float defaultValue = 0.0f) : base(label)
        {
            FloatValue = defaultValue;
        }

        public override ValueType Value => FloatValue;

        public override void RenderContents()
        {
            if (Window.Gui.InputFloat(Label, ref FloatValue))
            {
                Changed = true;
            }
            base.RenderContents();
        }
    }
}
