namespace Ryne.Gui.GuiElements
{
    class OkButtonGuiElement : ButtonGuiElement
    {
        public OkButtonGuiElement() 
            : base("Ok", result => 
            {
                result.Active = false;
                result.ExecuteCallback = true;
            })
        {
        }
    }
}
