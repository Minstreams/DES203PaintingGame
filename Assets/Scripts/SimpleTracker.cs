using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTracker : MonoBehaviour
{
    [Label] public Transform target;
    void LateUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
    private void Update()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
    private void FixedUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
