using System;

namespace mc2.managers {
    [AttributeUsage(AttributeTargets.Class)]
    internal class DontLoadOnStatupAttribute : Attribute {
    }
}