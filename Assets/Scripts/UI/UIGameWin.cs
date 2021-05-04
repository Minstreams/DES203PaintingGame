using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class UIGameWin : MonoBehaviour
{
    [Label] public SimpleEvent onWin;

    void Awake()
    {
        if (PlayerAvatar.currentHealth > 0)
        {
            onWin?.Invoke();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
