using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class GInteractable : GAttackable
{
    [MinsHeader("G Interactable", SummaryType.Title, -1)]
    [MinsHeader("This Component can be interacted.", SummaryType.CommentCenter)]
    [Label] public TextMesh tipTextMesh;    // to Show Text in side Scrolling mode
    [Label] public string tipText;
    [Label] public SimpleEvent onInteracted;

    public void ShowTipText()
    {
        tipTextMesh.text = $"[{InputSystem.Setting.MainKeys[InputKey.Interact]}] {tipText}";
    }
    public void HideTipText()
    {
        tipTextMesh.text = null;
    }
    public virtual void Interact()
    {
        onInteracted?.Invoke();
    }
}
