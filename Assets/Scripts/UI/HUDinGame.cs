using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.UI;

public class HUDinGame : MonoBehaviour
{
    [Label] public HUDPetal[] petals;
    [Label] public UIFading journalNotificaition;
    [Label] public GameObject[] crystals;

    void Start()
    {
        GameplaySystem.CurrentPlayer.Avatar.onDamaged.AddListener((delta) => OnHealthChange());
        GameplaySystem.CurrentPlayer.Avatar.onHealed.AddListener((delta) => OnHealthChange());
        OnHealthChange();
        GameplaySystem.onJournalUnlock += OnJournalUnlock;
        GameplaySystem.onCrystalHad += OnCrystalGot;
        OnCrystalGot();
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

    void OnDestroy()
    {
        GameplaySystem.onJournalUnlock -= OnJournalUnlock;
    }

    void OnJournalUnlock()
    {
        journalNotificaition?.Fadein();
    }

    void OnCrystalGot()
    {
        for (int i = 0; i < 3; ++i)
        {
            if (GameplaySystem.crystalsHad[i]) crystals[i].SetActive(true);
        }
    }
}
