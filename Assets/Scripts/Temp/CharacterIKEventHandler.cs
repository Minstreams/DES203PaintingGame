using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIKEventHandler : MonoBehaviour
{
    public int LayerIndex { get; set; }
    public event System.Action animatorIKHandler;

    void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex != LayerIndex) return;
        animatorIKHandler?.Invoke();
    }
    void Invoke()
    {
        // Do nothing
    }
}
