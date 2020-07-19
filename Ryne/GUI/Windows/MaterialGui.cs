using Ryne.Gui.GuiElements;
using Ryne.Scene;
using Ryne.Utility;
using System;
using System.Linq;

namespace Ryne.Gui.Windows
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
                // Skip name
                if (field.FieldType == typeof(string)) continue;

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

                InputGuiElement element = InputGuiElementSelector.SelectForType(field.FieldType, name, value);

                // Check if we can create a slider
                if (field.FieldType == typeof(float))
                {
                    var attributes = field.GetCustomAttributes(typeof(RangeAttribute), false);
                    if (attributes.Any())
                    {
                        var range = (RangeAttribute)attributes[0];
                        element = InputGuiElementSelector.SliderForType(field.FieldType, name, range.MinValue, range.MaxValue, value);
                    }
                }

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
