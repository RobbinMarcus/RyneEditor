using System;

namespace Ryne.Gui.GuiElements
{
    class TextGuiElement : IGuiElement
    {
        public TextGuiElement(string label)
        {
            Label = label;
        }

        public bool IsInput => false;
        public string Label { get; }
        public ValueType Value => 0;
        public WindowGui Window { get; set; }

        public void RenderContents()
        {
            Window.Gui.Text(Label);
        }
    }
}
