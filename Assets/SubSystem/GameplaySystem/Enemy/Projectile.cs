using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class Projectile : MonoBehaviour
{
    [Label] public float speed;
    [Label] public float damage;
    [Label] public float power;
    [Label] public float dieDelay;

    [Label] public SimpleEvent onEject;
    [Label] public SimpleEvent onHit;
    [Label] public SimpleEvent onRebounce;

    Rigidbody rig;
    bool dead;
    void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rig.velocity = transform.forward * speed;
        onEject?.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (dead) return;
        var block = collision.collider.GetComponent<BlockShield>();
        if (block != null)
        {
            block.Hit();
            if (block.rebounce)
            {
                //rig.AddForce(-rig.velocity * 2, ForceMode.Impulse);
                rig.velocity = -transform.forward * speed;
                block.onRebounce?.Invoke();
                onRebounce?.Invoke();
                return;
            }
        }
        var target = collision.collider.GetComponent<GAttackable>();
        if (target != null)
        {
            target.OnAttacked(damage, power, rig.velocity.normalized);
        }
        onHit?.Invoke();
        dead = true;
        Invoke("Die", dieDelay);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
