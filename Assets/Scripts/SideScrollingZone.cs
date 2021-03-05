using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class SideScrollingZone : MonoBehaviour
{
    [MinsHeader("Parameters")]
    [Label] public float distance;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);

        var p = transform.position + transform.right * distance;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawWireSphere(p, 0.3f);

        Gizmos.color = new Color(0, 0.6f, 0, 0.6f);
        Gizmos.DrawLine(transform.position, p);

        Gizmos.color = Color.white;
    }
}
