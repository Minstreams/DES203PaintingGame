using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class HUDPetal : MonoBehaviour
{
    [Label] public GameObject flower;
    [Label] public SimpleEvent onEvent;
    [Label] public SimpleEvent offEvent;
    public bool isOn = false;
    [ContextMenu("Turn On")]
    public void TurnOn()
    {
        isOn = true;
        flower.SetActive(true);
        onEvent?.Invoke();
    }
    [ContextMenu("Turn Off")]
    public void TurnOff()
    {
        isOn = false;
        flower.SetActive(false);
        offEvent?.Invoke();
    }
}
