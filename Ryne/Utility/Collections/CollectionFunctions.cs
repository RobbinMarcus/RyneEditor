using System.Collections.Generic;

namespace Ryne.Utility.Collections
{
    public class CollectionFunctions
    {
        public static void Swap<T>(int index1, int index2, T[] array)
        {
            T item = array[index1];
            array[index1] = array[index2];
            array[index2] = item;
        }

        public static void Swap<T>(int index1, int index2, List<T> list)
        {
            T item = list[index1];
            list[index1] = list[index2];
            list[index2] = item;
        }
    }
}
