using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

[RequireComponent(typeof(PlayerPlatformAvater))]
public class PlayerPlatformController : MonoBehaviour
{
    public static PlayerPlatformController instance;

    PlayerPlatformAvater avater;

    void Awake()
    {
        avater = GetComponent<PlayerPlatformAvater>();
        instance = this;
        GameplaySystem.CurrentPlayer = avater;
    }

    float IMovingX { set => avater.IMovingX = value; }
    void SetInputTrigger(string input) => avater.SetInputTrigger(input);

    void Update()
    {
        float movingX = 0;
        if (InputSystem.GetKey(InputKey.Right)) movingX += 1;
        if (InputSystem.GetKey(InputKey.Left)) movingX -= 1;
        IMovingX = movingX;

        if (InputSystem.GetKeyDown(InputKey.Jump)) SetInputTrigger("Jump");
        if (InputSystem.GetKeyDown(InputKey.Action)) SetInputTrigger("Interact");

        if (InputSystem.GetKeyDown(InputKey.Attack)) avater.TempAttack();
    }
}
