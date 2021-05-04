using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class JournalUnlocker : MonoBehaviour
{
    [Label] public int journalIndex = -1;
    [Label] public bool unlockOnStart = true;

    void Start()
    {
        if (unlockOnStart) Unlock();
    }

    [ContextMenu("Journal")]
    public void Unlock()
    {
        if (journalIndex < 0) return;
        GameplaySystem.UnlockJournal(journalIndex);
    }
}
