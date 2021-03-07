using UnityEngine;
using GameSystem;

public class PlayerAnimatorEventHandler : MonoBehaviour
{
    [MinsHeader("Output")]
    [Label] public SimpleEvent onStep;
    [Label] public SimpleEvent onStartAttack;
    [Label] public SimpleEvent onEndAttack;

    public void DoStep() => onStep?.Invoke();
    public void DoStartAttack() => onStartAttack?.Invoke();
    public void DoEndAttack() => onEndAttack?.Invoke();
}
