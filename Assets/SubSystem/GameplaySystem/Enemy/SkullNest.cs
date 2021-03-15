using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.Setting;

public class SkullNest : MonoBehaviour
{
    [MinsHeader("Skull Nest", SummaryType.Title, -2)]
    [MinsHeader("Parameters")]
    [Label] public Vector3 areaOffset;
    [Label] public float patrolRadius;
    [Label] public float chaseRadius;
    [Label(true)] public int capacity = 8;

    public Vector3 Center => transform.position + areaOffset;

    GameplaySystemSetting Setting => GameplaySystem.Setting;
    PlayerAvatarController Player => GameplaySystem.CurrentPlayer;

    HashSet<Skull> skulls;
    HashSet<Skull> skullsBattling = new HashSet<Skull>();
    Quaternion[] rots;
    float[] radius;
    int patrolPointIndex;

    void Awake()
    {
        skulls = new HashSet<Skull>(GetComponentsInChildren<Skull>());
        patrolPointIndex = 0;
        const float power = 1.0f / 3;
        rots = new Quaternion[capacity];
        radius = new float[capacity - 1];
        rots[capacity - 1] = Random.rotation;
        for (int i = 0; i < capacity - 1; ++i)
        {
            rots[i] = Random.rotation;
            radius[i] = Mathf.Pow(Random.value, power);
        }
    }
    public Vector3 NewPatrolPoint()
    {
        var res = rots[patrolPointIndex % capacity] * (radius[patrolPointIndex % (capacity - 1)] * patrolRadius * Vector3.up);
        ++patrolPointIndex;
        return Center + res;
    }

    // chase
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Patroling());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (skullsBattling.Count > 0)
            {
                foreach (var s in skullsBattling)
                {
                    s.ToPatrolState();
                }
            }
            skullsBattling.Clear();

            StopAllCoroutines();
        }
    }
    IEnumerator Patroling()
    {
        var interval = new WaitForSeconds(0.5f);

        while (true)
        {
            foreach (var s in skulls)
            {
                if (skullsBattling.Contains(s)) continue;
                if ((s.transform.position - Player.FocusPoint).sqrMagnitude < s.viewSqrDistance)
                {
                    s.ToBattleState();
                    skullsBattling.Add(s);
                }
            }
            yield return interval;
        }
    }
}
