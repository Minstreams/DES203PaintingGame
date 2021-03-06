﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.Setting;

/// <summary>
/// Base Character
/// </summary>
public class GCharacter : GDestroyable
{
    #region 【Parameters】
    [Separator]
    [MinsHeader("G Character", SummaryType.Title, -2)]
    [MinsHeader("This is a character.", SummaryType.CommentCenter, -1)]

    [MinsHeader("Move")]
    [Tooltip("The factor multiplied to the force applied off ground")]
    [LabelRange] public float movingOffGroundFactor;
    [Tooltip("The minimum speed to perform turning animation")]
    [Label] public float turningSpeedThreshold;
    [LabelRange] public float turningRatePerSecond;

    [MinsHeader("Physics")]
    [Label] public Vector3 groundOverlapSphereOffset;
    [Label] public float groundOverlapSphereRadius;
    #endregion

    #region 【References】
    protected GameplaySystemSetting Setting => GameplaySystem.Setting;
    protected Animator anim;
    protected Rigidbody rig;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rig = GetComponent<Rigidbody>();
    }
    #endregion

    #region 【Output Events】
    [MinsHeader("Events")]
    [Label] public FloatEvent onMoving;     // passing the speed
    [Label] public SimpleEvent onJump;      // invoked when input jump
    [Label] public SimpleEvent onLeavingGround; // invoked when actually leaving off ground
    [Label] public FloatEvent onLandingGround;  // passing the relevant speed landing
    #endregion

    #region 【Input】
    HashSet<string> inputTriggerSet = new HashSet<string>();

    // interface
    public bool SetInputTrigger(string input) => inputTriggerSet.Add(input);
    /// <summary>
    /// Set the moving Vector
    /// </summary>
    public void Move(Vector3 force)
    {
        IMovingForce = force;
    }
    /// <summary>
    /// Perform a jump action
    /// </summary>
    /// <param name="jumpForce">force applied upward</param>
    public void Jump(float jumpForce)
    {
        if (OffGround) return;
        onJump?.Invoke();
        rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    /// <summary>
    /// Perform an interact action
    /// </summary>
    public void Interact()
    {
        SetInputTrigger("Interact");
    }

    // protected
    protected void ClearInputTrigger() => inputTriggerSet.Clear();
    protected bool GetInputTrigger(string input) => inputTriggerSet.Remove(input);

    // inputs
    protected Vector3 IMovingForce { get; private set; }
    protected bool IInteract => GetInputTrigger("Interact");
    #endregion

    #region 【Physics】
    bool onGround;
    protected bool OnGround => onGround;
    protected bool OffGround => !onGround;

    void OnCollisionEnter(Collision collision)
    {
        if (OffGround)
        {
            var colliders = Physics.OverlapSphere(transform.position + transform.rotation * groundOverlapSphereOffset, groundOverlapSphereRadius, 1 << LayerMask.NameToLayer("Ground"), QueryTriggerInteraction.Ignore);
            if (colliders.Length > 0)
            {
                onGround = true;
                onLandingGround?.Invoke(collision.relativeVelocity.y);
                anim.SetBool("OnGround", true);
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (OnGround)
        {
            var colliders = Physics.OverlapSphere(transform.position + transform.rotation * groundOverlapSphereOffset, groundOverlapSphereRadius, 1 << LayerMask.NameToLayer("Ground"), QueryTriggerInteraction.Ignore);
            if (colliders.Length == 0)
            {
                onGround = false;
                onLeavingGround?.Invoke();
                anim.SetBool("OnGround", false);
            }
        }
    }
    void FixedUpdate()
    {
        rig.AddForce(IMovingForce * (OnGround ? 1 : movingOffGroundFactor), ForceMode.Force);
        var v = new Vector3(rig.velocity.x, 0, rig.velocity.z);
        anim.SetFloat("Speed", Vector3.Dot(v, transform.forward));
        if (v.sqrMagnitude > turningSpeedThreshold)
        {
            var angle = Vector3.SignedAngle(transform.forward, v, Vector3.up);
            anim.SetFloat("Turn", angle);
            transform.Rotate(Vector3.up, angle * (1 - Mathf.Pow(1 - turningRatePerSecond, Time.deltaTime)), Space.World);
        }

    }
    #endregion

    #region 【Debug】
    protected virtual void OnDrawGizmos()
    {
        // draw the ground overlap sphere
        Gizmos.color = new Color(1, 1, 0);
        Gizmos.DrawWireSphere(transform.position + transform.rotation * groundOverlapSphereOffset, groundOverlapSphereRadius);

        // draw the volecity
        Gizmos.color = Color.green;
        if (rig != null) Gizmos.DrawRay(transform.position, rig.velocity);

        // draw the facing direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.white;
    }
    #endregion
}
