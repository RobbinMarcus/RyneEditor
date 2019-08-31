namespace Ryne.Gui.GuiElements
{
    class CancelButtonGuiElement : ButtonGuiElement
    {
        public CancelButtonGuiElement()
            : base("Cancel", result =>
            {
                result.Active = false;
                result.ExecuteCallback = false;
            })
        {
        }
    }
}
