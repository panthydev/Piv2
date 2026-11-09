using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction moveInput;
    public InputAction mouseInput;
    private Vector3 K_WishDir;
    private Vector2 M_WishDir;
    private Vector2 M_Delta;

    private Camera _mainCamera = Camera.main;
    
    private void Awake()
    {
        Cursor.visible = false;
        moveInput.Enable();
        mouseInput.Enable();
    }

    private void Update()
    {
        GetPlayerInput();
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

    private void CalculateLook()
    {
        //perform rotation on the player transform Y axis and the Camera (head) X axis for up and down

        Vector3 playerEulerRotation = new Vector3(0, M_Delta.y, 0);
        Vector3 cameraEulerRotation = new Vector3(M_Delta.x, 0, 0);


        if (_mainCamera.transform.rotation.y > 89)
        {
            _mainCamera.transform.rotation. = new Vector3(_mainCamera.transform.rotation.x, 89, _mainCamera.transform.rotation.z);
        }
        _mainCamera.transform.Rotate(cameraEulerRotation);
        


    }
    
    
}
