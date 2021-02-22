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
}
