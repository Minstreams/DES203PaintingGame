using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class GDestroyable : GAttackable
{
    #region 【Parameters】
    [Separator]
    [MinsHeader("G Destroyable", SummaryType.Title, -1)]
    [MinsHeader("This Component can be destroyed.", SummaryType.CommentCenter)]
    [Label] public float maxHealth;
    [Label] public float defaultHealth;
    #endregion

    #region 【Output Events】
    [Label] public FloatEvent onDamaged;
    [Label] public FloatEvent onHealed;
    [Label] public SimpleEvent onDie;
    [Label] public SimpleEvent onTeabagged;
    #endregion

    #region 【Properties】
    // Fields
    protected float _health;
    // Properties
    public float Health
    {
        get => _health;
        set
        {
            if (value <= 0)
            {
                if (_health > 0)
                {
                    _health = 0;
                    onDamaged?.Invoke(value);
                    Die();
                }
            }
            else
            {
                float delta = value - _health;
                _health = value;
                if (delta > 0)
                {
                    onHealed?.Invoke(delta);
                }
                else if (delta < 0)
                {
                    onDamaged?.Invoke(-delta);
                }
            }
        }
    }
    public bool IsDead => Health <= 0;
    #endregion


    protected virtual void Awake()
    {
        _health = defaultHealth;
    }
    protected virtual void Start()
    {
    }
    public override void OnAttacked(float damage, float power, Vector3 direction)
    {
        base.OnAttacked(damage, power, direction);
        if (Health > 0)
        {
            OnDamaged(damage, power, direction);
        }
        else
        {
            OnTeabagged(damage, power, direction);
        }
    }
    protected virtual void OnDamaged(float damage, float power, Vector3 direction)
    {
        Health -= damage;

    }
    // 被鞭尸时
    protected virtual void OnTeabagged(float damage, float power, Vector3 direction)
    {
        onTeabagged?.Invoke();
    }

    protected virtual void Die()
    {
        onDie?.Invoke();
    }
}
