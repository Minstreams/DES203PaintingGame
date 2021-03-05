using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

[RequireComponent(typeof(PlayerAvatar))]
public class PlayerAvatarController : MonoBehaviour
{
    [MinsHeader("Move")]
    [Label] public float jumpForce;
    [Label] public float walkingForce;
    [Label] public float runningForce;

    public static PlayerAvatarController instance;

    PlayerAvatar avatar;

    void Awake()
    {
        avatar = GetComponent<PlayerAvatar>();
        instance = this;
        GameplaySystem.CurrentPlayer = avatar;
    }

    void SetInputTrigger(string input) => avatar.SetInputTrigger(input);

    void Update()
    {
        Vector3 input = Vector3.zero;
        if (InputSystem.GetKey(InputKey.Right)) input.x += 1;
        if (InputSystem.GetKey(InputKey.Left)) input.x -= 1;
        if (InputSystem.GetKey(InputKey.Up)) input.z += 1;
        if (InputSystem.GetKey(InputKey.Down)) input.z -= 1;
        input = input.normalized * Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.z));
        avatar.Move(Camera.main.transform.rotation * input * (InputSystem.GetKey(InputKey.Run) ? runningForce : walkingForce));

        if (InputSystem.GetKeyDown(InputKey.Jump)) avatar.Jump(jumpForce);
        if (InputSystem.GetKeyDown(InputKey.Interact)) SetInputTrigger("Interact");
        //if (InputSystem.GetKeyDown(InputKey.Attack)) avater.TempAttack();
    }
}
