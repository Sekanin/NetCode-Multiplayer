using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class NetworkAnimateHandOnInput : NetworkBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    private void Update()
    {
        if (IsOwner)
        {
            float triggerValue = pinchAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Trigger" , triggerValue);
        
            float gripValue = gripAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Grip" , gripValue);
        }
    }
}
