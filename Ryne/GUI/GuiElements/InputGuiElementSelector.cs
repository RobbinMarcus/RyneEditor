using System;

namespace Ryne.Gui.GuiElements
{
    static class InputGuiElementSelector
    {
        public static InputGuiElement SelectForType(Type type, string name, object defaultValue)
        {
            InputGuiElement element = null;
            if (type == typeof(float)) element = new InputFloatGuiElement(name, (float)defaultValue);
            if (type == typeof(int)) element = new InputIntGuiElement(name, (int)defaultValue);
            if (type == typeof(Float3)) element = new InputFloat3GuiElement(name, defaultValue is Float3 float3 ? float3 : new Float3(0));
            if (type == typeof(Float4)) element = new InputFloat4GuiElement(name, defaultValue is Float4 float4 ? float4 : new Float4(0));
            if (type == typeof(string)) element = new InputTextGuiElement(name);
            return element;
        }

        public static InputGuiElement SliderForType(Type type, string name, object minValue, object maxValue, object defaultValue)
        {
            InputGuiElement element = null;
            if (type == typeof(float)) element = new InputFloatSliderGuiElement(name, (float)minValue, (float)maxValue, (float)defaultValue);
            return element;
        }
    }
}
