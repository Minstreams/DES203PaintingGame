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
        if (unlockOnStart) StartCoroutine(DelayUnlock(2));
    }
    IEnumerator DelayUnlock(float time)
    {
        yield return new WaitForSeconds(time);
        Unlock();
    }

    [ContextMenu("Journal")]
    public void Unlock()
    {
        if (journalIndex < 0) return;
        GameplaySystem.UnlockJournal(journalIndex);
    }
}
