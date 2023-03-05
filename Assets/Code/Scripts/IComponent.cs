using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent
{
    string name { get; }
    GameObject gameObject { get; }
    Transform transform { get; }
}
