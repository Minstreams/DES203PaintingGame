using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

/// <summary>
/// Savable object in Scene
/// </summary>
public class GAttackable : GSavable
{
    [Space(8, order = -2)]
    [MinsHeader("G Attackable", SummaryType.Title, -1)]
    [MinsHeader("This Component can be attacked.", SummaryType.CommentCenter)]
    [Label] public FloatEvent onAttacked;
    [Label] public Vec3Event onAttackedDir;
    [Label] public FloatEvent onAttackedPower;

    public virtual Vector3 AttackPoint => transform.position;

    public virtual void OnAttacked(float damage, float power, Vector3 direction)
    {
        onAttacked?.Invoke(damage);
        onAttackedDir?.Invoke(direction);
        onAttackedPower?.Invoke(power);
        GameplaySystem.CurrentCamera.ReactBack();
    }
}
