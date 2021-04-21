using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The weapon used by player
public class PaintBrush : MonoBehaviour
{
    [MinsHeader("Paint Brush", SummaryType.Title)]
    [Label] public Transform damagePoint;
    [Label] public Transform handPoint;
    [Label] public Transform backPoint;

    public Vector3 DamagePoint => damagePoint.position;


    Transform stickyPoint;
    void Start()
    {
        stickyPoint = backPoint;
    }
    void Update()
    {
        transform.position = stickyPoint.position;
        transform.rotation = stickyPoint.rotation;
    }
    public void ToHand()
    {
        stickyPoint = handPoint;
    }

    public void ToBack()
    {
        stickyPoint = backPoint;
    }
}
