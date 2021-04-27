using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class UIDepthOfField : MonoBehaviour
{
    [Label] public float depth;
    [LabelRange(0.001f, 1)] public float fixedLerpRate = 0.05f;

    RectTransform rect;
    Vector3 pos;
    Vector3 targetPos;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        pos = rect.position;
    }

    void Update()
    {
        var offset = Input.mousePosition / Screen.height - new Vector3(0.5f, 0.5f, 0);
        targetPos = pos - offset * depth;
        rect.position = Vector3.Lerp(rect.position, targetPos, fixedLerpRate);
    }
}
