using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon : IComponent
{
    bool Shoot { get; set; }
}
