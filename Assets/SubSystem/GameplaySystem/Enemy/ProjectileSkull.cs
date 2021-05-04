using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkull : Skull
{
    [Label] public GameObject projectilePrefab;
    [Label] public float attackInterval;
    [Label] public float ejectDistance;
    protected override IEnumerator BattleState()
    {
        float timer = Random.Range(0, attackInterval);
        while (true)
        {
            yield return 0;
            timer += Time.deltaTime;
            if (timer > attackInterval)
            {
                timer -= attackInterval;
                var dir = (Player.BattlePoint - transform.position).normalized;
                Instantiate(projectilePrefab, transform.position + dir * ejectDistance, Quaternion.LookRotation(dir), null);
            }
        }
    }
}
