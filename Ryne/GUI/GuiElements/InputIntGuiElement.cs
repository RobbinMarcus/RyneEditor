using System;

namespace Ryne.Gui.GuiElements
{
    class InputIntGuiElement : InputGuiElement
    {
        public int IntValue;

        public InputIntGuiElement(string label, int defaultValue = 0) : base(label)
        {
            IntValue = defaultValue;
        }

        public override ValueType Value => IntValue;

        public override void RenderContents()
        {
            if (Window.Gui.InputInt(Label, ref IntValue))
            {
                Changed = true;
            }
            base.RenderContents();
        }
    }
}
