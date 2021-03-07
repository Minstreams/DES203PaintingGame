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
    [Label] public Vec2Event onAttackedDir;

    public virtual void OnAttacked(float damage, Vector2 direction)
    {
        onAttacked?.Invoke(damage);
        onAttackedDir?.Invoke(direction);
        GameplaySystem.CurrentCamera.ReactBack();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("DamageZone"))
        {
            var dz = collision.GetComponent<DamageZone>();
            if (dz.attacker != gameObject) OnAttacked(dz.damage, ((Vector2)(dz.transform.localToWorldMatrix * dz.direction)).normalized);
        }
    }
}
