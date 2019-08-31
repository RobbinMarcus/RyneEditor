namespace Ryne.Gui.GuiElements
{
    public interface IGuiElement
    {
        // Label of the value
        string Label { get; }

        // Reference to the window where this element is in
        WindowGui Window { get; set; }

        void RenderContents();
    }
}
