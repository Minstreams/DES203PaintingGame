using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class PushableCase : MonoBehaviour
{
    [Label] public TextMesh tipText;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tipText.text = $"[{InputSystem.Setting.MainKeys[InputKey.Action].ToString()}] Grab";
            PlayerPlatformAvater.caseNearby = this;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tipText.text = "";
            PlayerPlatformAvater.caseNearby = null;
        }
    }
}