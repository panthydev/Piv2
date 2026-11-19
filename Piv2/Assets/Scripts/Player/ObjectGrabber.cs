using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{

    public InputAction pickupAction;
    private float _pickupActionValue;
    public Interactable closestInteractable;
    public Interactable heldInteractable;
    public bool isInteractableHeld;

    public Transform ObjHoldingTransform;
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
    
    private void FixedUpdate()
    {
        objPickupCooldownTimer -= Time.deltaTime;
        handleInteraction();
    }

    private void handleInteraction()
    {
        if (_pickupActionValue > 0 && objPickupCooldownTimer < 0)
        {
            
            objPickupCooldownTimer = objPickupCooldownTimerReset;
            PickupInteractable();
        }

        if (isInteractableHeld)
        {
            PinObjectHeightToCamera();
        }
        
    }

    private void PinObjectHeightToCamera()
    {
        if (isInteractableHeld)
        {
            heldInteractable.transform.position = ObjHoldingTransform.transform.position;
        }
    }


    private void PickupInteractable()
    {
        if (heldInteractable == null)
        {
            if (closestInteractable == null)
                return;
            //pickup
            heldInteractable = closestInteractable;
        
            heldInteractable.transform.SetParent(ObjHoldingTransform, false);
            heldInteractable.transform.localPosition = Vector3.zero;
        
            Rigidbody rb = heldInteractable.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionX |
                             RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = true;
            isInteractableHeld = true;
        }
        else
        {
            //Drop
            heldInteractable.transform.SetParent(null, true);
            
            Rigidbody rb = heldInteractable.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;

            heldInteractable = null;
            isInteractableHeld = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable") && !isInteractableHeld)
        {
            print("interactable range entered");
            closestInteractable = other.GetComponent<Interactable>();
            closestInteractable.SetTextActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Interactable interactable = other.GetComponent<Interactable>();
            interactable.SetTextActive(false);

            if (closestInteractable == interactable)
            {
                closestInteractable = null;
            }
        }
    }
}
