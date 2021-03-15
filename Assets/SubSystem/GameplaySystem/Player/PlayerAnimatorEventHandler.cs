using UnityEngine;
using GameSystem;

public class PlayerAnimatorEventHandler : MonoBehaviour
{
    [MinsHeader("Output")]
    [Label] public SimpleEvent onStep;
    [Label] public IntEvent onRecordAttack;
    [Label] public IntEvent onEndAttack;
    [Label] public IntEvent onAttackSound;
    [Label] public SimpleEvent onSpecialAnimation;
    [Label] public SimpleEvent onNormalAnimation;
    [Label] public SimpleEvent onUnlockDash;
    [Label("Action")] public SimpleEvent[] onActions;

    public void DoStep() => onStep?.Invoke();
    public void DoRecordAttack(int val) => onRecordAttack?.Invoke(val);
    public void DoEndAttack(int val) => onEndAttack?.Invoke(val);
    public void DoAttackSound(int val) => onAttackSound?.Invoke(val);
    public void DoSpecialAnimation() => onSpecialAnimation?.Invoke();
    public void DoNormalAnimation() => onNormalAnimation?.Invoke();
    public void DoUnlockDash() => onUnlockDash?.Invoke();
    public void DoAction(int index) => onActions[index]?.Invoke();
}
