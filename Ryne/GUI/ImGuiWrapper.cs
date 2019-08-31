using System.Collections.Generic;
using Ryne.Utility;

namespace Ryne.Gui
{
    public class ImGuiWrapper : RyneImGuiWrapper
    {
        private readonly Dictionary<string, RyneImGuiFont> Fonts;

        private readonly RyneDelegate CustomFontsDelegate;
        private readonly RyneDelegate ApplyCustomStylesDelegate;

        public ImGuiWrapper() : base(Global.Application)
        {
            Fonts = new Dictionary<string, RyneImGuiFont>();

            CustomFontsDelegate = new RyneDelegate(LoadCustomFonts);
            ApplyCustomStylesDelegate = new RyneDelegate(ApplyCustomStyles);
            SetCustomFontEvent(CustomFontsDelegate);
            SetCustomStyleEvent(ApplyCustomStylesDelegate);
        }

        public void Destroy()
        {
            CustomFontsDelegate.Free();
            ApplyCustomStylesDelegate.Free();
        }

        public new virtual void Initialize(RyneSceneRenderData renderData)
        {
            base.Initialize(renderData);
        }

        public new virtual void Update(float dt)
        {
            base.Update(dt);
        }

        public new virtual void Draw(float dt)
        {
            base.Draw(dt);
        }

        public virtual void LoadCustomFonts()
        {
            AddFont("Roboto16", new RyneImGuiFont(Global.Config.WorkingDirectory + "Fonts/Roboto/Roboto-Regular.ttf", 16));
            AddFont("Consolas12", new RyneImGuiFont(Global.Config.WorkingDirectory + "Fonts/Consolas/consola.ttf", 12));
        }

        public virtual void ApplyCustomStyles()
        {
            RyneImGuiStyleEditor style = new RyneImGuiStyleEditor
            {
                WindowRounding = 0.0f,
                WindowBorderSize = 0.0f
            };
            ApplyCustomStyle(style);
        }

        public RyneImGuiFont GetFont(string name)
        {
            return Fonts[name];
        }

        /// <summary>
        /// Only call from LoadCustomFonts
        /// </summary>
        protected void AddFont(string name, RyneImGuiFont font)
        {
            Fonts.Add(name, font);
            AddCustomFont(Fonts[name]);
        }


        // Overrides for vector types
        public unsafe bool InputFloat2(string label, Float2 * value)
        {
            return InputFloat2(label, &value->X);
        }
        public unsafe bool InputFloat3(string label, Float3 * value)
        {
            return InputFloat3(label, &value->X);
        }
        public unsafe bool InputFloat4(string label, Float4 * value)
        {
            return InputFloat4(label, &value->X);
        }
    }
}
