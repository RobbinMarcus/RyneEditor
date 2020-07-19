using Ryne.Gui.Windows;

namespace Ryne.Gui
{
    class StringInputGui : WindowGui
    {
        private readonly RyneImGuiTextInput TextInput;

        // File with path
        public string Result { get; protected set; }

        public StringInputGui(ImGuiWrapper gui, string windowTitle = "Input", int bufferSize = 260) : base(gui, windowTitle)
        {
            TextInput = new RyneImGuiTextInput(windowTitle, bufferSize);
        }

        public override void RenderContents()
        {
            if (TextInput.InputText())
            {
            }

            if (Gui.MenuItem("Ok"))
            {
                Result = TextInput.GetBuffer();
                Active = false;
                ExecuteCallback = true;
            }
        }
    }
}
