using System;

namespace Ryne.Gui.GuiElements
{
    class InputTextGuiElement : InputGuiElement
    {
        private readonly RyneImGuiTextInput TextInput;

        public string Text => TextInput.GetBuffer();

        public InputTextGuiElement(string label, int bufferSize = 260) : base(label)
        {
            TextInput = new RyneImGuiTextInput(label, bufferSize);
        }

        public override ValueType Value => 0;

        public override void RenderContents()
        {
            if (TextInput.InputText())
            {
            }
        }
    }
}
