using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class CameraOverrideZone : MonoBehaviour
{
    [LabelRange] public float posLerpRate;
    [LabelRange] public float rotLerpRate;
    [Label] public Vector3 offset;
    [Label] public Vector3 forwardDir = Vector3.down;
    [Label] public Vector3 upwardDir = Vector3.forward;

    public Vector3 Pos => transform.position + transform.rotation * offset;
    public Quaternion Rot => Quaternion.LookRotation(transform.rotation * forwardDir, transform.rotation * upwardDir);

    CameraController Cam => GameplaySystem.CurrentCamera;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cam.OverrideZone = this;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cam.OverrideZone = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Pos, 0.2f);
        Gizmos.DrawLine(Pos, Pos + Rot * Vector3.forward * 5);
    }
}