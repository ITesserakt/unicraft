using UnityEngine;

namespace mc2.mod
{
    public interface IItem
    {
        int Id { get; }
        string ShortName { get; }
        string FullName { get; }
    }
}
