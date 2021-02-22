using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProp : GAttackable
{
    protected override void OnAttacked(float damage)
    {
        base.OnAttacked(damage);
        Debug.Log($"aaa:{damage}");
    }
}
