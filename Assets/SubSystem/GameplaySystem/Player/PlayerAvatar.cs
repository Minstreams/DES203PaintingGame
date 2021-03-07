using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class PlayerAvatar : GCharacter
{
    [Label] public PaintBrush brush;

    public override void Attack()
    {
        base.Attack();
    }


    #region 【Debug】
    [Separator]
    [MinsHeader("Player Avatar", SummaryType.Title, -2)]
    [MinsHeader("Avatar of the player.", SummaryType.CommentCenter, -1)]

    [MinsHeader("Debug")]
    [Label] public StringEvent onDebug;
    void Update()
    {
        onDebug?.Invoke($"onGround:{OnGround}");
    }
    #endregion
}
