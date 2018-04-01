using System;

namespace mc2.mod
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Mod : Attribute
    {
        internal string dependencies;

        internal string modid;
        internal string name;
        internal string version;

        public Mod(string modid, string name = "", string version = "0.1", string dependencies = "")
        {
            this.modid = modid;
            this.dependencies = dependencies;
            this.name = name;
            this.version = version;
        }
    }
}