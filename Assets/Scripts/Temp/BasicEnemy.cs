using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class BasicEnemy : GDestroyable
{
    protected Rigidbody rig;
    void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    //private void Update()
    //{
    //    //if (chasing)
    //    //    rig.AddForce(((GameplaySystem.CurrentPlayer.transform.position + Vector3.up * 1.6f - transform.position)).normalized * force, ForceMode2D.Force);
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        GameplaySystem.CurrentPlayer.Avatar.OnAttacked(1, rig.velocity.normalized);
    //        rig.velocity = -((Vector2)(GameplaySystem.CurrentPlayer.transform.position + Vector3.up * 1.6f - transform.position)).normalized * backforce;
    //    }
    //}
    protected override void OnDamaged(float damage, float power, Vector3 direction)
    {
        base.OnDamaged(damage, power, direction);
        rig.velocity = direction * power;
        Debug.DrawRay(transform.position, direction, Color.yellow, 1);
    }
}
