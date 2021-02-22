using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class PlayerPlatformAvater : Character
{
    #region 【Parameters】
    [MinsHeader("Attack")]
    public Transform damageZoneAnchor;
    public GameObject damageZonePrefab;
    public float attackDelay;
    #endregion

    #region 【Input】
    public void TempAttack()
    {
        StartCoroutine(TempAttackCoroutine());
    }
    IEnumerator TempAttackCoroutine()
    {
        onAttack?.Invoke();
        yield return new WaitForSeconds(attackDelay);
        Instantiate(damageZonePrefab, damageZoneAnchor.position, damageZoneAnchor.rotation).GetComponent<DamageZone>().attacker = gameObject;
    }
    #endregion

    #region 【Output Events】
    public SimpleEvent onAttack;
    #endregion

    public static PushableCase caseNearby;
    [Label] public float pushingForce;

    protected override IEnumerator MoveCondition()
    {
        yield return 0;
        if (IInteract)
        {
            if (caseNearby != null)
            {
                yield return GrabbingCase(caseNearby);
            }
        }
    }

    IEnumerator GrabbingCase(PushableCase @case)
    {
        Vector3 offset = @case.transform.position - transform.position;
        var box = @case.GetComponentInChildren<BoxCollider2D>().transform;
        box.SetParent(transform);
        ClearInputTrigger();
        while (true)
        {
            yield return 0;
            rig.AddForce(Vector2.right * IMovingX * pushingForce - rig.velocity * Setting.groundDrag, ForceMode2D.Force);
            var v = rig.velocity;
            onMoving?.Invoke(Mathf.Abs(v.x));

            @case.transform.position = transform.position + offset;

            if (IInteract)
            {
                groundAttached = 1;
                break;
            }
            if (OffGround) break;
        }
        box.SetParent(@case.transform);
    }
}
