using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    private Collider playerCollider;
    public InputAction pickupAction;
    public InputAction throwAction;
    private float _pickupActionValue;
    private float _throwActionValue;
    public Interactable closestInteractable;
    public Interactable heldInteractable;
    public bool isInteractableHeld;

    public Rigidbody PlayerRb;
    public Transform ObjHoldingTransform;
    public float objPickupCooldownTimer = 0.05f;
    public float objPickupCooldownTimerReset = 0.05f;

    private void Awake()
    {
        PlayerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        pickupAction.Enable();
        throwAction.Enable();
    }

    private void GetInput()
    {
        _pickupActionValue = pickupAction.ReadValue<float>();
        _throwActionValue = throwAction.ReadValue<float>();

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

        if (_throwActionValue > 0 && isInteractableHeld)
        {
            DropInteractable(true);
            print(_throwActionValue);
            
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
            AudioManager.Instance.PlaySFX("ui_click");
            heldInteractable = closestInteractable;
        
            heldInteractable.transform.SetParent(ObjHoldingTransform, false);
            heldInteractable.transform.localPosition = Vector3.zero;
        
            Rigidbody rb = heldInteractable.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionX |
                             RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = true;

            //disable collisions between player and object
            Collider objectCollider = heldInteractable.GetComponent<Collider>();
            Physics.IgnoreCollision(playerCollider, objectCollider);
            
            isInteractableHeld = true;
        }
        else
        {
            //Drop
            DropInteractable();
        }
    }

    private void DropInteractable(bool throwItem = false)
    {
        
        if (!throwItem)
        {
            AudioManager.Instance.PlaySFX("ui_click");
            
        }
        heldInteractable.transform.SetParent(null, true);
            
        Rigidbody rb = heldInteractable.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = false;
        //enable collisions again between player and object
        Collider objectCollider = heldInteractable.GetComponent<Collider>();
        Physics.IgnoreCollision(playerCollider, objectCollider, false);

        if (throwItem)
        {
            AudioManager.Instance.PlaySFX("Throw");
            heldInteractable.ThrowObject(Camera.main.transform.forward, PlayerRb.linearVelocity);
        }
        heldInteractable = null;
        isInteractableHeld = false;
        
        
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
