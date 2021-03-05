using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class PaintingPortal : MonoBehaviour
{
    [Label] public TextMesh tipText;

    bool acting;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            acting = true;
            tipText.text = $"[{InputSystem.Setting.MainKeys[InputKey.Interact].ToString()}] Enter Portal";
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            acting = false;
            tipText.text = null;
        }
    }

    private void Update()
    {
        if (acting && InputSystem.GetKeyDown(InputKey.Interact))
        {
            GameFlowSystem.SendGameMessage(GameMessage.Return);
        }
    }
}
