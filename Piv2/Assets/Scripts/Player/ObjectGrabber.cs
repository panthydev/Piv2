using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{

    public InputAction pickupAction;
    private float _pickupActionValue;

    public float interactRadius = 2;
    public float objPickupCooldownTimer = 0.05f;
    public float objPickupCooldownTimerReset = 0.05f;

    private void Awake()
    {
        pickupAction.Enable();
    }

    private void GetInput()
    {
        _pickupActionValue = pickupAction.ReadValue<float>();
    }

    private void Update()
    {
        GetInput();
    }

    private List<Interactable> OldInteractables = new List<Interactable>();
    private void FixedUpdate()
    {
        handleInteraction();
    }

    private void handleInteraction()
    {
        objPickupCooldownTimer -= Time.deltaTime;
        var newInteractables = CheckForInteractables();
        
        if (newInteractables.Count == 0)return; // return gate
        
        foreach (var interactable in this.OldInteractables)
        {
            if(!newInteractables.Contains(interactable)) {interactable.SetTextActive(false);}  // if interable is now outside range
        }

        foreach (var newInteractable in newInteractables)
        {
            newInteractable.SetTextActive(true);
        }

        PickupObject(newInteractables);
        OldInteractables = newInteractables;
    }


    private List<Interactable> CheckForInteractables()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, interactRadius);
        List<Interactable> interactables = new List<Interactable>();
        foreach (var collider in hitColliders)
        {
            Interactable interactable =  collider.gameObject.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactables.Add(interactable);
            }
        }

        return interactables;
    }

    private bool isHoldingObject;
    private GameObject heldObject;


    private void PickupObject(List<Interactable> interactables)
    {
        if (_pickupActionValue > 0 && objPickupCooldownTimer < 0)
        {
            objPickupCooldownTimer = objPickupCooldownTimerReset; // reset timer
            interactables[0].gameObject.transform.SetParent(this.transform);
            
        } 
    }
    
}
