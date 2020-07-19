using System;
using Ryne.Gui.Windows;

namespace Ryne.Gui.GuiElements
{
    delegate void InputChangedDelegate(ValueType newValue);

    abstract class InputGuiElement : IGuiElement
    {
        // Result of the editable value
        public abstract ValueType Value { get; }
        public string Label { get; }
        public WindowGui Window { get; set; }

        protected bool Changed;

        public InputChangedDelegate OnValueChangedCallback { get; set; }

        protected InputGuiElement(string label)
        {
            Label = label;
            Changed = false;
        }

        public virtual void RenderContents()
        {
            if (Changed & OnValueChangedCallback != null)
            {
                OnValueChangedCallback(Value);
                Changed = false;
            }
        }
    }
}
