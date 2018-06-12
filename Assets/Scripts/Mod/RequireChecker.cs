using System;
using System.Reflection;

namespace mc2.mod
{
    [Obsolete("Don`t use it")]
    internal sealed class RequireChecker
    {
        internal static void Check(Type t, object objT)
        {
            var props = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

            for (var i = 0; i < props.Length; i++) {
                var prop = props[i];
                if (!Attribute.IsDefined(prop.GetType(), typeof(RequiredAttribute))) continue;
                if (prop.GetValue(objT) == null)
                    throw new ArgumentNullException(nameof(prop), $"{prop.Name} обозначен как обязательный, но имеет он не имеет значения");
            }
        }
    }

    [Obsolete("Don`t use it")]
    internal sealed class RequiredAttribute : Attribute
    {
        internal RequiredAttribute() { }
    }
}
