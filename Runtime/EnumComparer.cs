using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SKTools
{
    /// <summary>
    /// No boxing, no heap allocations. Very fast. No need to write a specific comparer for each enum.
    /// bool contained = enumArray.Contains(MyEnum.someValue, EnumComparer<MyEnum>.Default);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EnumComparer<T> : IEqualityComparer<T>
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Transformer
        {
            [FieldOffset(0)] public T t;

            [FieldOffset(0)] public int int32;
        }

        public static EnumComparer<T> Default { get; } = new EnumComparer<T>();

        private EnumComparer()
        {
        }

        public bool Equals(T a, T b)
        {
            Transformer aTransformer = new Transformer {t = a};
            Transformer bTransformer = new Transformer {t = b};
            return aTransformer.int32 == bTransformer.int32;
        }

        public int GetHashCode(T value)
        {
            Transformer valueTransformer = new Transformer {t = value};
            return valueTransformer.int32.GetHashCode();
        }
    }
}