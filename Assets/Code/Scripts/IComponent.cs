using UnityEngine;
// ReSharper disable InconsistentNaming
// #FuckUnity

namespace Code.Scripts
{
    public interface IComponent
    {
        string name { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}
