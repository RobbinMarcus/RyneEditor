using System.Collections.Generic;

namespace Ryne.Utility.Collections
{
    public static class ListExtensionFunctions
    {
        public static void RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static bool Empty<T>(this List<T> list)
        {
            return list.Count == 0;
        }
    }
}
