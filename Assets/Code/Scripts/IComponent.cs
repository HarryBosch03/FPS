using UnityEngine;

namespace Code.Scripts
{
    public interface IComponent
    {
        string name { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}