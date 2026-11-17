using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool isAir;
    private Vector3 _gravityVector;
    private float _verticalAngle;
    private float _horizontalAngle = 0f;

    public InputAction moveInput;
    public InputAction mouseInput;
    public InputAction jumpInput;
    private Vector3 K_Input;
    private Vector3 K_WishDir;
    private Vector3 M_WishDir;
    private Vector3 _currentDir;
    private Vector2 _mDelta;
    private float _jumpValue;


    public float acceleration = 1.5f;
    public float airAcceleration = 1.5f;
    public float airResistance = 2;
    public float MaxSpeed = 10;
    private float MaxAirSpeed = 20;

    public Rigidbody rb;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        _gravityVector = new Vector3(0, this.transform.up.y * -2, 0);

        _verticalAngle = _mainCamera.transform.rotation.eulerAngles.x;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        moveInput.Enable();
        mouseInput.Enable();
        jumpInput.Enable();
    }

    private void Update()
    {
        GetPlayerInput();
        CalculateLook();
    }

    private void FixedUpdate()
    {
        DoJump();
        CalculatePlayerDirection();
    }



    private void RunGroundDetection()
    {
        if (isAir)
        {
        }
    }

    private void DoJump()
    {
        if (_jumpValue == 1)
        {
            rb.AddForce(transform.forward * 500);
        }
    }

    private bool CanJump()
    {
        return false;
    }

    private void GetPlayerInput()
    {
        K_Input = moveInput.ReadValue<Vector2>().normalized;
        K_Input.z = K_Input.y;
        K_Input.y = 0;

        _mDelta = mouseInput.ReadValue<Vector2>();
        _jumpValue = jumpInput.ReadValue<float>();
    }

    [SerializeField] private float WishDirAmplifier = 2;
    [SerializeField] private float M_WishDirAmplifier = 0.1f;


    private void CalculatePlayerDirection()
    {
        float maxGroundVelocityChange = acceleration * Time.fixedDeltaTime;
        float maxAirVelocityChange = airAcceleration * Time.fixedDeltaTime; // for DE-accelerating usually.
        M_WishDir = _mainCamera.transform.forward;
        
        
        Vector3 right = transform.right;
        right *= K_Input.x;
        Vector3 forward = transform.forward;
        forward *= K_Input.z;

        K_WishDir = right + forward;

        //scalar ts
        K_WishDir *= WishDirAmplifier;
        M_WishDir *= M_WishDirAmplifier;


        if (K_Input.magnitude != 0 && !isAir)
        {
            Vector3 wishDir = K_WishDir * MaxSpeed;
            Vector3 newVelocityDir = Vector3.MoveTowards(rb.linearVelocity, wishDir, maxGroundVelocityChange);
            rb.linearVelocity = newVelocityDir;
        }
        else if (isAir) // air control / steering
        {

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (horizontalVelocity.sqrMagnitude > 0.01f)
            {
                Vector3 wishDir = M_WishDir * horizontalVelocity.magnitude;

                horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, wishDir, maxAirVelocityChange);
            }
            
            //always apply air resistance

            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity,
                Vector3.zero,
                airResistance * Time.fixedDeltaTime);


            rb.linearVelocity = new Vector3(horizontalVelocity.x,
                rb.linearVelocity.y,
                horizontalVelocity.z);
        }
        else
        {
            Vector3 wishDir = K_WishDir * MaxSpeed;
            Vector3 newVelocityDir = Vector3.MoveTowards(rb.linearVelocity, wishDir, maxGroundVelocityChange);
            rb.linearVelocity = newVelocityDir;
        }
    }

    private float _upperLookLimit = 60;
    private float _lowerLookLimit = -89f;


    private void CalculateLook()
    {
        //perform rotation on the player transform Y axis and the Camera (head) X axis for up and down
        _verticalAngle =
            Mathf.Clamp(_verticalAngle += _mDelta.y * -1, _lowerLookLimit,
                _upperLookLimit); // update stored camera angle from mouse input
        _horizontalAngle += _mDelta.x;

        Vector3 newPlayerRotation = new Vector3(0, _horizontalAngle, 0);

        transform.eulerAngles = newPlayerRotation; // set body rotation
        _mainCamera.transform.eulerAngles =
            new Vector3(_verticalAngle, _horizontalAngle, 0); // set camera rotation up/down and left/right
    }
}
/* TODO
- add ground detection via raycast
- add jumping
- add gravity
- need ramp-up to max speed, instead of insta top speed
- add slide
- add walljumps
- add wall hold/grab (optional)
- maybe fix animations ?
*/