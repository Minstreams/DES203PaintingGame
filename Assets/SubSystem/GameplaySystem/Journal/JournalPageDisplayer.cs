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
        pageIndex = 0;
        pageImage.sprite = Pages[0];
    }

    public void NextPage()
    {
        do
        {
            ++pageIndex;
            if (pageIndex >= Pages.Length) pageIndex = 0;
        } while (!Unlocked[pageIndex]);
        pageImage.sprite = Pages[pageIndex];
    }
    public void PreviousPage()
    {
        do
        {
            --pageIndex;
            if (pageIndex < 0) pageIndex = Pages.Length - 1;
        } while (!Unlocked[pageIndex]);
        pageImage.sprite = Pages[pageIndex];
    }

}
