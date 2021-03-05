using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class GInteractable : MonoBehaviour
{
    [Label] public TextMesh tipTextMesh;
    [Label] public string tipText;
    [Label] public SimpleEvent onInteracted;

    bool pendingInteract;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pendingInteract = true;
            tipTextMesh.text = $"[{InputSystem.Setting.MainKeys[InputKey.Interact].ToString()}] {tipText}";
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pendingInteract = false;
            tipTextMesh.text = null;
        }
    }
    protected virtual void Start()
    {
        tipTextMesh.text = null;
    }

    void Update()
    {
        if (pendingInteract && InputSystem.GetKeyDown(InputKey.Interact))
        {
            OnInteracted();
        }
    }

    protected virtual void OnInteracted()
    {
        onInteracted?.Invoke();
    }
}
