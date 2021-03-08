using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.Setting;

public class Skull : GDestroyable
{
    [MinsHeader("Skull", SummaryType.Title, -2)]
    [MinsHeader("Parameters")]
    [LabelRange] public float approachRate = 0.9f;
    [Label] public float approachTime = 0.9f;
    [Label] public float maxForce;
    [Label] public float maxSpeed = 3;
    [Label] public Vector2 patrolIntervalRange;
    [Label] public float viewSqrDistance;
    [MinsHeader("Events")]
    [Label] public SimpleEvent onEnterPatrolState;
    [Label] public SimpleEvent onEnterBattleState;

    protected GameplaySystemSetting Setting => GameplaySystem.Setting;
    protected PlayerAvatarController Player => GameplaySystem.CurrentPlayer;

    protected Rigidbody rig;
    protected SkullNest nest;
    protected Vector3 targetPos;

    void Awake()
    {
        rig = GetComponent<Rigidbody>();
        nest = GetComponentInParent<SkullNest>();
    }
    protected override void Start()
    {
        base.Start();
        if (nest == null)
        {
            GameplaySystem.LogAssertion("Skull should be in a nest!");
            return;
        }
        StartCoroutine(PatrolState());
    }
    void FixedUpdate()
    {
        var targetV = (targetPos - transform.position) * 1.5f * approachRate / approachTime;
        var force = (targetV - rig.velocity) * approachRate * rig.mass;
        var f = force.magnitude;
        if (f > maxForce * Time.deltaTime) force *= maxForce * Time.deltaTime / f;
        rig.AddForce(force, ForceMode.Impulse);
        var speed = rig.velocity.magnitude;
        if (speed > maxSpeed) rig.velocity *= maxSpeed / speed;
    }
    public override void OnAttacked(float damage, float power, Vector3 direction)
    {
        base.OnAttacked(damage, power, direction);
        rig.velocity = Vector3.zero;
        rig.AddForce(direction * power, ForceMode.Impulse);
        Debug.DrawRay(transform.position, direction * power, Color.yellow, 1);
    }
    protected override void Die()
    {
        base.Die();
        GetComponent<Collider>().enabled = false;
        StopAllCoroutines();
        enabled = false;
    }
    IEnumerator PatrolState()
    {
        onEnterPatrolState?.Invoke();
        while (true)
        {
            targetPos = nest.NewPatrolPoint();
            yield return new WaitForSeconds(Random.Range(patrolIntervalRange.x, patrolIntervalRange.y));
        }
    }
    protected virtual IEnumerator BattleState()
    {
        while (true)
        {
            yield return 0;
        }
    }
    public void ToBattleState()
    {
        StopAllCoroutines();
        onEnterBattleState?.Invoke();
        StartCoroutine(BattleState());
    }
    public void ToPatrolState()
    {
        StopAllCoroutines();
        StartCoroutine(PatrolState());
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(viewSqrDistance));
        Gizmos.color = Color.white;
    }
}
