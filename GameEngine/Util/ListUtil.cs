
using System;
using System.Collections.Generic;

namespace GameEngine.Util
{
    public struct Pair<T, V>
    {
        public T First;
        public V Second;

        public Pair(T first, V second)
        {
            First = first;
            Second = second;
        }
    }

    public static class ListUtil
    {
        public static Pair<T, float> GetMinOrDefault<T>(this IEnumerable<T> collection, Func<T, float> value, T def = default(T))
        {
            Pair<T, float> minValue = new Pair<T, float>(def, float.PositiveInfinity);
            foreach (T item in collection)
            {
                float val = value(item);
                if (val < minValue.Second)
                {
                    minValue.First = item;
                    minValue.Second = val;
                }
            }

            return minValue;
        }

        public static Pair<T, float> GetMaxOrDefault<T>(this IEnumerable<T> collection, Func<T, float> value, T def = default(T))
        {
            Pair<T, float> maxValue = new Pair<T, float>(def, float.NegativeInfinity);
            foreach (T item in collection)
            {
                float val = value(item);
                if (val > maxValue.Second)
                {
                    maxValue.First = item;
                    maxValue.Second = val;
                }
            }

            return maxValue;
        }
    }
}
