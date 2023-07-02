using System;
using System.Collections.Generic;

namespace SKTools.EnumComparer
{
    public static class Extensions
    {
        public static bool Contains<T>(this T[] array, T value, IEqualityComparer<T> equalityComparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            return Extensions.IndexOf<T>(array, value, 0, array.Length, equalityComparer) >= 0;
        }

        public static int IndexOf<T>(this T[] array, T value, IEqualityComparer<T> equalityComparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            return Extensions.IndexOf<T>(array, value, 0, array.Length, equalityComparer);
        }

        public static int IndexOf<T>(this T[] array, T value, int startIndex, int count,
            IEqualityComparer<T> equalityComparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (count < 0 || startIndex < array.GetLowerBound(0) || startIndex - 1 > array.GetUpperBound(0) - count)
            {
                throw new ArgumentOutOfRangeException();
            }

            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (equalityComparer.Equals(array[i], value))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}