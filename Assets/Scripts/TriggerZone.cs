using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class TriggerZone : MonoBehaviour
{
    public SimpleEvent onTriggered;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onTriggered?.Invoke();
        }
    }
}
