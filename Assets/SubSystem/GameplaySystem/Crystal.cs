using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class Crystal : MonoBehaviour
{
    public enum CrystalType
    {
        Pink,
        Blue,
        Red,
    }
    [Label] public CrystalType ctype;
    [Label] public SimpleEvent onPick;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pick();
        }
    }
    [ContextMenu("Pick")]
    public void Pick()
    {
        onPick?.Invoke();
        switch (ctype)
        {
            case CrystalType.Pink:
                GameplaySystem.GetCrystal(0);
                break;
            case CrystalType.Blue:
                GameplaySystem.GetCrystal(1);
                break;
            case CrystalType.Red:
                GameplaySystem.GetCrystal(2);
                break;
        }
    }
}
