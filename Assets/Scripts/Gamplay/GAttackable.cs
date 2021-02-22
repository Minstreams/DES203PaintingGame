using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

/// <summary>
/// Savable object in Scene
/// </summary>
public class GAttackable : GSavable
{
    [MinsHeader("Attackable", SummaryType.Header)]
    [Label] public FloatEvent onAttacked;

    protected virtual void OnAttacked(float damage)
    {
        onAttacked?.Invoke(damage);
        SmartCamera.ReactBack();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("DamageZone"))
        {
            var dz = collision.GetComponent<DamageZone>();
            if (dz.attacker != gameObject) OnAttacked(dz.damage);
        }
    }
}
