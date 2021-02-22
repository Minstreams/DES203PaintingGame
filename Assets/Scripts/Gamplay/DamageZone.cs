﻿using System.Collections;
using UnityEngine;
using GameSystem;

public class DamageZone : MonoBehaviour
{
    [Label] public float lifeTime;
    [Label] public float damage;

    public SimpleEvent onStart;

    [HideInInspector] public GameObject attacker;

    float timer;
    void Start()
    {
        timer = lifeTime;
        onStart?.Invoke();
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0) Destroy(gameObject);
    }
}