using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A normal skull Enemy
/// </summary>
public class NormalSkull : Skull
{
    public float backPower;
    protected override IEnumerator BattleState()
    {
        while (true)
        {
            yield return 0;
            targetPos = Player.BattlePoint;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            rig.velocity = collision.contacts[0].normal * backPower;
            Player.Avatar.OnAttacked(1, 1, -collision.contacts[0].normal);
        }
    }
}
