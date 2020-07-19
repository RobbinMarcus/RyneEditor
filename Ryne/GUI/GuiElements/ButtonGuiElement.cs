using System;
using Ryne.Gui.Windows;

namespace Ryne.Gui.GuiElements
{
    class ButtonGuiElement : IGuiElement
    {
        private readonly WindowDelegate Callback;

        public ButtonGuiElement(string label, WindowDelegate callback)
        {
            Label = label;
            Callback = callback;
        }

        public bool IsInput => false;
        public string Label { get; }
        public ValueType Value => 0;
        public WindowGui Window { get; set; }

        public void RenderContents()
        {
            if (Window.Gui.Button(Label))
            {
                Callback(Window);
            }
        }
    }
}
