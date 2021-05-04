using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class HealingTree : MonoBehaviour
{
    [Label] public SimpleEvent onHealing;
    [Label] public SimpleEvent onHealed;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var avater = GameplaySystem.CurrentPlayer.Avatar;
            if (avater.Health < avater.maxHealth)
            {
                StartCoroutine(DelayHeal());
            }
        }
    }

    IEnumerator DelayHeal()
    {
        onHealing?.Invoke();
        yield return new WaitForSeconds(1.5f);
        var avater = GameplaySystem.CurrentPlayer.Avatar;
        avater.Health = avater.maxHealth;
        onHealed?.Invoke();
    }
}
