using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The weapon used by player
public class PaintBrush : MonoBehaviour
{
    [MinsHeader("Paint Brush", SummaryType.Title)]
    [Label] public Transform damagePoint;

    public Vector3 DamagePoint => damagePoint.position;
}
