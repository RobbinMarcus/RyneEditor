using System;
using System.Collections.Generic;
using Ryne.Gui.GuiElements;
using Ryne.Utility;

namespace Ryne.Gui
{
    public delegate void WindowDelegate(WindowGui result);

    public class WindowGui
    {
        public ImGuiWrapper Gui { get; }

        public virtual object GetWindowObject => null;

        private readonly List<IGuiElement> GuiElements;

        public bool Active { get; set; }

        // Condition to execute callback when the window is no longer active
        public bool ExecuteCallback { get; set; }

        public string WindowTitle { get; }

        // Called when the window closes and ExecuteCallback is true
        public WindowDelegate Callback { get; set; }

        public float MaxWidth;
        public float MaxHeight;
        public float MinWidth;
        public float MinHeight;

        protected float ItemWidth;

        protected bool FirstRender;
        protected bool SetSizeConstraints;

        public WindowGui(ImGuiWrapper gui, string windowTitle)
        {
            Gui = gui;
            GuiElements = new List<IGuiElement>();

            WindowTitle = windowTitle;
            Active = true;
            ExecuteCallback = false;

            MaxWidth = Global.Config.Width - 100;
            MaxHeight = Global.Config.Height - 100;
            MinWidth = 100.0f;
            MinHeight = 50.0f;

            ItemWidth = 0.0f;

            FirstRender = true;
            SetSizeConstraints = true;
        }

        public virtual void BeginWindow()
        {
            if (SetSizeConstraints)
            {
                //Gui.SetNextWindowPosCenter(RyneImGuiCondition.Once);
                Gui.SetNextWindowSizeConstraints(new Float2(MinWidth, MinHeight), new Float2(MaxWidth, MaxHeight));
                //Gui.SetNextWindowSize(new Float2(0.0f, 0.0f), RyneImGuiCondition.Once);
            }

            bool active = true;
            Gui.Begin(WindowTitle, ref active);
            Active = active;
        }

        public virtual void EndWindow()
        {
            Gui.End();

            if (FirstRender)
            {
                FirstRender = false;
            }
        }

        public virtual void OnClose()
        {
            if (ExecuteCallback)
            {
                Callback?.Invoke(this);
            }
        }

        public virtual void RenderContents()
        {
            if (Active)
            {
                if (ItemWidth > 0.0f)
                {
                    Gui.PushItemWidth(ItemWidth);
                }

                foreach (var element in GuiElements)
                {
                    element.RenderContents();
                }

                if (ItemWidth > 0.0f)
                {
                    Gui.PopItemWidth();
                }
            }
        }

        public Dictionary<string, ValueType> GetElementValues()
        {
            Dictionary<string, ValueType> result = new Dictionary<string, ValueType>();
            foreach (var element in GuiElements)
            {
                if (element is InputGuiElement input)
                {
                    if (input is InputTextGuiElement)
                    {
                        continue;
                    }
                    result.Add(input.Label, input.Value);
                }
            }

            return result;
        }

        public Dictionary<string, string> GetTextInputValues()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var element in GuiElements)
            {
                if (element is InputTextGuiElement input)
                {
                    result.Add(input.Label, input.Text);
                }
            }

            return result;
        }

        public void AddElement(IGuiElement element)
        {
            element.Window = this;
            GuiElements.Add(element);
        }
    }
}
