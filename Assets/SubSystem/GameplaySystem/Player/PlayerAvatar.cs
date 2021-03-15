using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class PlayerAvatar : GCharacter
{
    [Separator]
    [MinsHeader("Player Avatar", SummaryType.Title, -2)]
    [MinsHeader("Avatar of the player.", SummaryType.CommentCenter, -1)]
    [MinsHeader("Parameters")]
    [Label] public PaintBrush brush;
    [Label] public float attackerPointOffsetY;
    [Label] public SimpleEvent onStartAttack;
    [Label] public SimpleEvent onEndAttack;

    [System.Serializable]
    public struct AttackInfo
    {
        [Label] public string name;
        [Label] public float attackDamage;
        [Label] public float attackPower;
        [Label] public AudioSource attackSound;
    }

    [Label("Info")] public AttackInfo[] attackInfos;

    public override void Attack()
    {
        base.Attack();
    }

    List<Vector3> attackPoints = new List<Vector3>();
    public override Vector3 AttackPoint => transform.position + Vector3.up * attackerPointOffsetY;
    int currentIndex = -1;
    public void OnRecordAttack(int index)
    {
        if (currentIndex != index)
        {
            attackPoints.Clear();
            currentIndex = index;
        }
        if (attackPoints.Count == 0) onStartAttack?.Invoke();
        attackPoints.Add(brush.DamagePoint);
    }
    public void OnEndAttack(int index)
    {
        attackPoints.Add(brush.DamagePoint);
        var info = attackInfos[index];
        GameplaySystem.GenerateDamageLine(ref attackPoints, info.attackDamage, info.attackPower, this);
        attackPoints.Clear();
        onEndAttack?.Invoke();
    }
    public void OnAttackSound(int index)
    {
        attackInfos[index].attackSound.Play();
    }

    #region 【Debug】
    [MinsHeader("Debug")]
    [Label] public StringEvent onDebug;
    void Update()
    {
        onDebug?.Invoke($"onGround:{OnGround}");
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(AttackPoint, 0.1f);
        Gizmos.color = Color.white;
    }
    #endregion
}
