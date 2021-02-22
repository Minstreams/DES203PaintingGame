using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.Setting;

/// <summary>
/// Base Character
/// </summary>
public class Character : GDestroyable
{
    #region 【Parameters】
    [MinsHeader("Move")]
    [Label] public float movingForceOnGround;
    [Label] public float movingForceOffGround;
    [Label] public float jumpForce;
    [Label] public AnimationCurve turningCurve;
    [Label] public float turningTime;
    #endregion

    #region 【References】
    protected GameplaySystemSetting Setting => GameplaySystem.Setting;
    protected Animator anim;
    protected Rigidbody2D rig;

    void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region 【Input】
    HashSet<string> inputTriggerSet = new HashSet<string>();
    protected bool GetInputTrigger(string input) => inputTriggerSet.Remove(input);
    protected void ClearInputTrigger() => inputTriggerSet.Clear();
    public bool SetInputTrigger(string input) => inputTriggerSet.Add(input);

    public float IMovingX { get; set; } = 0;
    protected bool IJump => GetInputTrigger("Jump");
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

    #region 【Properties】
    bool OnGround => groundAttached > 0;
    bool OffGround => groundAttached == 0;
    float MovingForce => OnGround ? movingForceOnGround : movingForceOffGround;
    #endregion

    #region 【State Machine】
    protected override void Start()
    {
        StartCoroutine(Move());
        facingRight = true;
        facingX = 1;
    }

    bool facingRight;
    IEnumerator Move()
    {
        Coroutine turnCoroutine = null;
        ClearInputTrigger();
        while (true)
        {
            yield return 0;
            rig.AddForce(Vector2.right * IMovingX * MovingForce - rig.velocity * Setting.groundDrag, ForceMode2D.Force);
            var v = rig.velocity;
            onMoving?.Invoke(Mathf.Abs(v.x));

            if ((facingRight && v.x < 0) || (!facingRight && v.x > 0))
            {
                facingRight = !facingRight;
                if (turnCoroutine != null) StopCoroutine(turnCoroutine);
                turnCoroutine = StartCoroutine(Turn());
            }

            if (IJump)
            {
                // Clear velocity in Y axis
                v.y = 0f;
                rig.velocity = v;

                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                onJump?.Invoke();
            }

            if (OffGround)
            {
                StartCoroutine(MoveInTheAir());
                break;
            }
        }
    }
    float facingX;
    IEnumerator Turn()
    {
        if (facingRight)
        {
            while (facingX < 1)
            {
                yield return 0;
                facingX += Time.deltaTime * 2 / turningTime;
                anim.transform.rotation = Quaternion.AngleAxis(-turningCurve.Evaluate(facingX * 0.5f + 0.5f) * 180 - 90, Vector3.up);
                anim.SetFloat("Turn", 1 - Mathf.Abs(facingX));
            }
            facingX = 1;
            anim.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        }
        else
        {
            while (facingX > -1)
            {
                yield return 0;
                facingX -= Time.deltaTime * 2 / turningTime;
                anim.transform.rotation = Quaternion.AngleAxis(turningCurve.Evaluate(-facingX * 0.5f + 0.5f) * 180 + 90, Vector3.up);
                anim.SetFloat("Turn", Mathf.Abs(facingX) - 1);
            }
            facingX = -1;
            anim.transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
        }
        anim.SetFloat("Turn", 0);
    }
    IEnumerator MoveInTheAir()
    {
        ClearInputTrigger();
        while (true)
        {
            yield return 0;
            rig.AddForce(Vector2.right * IMovingX * MovingForce, ForceMode2D.Force);
            onMoving?.Invoke(Mathf.Abs(rig.velocity.x));

            if (OnGround)
            {
                StartCoroutine(Move());
                yield break;
            }
        }
    }

    private void Update()
    {
        anim.SetFloat("SpeedX", Mathf.Abs(rig.velocity.x));

        onDebug?.Invoke($"SpeedX:{rig.velocity}\n" +
            $"OnGround:{OnGround}\n" +
            $"FacingRight:{facingRight}\n" +
            $"Health:{Health}");
    }
    #endregion

    #region 【Reactions】
    int groundAttached = 0; //the number of attached grounds
    Vector2 normal = Vector2.down;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            foreach (ContactPoint2D v in collision.contacts)
            {
                if (v.normal.y > normal.y) normal = v.normal;
            }
            onContactsChanged?.Invoke(normal);

            if (groundAttached <= 0)
            {
                float relativeV = Vector2.Dot(collision.relativeVelocity, normal);
                onLandingGround?.Invoke(relativeV);
                anim.SetBool("OnGround", true);
            }
            groundAttached++;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            groundAttached--;

            if (groundAttached > 0)
            {
                ContactPoint2D[] contacts = new ContactPoint2D[32];
                int ccount = rig.GetContacts(contacts);
                normal = Vector2.down;
                for (int i = 0; i < ccount; i++)
                {
                    if (contacts[i].normal.y > normal.y) normal = contacts[i].normal;
                }
                onContactsChanged?.Invoke(normal);
            }
            else
            {
                anim.SetBool("OnGround", false);
            }
        }
    }
    #endregion
}
