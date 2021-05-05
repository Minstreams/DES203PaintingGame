using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalPageDisplayer : MonoBehaviour
{
    Image pageImage;
    int pageIndex;
    Sprite[] Pages => GameSystem.GameplaySystem.Setting.journalPages;
    bool[] Unlocked => GameSystem.GameplaySystem.journalUnlocked;
    private void Awake()
    {
        pageImage = GetComponent<Image>();
    }
    void OnEnable()
    {
        for (int i = 0; i < Unlocked.Length; ++i)
        {
            if (Unlocked[i])
            {
                pageIndex = i;
                pageImage.sprite = Pages[i];
                break;
            }
        }
    }

    public void NextPage()
    {
        for (int i = pageIndex + 1; i < Unlocked.Length; ++i)
        {
            if (Unlocked[i])
            {
                pageIndex = i;
                pageImage.sprite = Pages[i];
                break;
            }
        }
    }
    public void PreviousPage()
    {
        for (int i = pageIndex - 1; i >= 0; --i)
        {
            if (Unlocked[i])
            {
                pageIndex = i;
                pageImage.sprite = Pages[i];
                break;
            }
        }
    }

}
