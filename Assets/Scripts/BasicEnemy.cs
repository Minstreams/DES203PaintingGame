using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class BasicEnemy : GDestroyable
{
    Rigidbody2D rig;
    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    [Label] public bool chasing;
    [Label] public float force;
    [Label] public float backforce;
    private void Update()
    {
        if (chasing)
            rig.AddForce(((Vector2)(GameplaySystem.CurrentPlayer.transform.position + Vector3.up * 1.6f - transform.position)).normalized * force, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameplaySystem.CurrentPlayer.OnAttacked(1, rig.velocity.normalized);
            rig.velocity = -((Vector2)(GameplaySystem.CurrentPlayer.transform.position + Vector3.up * 1.6f - transform.position)).normalized * backforce;
        }
    }
    public override void OnAttacked(float damage, Vector2 direction)
    {
        base.OnAttacked(damage, direction);
        rig.velocity = direction * backforce;
    }
}
