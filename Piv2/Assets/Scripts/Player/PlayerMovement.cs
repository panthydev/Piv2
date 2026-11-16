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
    private Vector2 _mDelta;

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

        _mDelta = mouseInput.ReadValue<Vector2>();
        print("Mouse input: " + _mDelta);
    }


    private void CalculatePlayerDirection()
    {
        
    }


    private float _upperLookLimit = 60;
    private float _lowerLookLimit = -89f;
    

    private void CalculateLook()
    {
        //perform rotation on the player transform Y axis and the Camera (head) X axis for up and down
        
        _verticalAngle = Mathf.Clamp(_verticalAngle += _mDelta.y *-1, _lowerLookLimit, _upperLookLimit); // update stored camera angle from mouse input
        _horizontalAngle += _mDelta.x;

        Vector3 newPlayerRotation = new Vector3(0, _horizontalAngle, 0);

        transform.eulerAngles = newPlayerRotation; // set body rotation
        _mainCamera.transform.eulerAngles = new Vector3(_verticalAngle, _horizontalAngle, 0); // set camera rotation up/down and left/right




    }
    
    
}
