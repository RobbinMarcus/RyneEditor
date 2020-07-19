using Ryne.Gui.Windows;

namespace Ryne.Gui
{
    public class PopupGui : WindowGui
    {
        public bool PopupActive { get; private set; }

        private readonly bool RenderOkButton;
        private readonly bool RenderCancelButton;
        private readonly bool IsModal;

        public PopupGui(ImGuiWrapper gui, string windowTitle, bool okButton = true, bool cancelButton = true, bool isModal = true) : base(gui, windowTitle)
        {
            PopupActive = false;
            RenderOkButton = okButton;
            RenderCancelButton = cancelButton;
            IsModal = isModal;
        }

        public override void BeginWindow()
        {
            Gui.SetNextWindowSizeConstraints(new Float2(MinWidth, MinHeight), new Float2(MaxWidth, MaxHeight));
            if (!IsModal)
            {
                Gui.SetNextWindowPosCenter(RyneImGuiCondition.Always);
            }

            PopupActive = IsModal ? Gui.BeginPopupModal(WindowTitle) : Gui.BeginPopup(WindowTitle);
        }

        public override void RenderContents()
        {
            if (PopupActive)
            {
                if (!IsModal)
                {
                    Gui.Text(WindowTitle);
                }

                if (FirstRender)
                {
                    Gui.SetKeyboardFocusHere(0);
                }

                base.RenderContents();

                if (RenderOkButton)
                {
                    if (Gui.Button("Ok"))
                    {
                        Active = false;
                        ExecuteCallback = true;
                    }
                    Gui.SameLine();
                }

                if (RenderCancelButton)
                {
                    if (Gui.Button("Cancel"))
                    {
                        Active = false;
                        ExecuteCallback = false;
                    }
                }
            }
        }

        public override void EndWindow()
        {
            if (PopupActive)
            {
                Gui.EndPopup();

                if (FirstRender)
                {
                    FirstRender = false;
                }
            }
        }
    }
}
