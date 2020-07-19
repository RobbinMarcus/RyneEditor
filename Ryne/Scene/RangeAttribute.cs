using System;

namespace Ryne.Scene
{
    internal class RangeAttribute : Attribute
    {
        public float MinValue;
        public float MaxValue;

        public RangeAttribute(float min, float max)
        {
            MinValue = min;
            MaxValue = max;
        }
    }
}