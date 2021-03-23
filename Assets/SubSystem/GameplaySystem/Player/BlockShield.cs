using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class BlockShield : MonoBehaviour
{
    [LabelRange] public float glowingValue;
    [LabelRange] public float glowingLerpRate;
    [Label] public SimpleEvent onHit;
    [Label] public bool rebounce;
    [ConditionalShow("rebounce")] public SimpleEvent onRebounce;
    [ConditionalShow("rebounce")] public float blockOnceTime;

    Collider col;
    Renderer ren;
    float blockGlowing;
    float targetGlowing;

    void Awake()
    {
        col = GetComponent<Collider>();
        ren = GetComponent<Renderer>();
    }

    public void BlockStart()
    {
        targetGlowing = glowingValue;
        col.enabled = true;
    }
    public void BlockEnd()
    {
        targetGlowing = 0;
        col.enabled = false;
    }
    public void BlockOnce()
    {
        StartCoroutine(blockOnce());
    }
    IEnumerator blockOnce()
    {
        BlockStart();
        yield return new WaitForSeconds(blockOnceTime);
        BlockEnd();
    }

    public void Hit()
    {
        ren.material.SetFloat("_block", 1);
        blockGlowing = 1;
        onHit?.Invoke();
    }

    void Update()
    {
        var glowing = Mathf.Lerp(blockGlowing, targetGlowing, glowingLerpRate);
        if (glowing != blockGlowing)
        {
            blockGlowing = glowing;
            ren.material.SetFloat("_block", blockGlowing);
        }
    }
}
