using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

// The weapon used by player
public class PaintBrush : MonoBehaviour
{
    [MinsHeader("Paint Brush", SummaryType.Title)]
    [Label] public Transform damagePoint;
    [Label] public Transform handPoint;
    [Label] public Transform backPoint;
    [Label] public SimpleEvent onAppear;

    public Vector3 DamagePoint => damagePoint.position;


    Transform stickyPoint;
    void Start()
    {
        stickyPoint = backPoint;
        GameplaySystem.onCrystalHad += UpdateCrystal;
        UpdateCrystal();
    }
    void OnDestroy()
    {
        GameplaySystem.onCrystalHad -= UpdateCrystal;
    }
    void Update()
    {
        transform.position = stickyPoint.position;
        transform.rotation = stickyPoint.rotation;
    }
    public void ToHand()
    {
        if (stickyPoint == handPoint) return;
        stickyPoint = handPoint;
        onAppear?.Invoke();
    }

    public void ToBack()
    {
        if (stickyPoint == backPoint) return;
        stickyPoint = backPoint;
        onAppear?.Invoke();
    }
    bool appeared = true;
    public void UpdateCrystal()
    {
        if (GameplaySystem.crystalsHad[0])
        {
            if (!appeared)
            {
                gameObject.SetActive(true);
                onAppear?.Invoke();
            }
        }
        else
        {
            appeared = false;
            gameObject.SetActive(false);
        }
    }
}
