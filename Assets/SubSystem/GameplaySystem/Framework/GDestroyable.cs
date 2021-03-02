using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class GDestroyable : GAttackable
{
    #region 【Parameters】
    [Label] public float maxHealth;
    [Label] public float defaultHealth;
    #endregion

    #region 【Output Events】
    [MinsHeader("Events")]
    public FloatEvent onDamaged;
    public FloatEvent onHealed;
    public SimpleEvent onDie;
    #endregion

    #region 【Properties】
    // Fields
    float _health;
    // Properties
    public float Health
    {
        get => _health;
        set
        {
            if (value <= 0)
            {
                if (_health > 0) Die();
                _health = 0;
            }
            else
            {
                if (value > _health)
                {
                    onHealed?.Invoke(value - _health);
                }
                else if (value < _health)
                {
                    onDamaged?.Invoke(_health - value);
                }
                _health = value;
            }
        }
    }
    #endregion

    protected virtual void Start()
    {
        _health = defaultHealth;
    }
    public override void OnAttacked(float damage, Vector2 direction)
    {
        base.OnAttacked(damage, direction);
        Health -= damage;
    }

    protected virtual void Die()
    {
        onDie?.Invoke();
    }
}
