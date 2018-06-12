using System;

namespace mc2.general {
    public static class RightAssigmentExtensions {

        public static object ChangeType(this object o, Type t) =>
            o == null || t.IsValueType || o is IConvertible ? Convert.ChangeType(o, t) : o;

        public static T To<T>(this T o) => o;

        public static T To<T>(this T o, out T x) => x = o;

        public static T To<T>(this object o) => (T) o.ChangeType(typeof(T));
        
        public static T To<T>(this object o, out T x) => x = (T) o.ChangeType(typeof(T));


        public static TA Put<T, TA>(this T o, TA x) => x;

        public static TA Put<T, TA>(this T o, ref TA x) => x;
        
        
        public static bool[] Check<T>(this T o, params bool[] pattern) => pattern;


        public static T Call<T>(this T o, Action<T> action) {
            action(o);
            return o;
        }
        
        public static T Use<T, TR>(this T o, Func<TR> func) => func().Put(o);
        
        public static T Use<T, TR>(this T o, Func<T, TR> func) => func(o).Put(o);
        
        public static T Use<T, TA>(this T o, out TA x, TA a = default(TA)) => (x = a).Put(o);
    }
}