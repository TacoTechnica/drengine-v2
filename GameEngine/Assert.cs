using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class Assert
    {
        private const string DEFAULT_MESSAGE = "Assertion Failed:";
        public static bool IsTrue(bool statement, string message=null)
        {
            if (!statement)
            {
                Debug.LogError(GetMessage(message, "Expected True"));
            }
            return statement;
        }
        public static bool IsFalse(bool statement, string message = null)
        {
            return IsTrue(!statement, GetMessage(message, "Expected False"));
        }
        public static bool AreEqual(object a, object b, string message=null)
        {
            return IsTrue(a == b, GetMessage(message, $"{a} == {b}"));
        }
        public static bool IsInstanceOf<T>(object obj, string message = null)
        {
            return IsTrue(obj is T, GetMessage(message, $"{obj} of type {obj.GetType()} is type of {typeof(T)}"));
        }

        public static bool IsInstanceOf(object obj, Type t, string message = null)
        {
            return IsTrue(t.IsInstanceOfType(obj),
                GetMessage(message, $"{obj} of type {obj.GetType()} is type of {t}"));
        }

        public static bool IsNotNull(object obj, string message = null)
        {
            return IsTrue(obj != null, "Passed object is not null.");
        }
        public static bool ContainsKey<TK, TV>(Dictionary<TK, TV> dict, TK key)
        {
            return IsTrue(dict.ContainsKey(key), $"Dictionary {dict} does not contain key {key}");
        }
        private static string GetMessage(string message, string statement)
        {
            return message ?? $"{DEFAULT_MESSAGE} {statement}";
        }


    }
}
