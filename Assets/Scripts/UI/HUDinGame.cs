using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class HUDinGame : MonoBehaviour
{
    [Label] public HUDPetal[] petals;

    void Start()
    {
        GameplaySystem.CurrentPlayer.Avatar.onDamaged.AddListener((delta) => OnHealthChange());
        GameplaySystem.CurrentPlayer.Avatar.onHealed.AddListener((delta) => OnHealthChange());
        OnHealthChange();
    }

    void OnHealthChange()
    {
        float h = GameplaySystem.CurrentPlayer.Avatar.Health;
        int hMax = Mathf.FloorToInt(h);

        for (int i = 0; i < petals.Length; ++i)
        {
            if (i < hMax && !petals[i].isOn) petals[i].TurnOn();
            else if (i >= hMax && petals[i].isOn) petals[i].TurnOff();
        }
    }


}
