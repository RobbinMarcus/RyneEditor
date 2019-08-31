using System;

namespace Ryne.Gui.GuiElements
{
    class InputCheckboxGuiElement : InputGuiElement
    {
        public bool BoolValue;

        public InputCheckboxGuiElement(string label, bool defaultValue = false) : base(label)
        {
            BoolValue = defaultValue;
        }

        public override ValueType Value => BoolValue;

        public override void RenderContents()
        {
            if (Window.Gui.InputCheckBox(Label, ref BoolValue))
            {
                Changed = true;
            }
            base.RenderContents();
        }
    }
}
