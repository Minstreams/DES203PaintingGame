using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProp : GAttackable
{
    public override void OnAttacked(float damage, Vector2 direction)
    {
        base.OnAttacked(damage, direction);
        Debug.Log($"aaa:{damage}");
    }
}
