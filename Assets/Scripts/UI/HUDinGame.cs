using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.UI;

public class HUDinGame : MonoBehaviour
{
    [Label] public HUDPetal[] petals;
    [Label] public UIFading journalNotificaition;
    [Label] public HUDPetal[] crystals;

    void Awake()
    {
        GameplaySystem.onJournalUnlock += OnJournalUnlock;
        GameplaySystem.onCrystalHad += OnCrystalGot;
    }

    void Start()
    {
        GameplaySystem.CurrentPlayer.Avatar.onDamaged.AddListener((delta) => OnHealthChange());
        GameplaySystem.CurrentPlayer.Avatar.onHealed.AddListener((delta) => OnHealthChange());
        OnHealthChange();
        OnCrystalGot();
    }

    void OnHealthChange()
    {
        float h = GameplaySystem.CurrentPlayer.Avatar.Health;
        int hMax = Mathf.FloorToInt(h);

        StartCoroutine(UpdateHealth(hMax));
    }
    IEnumerator UpdateHealth(int hMax)
    {
        for (int i = 0; i < petals.Length; ++i)
        {
            if (i < hMax && !petals[i].isOn)
            {
                petals[i].TurnOn();
                yield return new WaitForSeconds(0.2f);
            }
            else if (i >= hMax && petals[i].isOn) petals[i].TurnOff();
        }
        yield return 0;
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
        StartCoroutine(UpdateCrystal());
    }

    IEnumerator UpdateCrystal()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 3; ++i)
        {
            if (GameplaySystem.crystalsHad[i] && !crystals[i].isOn)
            {
                crystals[i].TurnOn();
                yield return new WaitForSeconds(0.5f);
            }
        }
        yield return 0;
    }
}
