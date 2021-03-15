using UnityEngine;
using GameSystem;
using GameSystem.Setting;

[RequireComponent(typeof(PlayerAvatar))]
public class PlayerAvatarController : MonoBehaviour
{
    [MinsHeader("Move")]
    [Label] public float jumpForce;
    [Label] public float walkingForce;
    [Label] public float runningForce;
    [Label] public float dashForce;

    GameplaySystemSetting Setting => GameplaySystem.Setting;

    public PlayerAvatar Avatar { get; private set; }
    public Vector3 FocusPoint => transform.position + Setting.playerFocusHeight * Vector3.up;

    void Awake()
    {
        Avatar = GetComponent<PlayerAvatar>();
        GameplaySystem.CurrentPlayer = this;
    }

    void SetInputTrigger(string input) => Avatar.SetInputTrigger(input);

    bool dashLocked = false;
    void Update()
    {
        Vector3 input = Vector3.zero;
        if (InputSystem.GetKey(InputKey.Right)) input.x += 1;
        if (InputSystem.GetKey(InputKey.Left)) input.x -= 1;
        if (InputSystem.GetKey(InputKey.Up)) input.z += 1;
        if (InputSystem.GetKey(InputKey.Down)) input.z -= 1;
        if (input != Vector3.zero)
        {
            input = input.normalized * Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.z));
            var dir = Quaternion.LookRotation(Vector3.Cross(GameplaySystem.CurrentCamera.transform.right, Vector3.up), Vector3.up) * input;
            Avatar.Move(dir * (InputSystem.GetKey(InputKey.Run) ? runningForce : walkingForce));

            if (!dashLocked && InputSystem.GetKeyDown(InputKey.Run))
            {
                if (Vector3.Dot(transform.forward, dir) > 0)
                {
                    Avatar.Dash(dir * dashForce);
                }
                else
                {
                    Avatar.Dodge(dir * dashForce);
                }
                dashLocked = true;
            }
        }
        else
        {
            Avatar.Move(input);
            if (!dashLocked && InputSystem.GetKeyDown(InputKey.Run))
            {
                Avatar.Dodge(-transform.forward * dashForce);
                dashLocked = true;
            }
        }
        if (InputSystem.GetKeyDown(InputKey.Jump)) Avatar.Jump(jumpForce);
        if (InputSystem.GetKeyDown(InputKey.Interact)) SetInputTrigger("Interact");
        if (InputSystem.GetKeyDown(InputKey.Attack)) Avatar.Attack();
        if (InputSystem.GetKeyDown(InputKey.Block)) Avatar.Block();
        if (InputSystem.GetKeyUp(InputKey.Block)) Avatar.EndBlock();
    }
    public void UnlockDash() => dashLocked = false;
}
