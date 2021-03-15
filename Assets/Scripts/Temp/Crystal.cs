using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class Crystal : GInteractable
{
    public enum CrystalType
    {
        Pink,
        Red,
        Green,
        Yellow,
    }
    public CrystalType ctype;
    public override void Interact()
    {
        base.Interact();
        gameObject.SetActive(false);
        switch (ctype)
        {
            case CrystalType.Pink:
                GameplaySystem.Setting.hasPinkCrystal = true;
                break;
            case CrystalType.Red:
                GameplaySystem.Setting.hasRedCrystal = true;
                break;
            case CrystalType.Green:
                GameplaySystem.Setting.hasGreenCrystal = true;
                break;
            case CrystalType.Yellow:
                GameplaySystem.Setting.hasYellowCrystal = true;
                break;
        }
    }
}
