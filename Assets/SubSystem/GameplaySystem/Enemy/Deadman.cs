using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A deadman, for practice
/// </summary>
public class Deadman : GAttackable
{
    public override void OnAttacked(float damage, float power, Vector3 direction)
    {
        base.OnAttacked(damage, power, direction);
        Debug.DrawRay(transform.position + Vector3.up * GameSystem.GameplaySystem.CurrentPlayer.Avatar.attackerPointOffsetY, direction, Color.green, 1);
    }
}
