using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

/// <summary>
/// Protal in museum
/// </summary>
public class MPortal : MonoBehaviour, MInteractable
{
    public string GetTipText()
    {
        return "Enter Painting";
    }

    public void OnInteracted()
    {
        GameFlowSystem.SendGameMessage(GameMessage.Start);
    }
}
