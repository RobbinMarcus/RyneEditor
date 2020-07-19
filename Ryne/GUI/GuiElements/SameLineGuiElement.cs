using Ryne.Gui.Windows;

namespace Ryne.Gui.GuiElements
{
    class SameLineGuiElement : IGuiElement
    {
        public string Label { get; }
        public WindowGui Window { get; set; }
        public void RenderContents()
        {
            Window.Gui.SameLine();
        }
    }
}
