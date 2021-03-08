using UnityEngine;
using GameSystem;

public class PlayerAnimatorEventHandler : MonoBehaviour
{
    [MinsHeader("Output")]
    [Label] public SimpleEvent onStep;
    [Label] public IntEvent onRecordAttack;
    [Label] public IntEvent onEndAttack;
    [Label] public IntEvent onAttackSound;

    public void DoStep() => onStep?.Invoke();
    public void DoRecordAttack(int val) => onRecordAttack?.Invoke(val);
    public void DoEndAttack(int val) => onEndAttack?.Invoke(val);
    public void DoAttackSound(int val) => onAttackSound?.Invoke(val);

}
