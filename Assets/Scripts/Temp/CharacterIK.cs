using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    [Label] public Animator target;
    [Label("Override Layer Index", Const = true)] public bool ifOverrideLayerIndex;
    [ConditionalShow("ifOverrideLayerIndex", Const = true)] public int overrideLayerIndex;

    [MinsHeader("Lookat")]
    [LabelRange] public float ikWeight;
    [LabelRange] public float ikBodyWeight;
    [LabelRange] public float ikHeadWeight;
    [LabelRange] public float ikEyesWeight;
    [LabelRange] public float ikClampWeight;
    [Label] public Transform ikLookatPosition;

    [MinsHeader("Hint")]
    [LabelRange] public float ikLeftElbowWeight;
    [Label] public Transform ikLeftElbow;
    [LabelRange] public float ikRightElbowWeight;
    [Label] public Transform ikRightElbow;

    [MinsHeader("Hands")]
    [LabelRange] public float ikLeftHandWeight;
    [LabelRange] public float ikLeftHandRotationWeight;
    [Label] public Transform ikLeftHand;
    [LabelRange] public float ikRightHandWeight;
    [LabelRange] public float ikRightHandRotationWeight;
    [Label] public Transform ikRightHand;


    void Start()
    {
        var handler = target.gameObject.AddComponent<CharacterIKEventHandler>();
        handler.LayerIndex = ifOverrideLayerIndex ? overrideLayerIndex : 0;
        handler.animatorIKHandler += AnimatorIKHandler;
    }

    void AnimatorIKHandler()
    {
        // Look At
        if (ikLookatPosition != null)
        {
            target.SetLookAtWeight(ikWeight, ikBodyWeight, ikHeadWeight, ikEyesWeight, ikClampWeight);
            target.SetLookAtPosition(ikLookatPosition.position);
        }

        // Hint Set
        if (ikLeftElbow != null)
        {
            target.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ikLeftElbowWeight);
            target.SetIKHintPosition(AvatarIKHint.LeftElbow, ikLeftElbow.position);
        }
        if (ikRightElbow != null)
        {
            target.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ikRightElbowWeight);
            target.SetIKHintPosition(AvatarIKHint.RightElbow, ikRightElbow.position);
        }

        // Hands
        if (ikLeftHand != null)
        {
            target.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikLeftHandWeight);
            target.SetIKPosition(AvatarIKGoal.LeftHand, ikLeftHand.position);
            target.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikLeftHandRotationWeight);
            target.SetIKRotation(AvatarIKGoal.LeftHand, ikLeftHand.rotation);
        }
        if (ikRightHand != null)
        {
            target.SetIKPositionWeight(AvatarIKGoal.RightHand, ikRightHandWeight);
            target.SetIKPosition(AvatarIKGoal.RightHand, ikRightHand.position);
            target.SetIKRotationWeight(AvatarIKGoal.RightHand, ikRightHandRotationWeight);
            target.SetIKRotation(AvatarIKGoal.RightHand, ikRightHand.rotation);
        }
    }
}
