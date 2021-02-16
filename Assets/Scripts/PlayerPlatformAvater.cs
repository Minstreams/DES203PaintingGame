using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class PlayerPlatformAvater : MonoBehaviour
{
    public float maxSpeed;
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    float speed;
    void Update()
    {
        float input = 0;
        if (InputSystem.GetKey(InputKey.Right)) input += 1;
        if (InputSystem.GetKey(InputKey.Left)) input -= 1;
        speed += (input - speed) * 0.1f;
        transform.position += Vector3.right * speed * maxSpeed * Time.deltaTime;
        anim.SetFloat("Speed", speed);
    }

    void OnGUI()
    {
        GUILayout.Label($"Speed:{speed}");
    }
}
