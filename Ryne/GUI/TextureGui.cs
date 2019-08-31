using System;
using Ryne.Entities;
using Ryne.Gui.GuiElements;
using Ryne.Scene;

namespace Ryne.Gui
{
    class TextureGui : WindowGui
    {
        //private readonly Entity Entity;
        public RyneTextureInstance Texture { get; set; }
        private readonly WindowDelegate OnMaterialChanged;

        public override object GetWindowObject => Texture;

        public TextureGui(ImGuiWrapper gui, RyneTextureInstance texture, WindowDelegate onMaterialChanged) : base(gui, "Edit Texture")
        {
            Texture = texture;
            OnMaterialChanged = onMaterialChanged;

            AddProperties();
        }

        public override void RenderContents()
        {
            base.RenderContents();
        }

        /// <summary>
        /// Add all public properties of an entity to edit
        /// Currently pretty slow due to Reflection
        /// </summary>
        private void AddProperties()
        {
            //Texture.

                //// Local function callback
                //void OnValueChanged(ValueType newValue)
                //{
                //    object boxed = Material;
                //    field.SetValue(boxed, newValue);
                //    Material = (Material) boxed;
                //    OnMaterialChanged?.Invoke(this);
                //}

                //if (field.FieldType == typeof(float))
                //{
                //    var element = new InputFloatGuiElement(name, (float)value);
                //    element.OnValueChangedCallback += OnValueChanged;
                //    AddElement(element);
                //}
        }
    }
}
