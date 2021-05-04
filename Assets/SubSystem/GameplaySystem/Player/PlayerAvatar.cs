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
    [Label] public BlockShield blockInner;
    [Label] public BlockShield blockOutter;
    [Label] public float attackerPointOffsetY;
    [Label] public SimpleEvent onStartAttack;
    [Label] public SimpleEvent onEndAttack;
    [Label] public SimpleEvent onBlock;
    [Label] public SimpleEvent onEndBlock;

    public static float currentHealth = 0;

    [System.Serializable]
    public struct AttackInfo
    {
        [Label] public string name;
        [Label] public float attackDamage;
        [Label] public float attackPower;
        [Label] public AudioSource attackSound;
    }

    [Label("Info")] public AttackInfo[] attackInfos;

    float brushAppearTimer;
    public override void Attack()
    {
        brushAppearTimer = GameplaySystem.Setting.brushAppearTime;
        brush.ToHand();
        base.Attack();
    }

    public void HeavyAttack()
    {
        brushAppearTimer = GameplaySystem.Setting.brushAppearTime;
        brush.ToHand();
        anim.SetTrigger("Heavy Attack");
    }

    public void Block()
    {
        brushAppearTimer = GameplaySystem.Setting.brushAppearTime;
        brush.ToHand();
        anim.SetBool("Block", true);
        blockOutter.BlockOnce();
        blockInner.BlockStart();
        onBlock?.Invoke();
    }
    public void EndBlock()
    {
        anim.SetBool("Block", false);
        blockInner.BlockEnd();
        onEndBlock?.Invoke();
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

    protected override void Die()
    {
        base.Die();
        GameFlowSystem.SendGameMessage(GameMessage.GameOver);
    }

    protected override void Awake()
    {
        base.Awake();
        if (currentHealth > 0) _health = currentHealth;
    }

    void OnDestroy()
    {
        currentHealth = Health;
    }


    #region 【Debug】
    [MinsHeader("Debug")]
    [Label] public StringEvent onDebug;
    void Update()
    {
        onDebug?.Invoke($"onGround:{OnGround}");
        if (brushAppearTimer > 0)
        {
            brushAppearTimer -= Time.deltaTime;
            if (brushAppearTimer < 0)
            {
                brush.ToBack();
            }
        }
    }
    [ContextMenu("Test Harm")]
    public void TestHarm()
    {
        OnAttacked(1, 10, Vector3.up);
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
