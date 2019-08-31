using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ryne.Utility
{
    public class Helpers
    {
        // Keep track of unique name count
        private static Dictionary<string, int> NameCounter;

        [StructLayout(LayoutKind.Explicit)]
        struct IntFloatUnion
        {
            [FieldOffset(0)] public int a;
            [FieldOffset(0)] public float b;
        }

        public static float IntAsFloat(int value)
        {
            IntFloatUnion u = new IntFloatUnion {a = value};
            return u.b;
        }

        public static int FloatAsInt(float value)
        {
            IntFloatUnion u = new IntFloatUnion { b = value };
            return u.a;
        }

        // Returns a new unique name by adding a counter to given baseName if it was used through this function before
        public static string GetUniqueName(string baseName)
        {
            if (NameCounter == null)
            {
                NameCounter = new Dictionary<string, int>();
            }

            if (baseName.Length == 0)
            {
                Logger.Error("GetUniqueName: empty name");
                baseName = "Empty";
            }

            // Remove any trailing digits
            var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            baseName = baseName.TrimEnd(digits);

            if (baseName.Length == 0)
            {
                Logger.Error("GetUniqueName: name only consisted of digits");
                baseName = "Empty";
            }

            if (!NameCounter.ContainsKey(baseName))
            {
                NameCounter.Add(baseName, 0);
            }

            int count = NameCounter[baseName]++;
            var name = baseName + (count > 0 ? count.ToString() : "");

            if (Global.EntityManager.Entities.Any(x => x.Name == name))
            {
                return GetUniqueName(baseName);
            }

            return name;
        }
    }
}
