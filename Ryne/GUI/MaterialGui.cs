using System;
using Ryne.Gui.GuiElements;
using Ryne.Scene;
using Ryne.Utility;

namespace Ryne.Gui
{
    class MaterialGui : WindowGui
    {
        public Material Material { get; set; }
        private readonly WindowDelegate OnMaterialChanged;

        public override object GetWindowObject => Material;

        public MaterialGui(ImGuiWrapper gui, Material material, WindowDelegate onMaterialChanged) : base(gui, "Edit " + material.Name)
        {
            Material = material;
            OnMaterialChanged = onMaterialChanged;
            ItemWidth = 100.0f;
            AddProperties();
        }

        /// <summary>
        /// Add all public properties of an entity to edit
        /// Currently pretty slow due to Reflection
        /// </summary>
        private void AddProperties()
        {
            var fields = Material.GetType().GetFields();
            foreach (var field in fields)
            {
                var name = field.Name;
                var value = field.GetValue(Material);

                // Local function callback
                void OnValueChanged(ValueType newValue)
                {
                    object boxed = Material;
                    field.SetValue(boxed, newValue);
                    Material = (Material) boxed;
                    OnMaterialChanged?.Invoke(this);
                }

                InputGuiElement element = null;
                if (field.FieldType == typeof(float)) element = new InputFloatGuiElement(name, (float)value);
                if (field.FieldType == typeof(int)) element = new InputIntGuiElement(name, (int)value);
                if (field.FieldType == typeof(Float3)) element = new InputFloat3GuiElement(name, (Float3)value);

                if (element != null)
                {
                    element.OnValueChangedCallback += OnValueChanged;
                    AddElement(element);
                }
                else
                {
                    Logger.Warning("MaterialGui.AddProperties: element of unknown type found");
                }
            }
        }
    }
}
