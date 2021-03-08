using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProp : GAttackable
{
    public override void OnAttacked(float damage, float power, Vector3 direction)
    {
        base.OnAttacked(damage, power, direction);
        Debug.Log($"aaa:{damage}");
    }
}
