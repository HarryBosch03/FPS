using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBundle : MonoBehaviour
{
    [SerializeField] float duration;

    private void Awake()
    {
        Destroy(gameObject, duration);
    }
}
