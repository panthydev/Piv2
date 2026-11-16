using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float _verticalAngle;
    private float _horizontalAngle = 0f;
    
    public InputAction moveInput;
    public InputAction mouseInput;
    private Vector3 K_WishDir;
    private Vector2 M_WishDir;
    private Vector2 M_Delta;

    private Camera _mainCamera;
    
    private void Awake()
    {
        _mainCamera = Camera.main;

        _verticalAngle = _mainCamera.transform.rotation.eulerAngles.x;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        moveInput.Enable();
        mouseInput.Enable();
    }

    private void Update()
    {
        GetPlayerInput();
        CalculateLook();
    }

    private void GetPlayerInput()
    {
        K_WishDir = moveInput.ReadValue<Vector2>().normalized;
        K_WishDir.z = K_WishDir.y;
        K_WishDir.y = 0;
        print("Keyboard Direction: "+ K_WishDir);

        M_Delta = mouseInput.ReadValue<Vector2>();
        print("Mouse input: " + M_Delta);
    }


    private void CalculatePlayerDirection()
    {
        
    }


    private float _upperLookLimit = 60;
    private float _lowerLookLimit = -89f;
    

    private void CalculateLook()
    {
        //perform rotation on the player transform Y axis and the Camera (head) X axis for up and down

        Vector3 playerEulerRotation = new Vector3(0, M_Delta.x, 0);
        Vector3 cameraEulerRotation = new Vector3(M_Delta.y , 0, 0);

        
        // UP DOWN
        _verticalAngle = Mathf.Clamp(_verticalAngle += M_Delta.y *-1, _lowerLookLimit, _upperLookLimit); // update stored camera angle from mouse input
        
        Vector3 newCameraRotation = new Vector3(_verticalAngle, 0, 0);
        
        _mainCamera.transform.eulerAngles = newCameraRotation; // set camera new rotation
        
        // LEFT RIGHT


        _horizontalAngle += M_Delta.x;

        Vector3 newPlayerRotation = new Vector3(0, _horizontalAngle, 0);

        transform.eulerAngles = newPlayerRotation;
        _mainCamera.transform.eulerAngles = new Vector3(_verticalAngle, _horizontalAngle, 0);




    }
    
    
}
