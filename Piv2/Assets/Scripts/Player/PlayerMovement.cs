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
    private Vector3 M_WishDir;
    private Vector3 _currentDir;
    private Vector2 _mDelta;

    public Rigidbody rb;

    private Camera _mainCamera;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

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
        CalculatePlayerDirection();
    }

    private void GetPlayerInput()
    {
        K_WishDir = moveInput.ReadValue<Vector2>().normalized;
        K_WishDir.z = K_WishDir.y;
        K_WishDir.y = 0;
        print("Keyboard Direction: "+ K_WishDir);

        _mDelta = mouseInput.ReadValue<Vector2>();
       // print("Mouse input: " + _mDelta);
    }




    [SerializeField] private float WishDirAmplifier = 2;
    [SerializeField] private float M_WishDirAmplifier = 0.1f;
    [SerializeField] private float C_CurrentDirAmplifier = 1;
   
    private void CalculatePlayerDirection()
    {
        //new direction is the result of the current direction and the inputDirection and the look direction
        //keep current direction as a stored variable, NOT rb direction
        
        // set linear velocity directly
        M_WishDir = _mainCamera.transform.forward;
        
        

        K_WishDir *= WishDirAmplifier;
        M_WishDir *= M_WishDirAmplifier;
        _currentDir *= C_CurrentDirAmplifier;

        _currentDir = K_WishDir + M_WishDir + _currentDir;
        _currentDir = Vector3.ClampMagnitude(_currentDir, 10);
        
        

        rb.linearVelocity = _currentDir;



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
