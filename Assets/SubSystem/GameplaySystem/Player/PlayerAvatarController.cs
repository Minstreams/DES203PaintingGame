using UnityEngine;
using GameSystem;

[RequireComponent(typeof(PlayerAvatar))]
public class PlayerAvatarController : MonoBehaviour
{
    [MinsHeader("Move")]
    [Label] public float jumpForce;
    [Label] public float walkingForce;
    [Label] public float runningForce;

    public PlayerAvatar Avatar { get; private set; }

    void Awake()
    {
        Avatar = GetComponent<PlayerAvatar>();
        GameplaySystem.CurrentPlayer = this;
    }

    void SetInputTrigger(string input) => Avatar.SetInputTrigger(input);

    void Update()
    {
        Vector3 input = Vector3.zero;
        if (InputSystem.GetKey(InputKey.Right)) input.x += 1;
        if (InputSystem.GetKey(InputKey.Left)) input.x -= 1;
        if (InputSystem.GetKey(InputKey.Up)) input.z += 1;
        if (InputSystem.GetKey(InputKey.Down)) input.z -= 1;
        input = input.normalized * Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.z));
        Avatar.Move(Quaternion.LookRotation(Vector3.Cross(GameplaySystem.CurrentCamera.transform.right, Vector3.up), Vector3.up) * input * (InputSystem.GetKey(InputKey.Run) ? runningForce : walkingForce));

        if (InputSystem.GetKeyDown(InputKey.Jump)) Avatar.Jump(jumpForce);
        if (InputSystem.GetKeyDown(InputKey.Interact)) SetInputTrigger("Interact");
        //if (InputSystem.GetKeyDown(InputKey.Attack)) avater.TempAttack();
    }
}
