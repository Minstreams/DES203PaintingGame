using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.Setting;

/// <summary>
/// Base Character
/// </summary>
public class GCharacter : MonoBehaviour
{
    #region 【Parameters】
    [MinsHeader("Move")]
    [Label] public AnimationCurve turningCurve;
    [Label] public float turningTime;
    #endregion

    #region 【References】
    protected GameplaySystemSetting Setting => GameplaySystem.Setting;
    protected Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    #endregion

    #region 【Output Events】
    [MinsHeader("Events")]
    public FloatEvent onMoving;
    public SimpleEvent onJump;
    public SimpleEvent onOffGround;
    public FloatEvent onLandingGround;
    public Vec2Event onContactsChanged;
    public StringEvent onDebug;
    #endregion
    //TODO
}
