using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalButtonActivator : MonoBehaviour
{
    void OnEnable()
    {
        bool[] Unlocked = GameSystem.GameplaySystem.journalUnlocked;
        bool res = false;
        for (int i = 0; i < Unlocked.Length; ++i)
        {
            if (Unlocked[i]) res = true;
        }
        GetComponent<Button>().interactable = res;
    }
}
