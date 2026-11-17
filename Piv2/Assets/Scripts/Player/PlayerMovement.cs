using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    // fix all these variables at some point
    public bool isAir;
    private float _verticalAngle;
    private float _horizontalAngle = 0f;
    public float jumpForceUp = 10;
    public float jumpForceForward = 4f;

    public InputAction moveInput;
    public InputAction mouseInput;
    public InputAction jumpInput;
    private Vector3 K_Input;
    private Vector3 K_WishDir;
    private Vector3 M_WishDir;
    private Vector3 _currentDir;
    private Vector2 _mDelta;
    private float _jumpValue;

    private float _upperLookLimit = 60;
    private float _lowerLookLimit = -89f;

    public float gravityAcceleration = 9.8f;
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
        HandleJump();
        CalculatePlayerDirection();
    }



    private void RunGroundDetection()
    {
        if (isAir)
        {
        }
    }

    public float jumpCoolDownTimer = 1f;
    public float jumpCoolDownTimerResetValue = 1f;

    private void HandleJump()
    {
        jumpCoolDownTimer -= Time.fixedDeltaTime;
        
        CheckIsAir();
        if (_jumpValue == 1 && !isAir)
        {
            jumpCoolDownTimer = jumpCoolDownTimerResetValue;
            isAir = true;

            
            
            
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            Vector3 verticalVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            
            Vector3 wishDir = horizontalVelocity;

            float ForwardBoostAmount = horizontalVelocity.magnitude * 0.6f;
            
            Vector3 jumpVector = new Vector3(horizontalVelocity.x *0.6f, jumpForceUp, horizontalVelocity.z * 0.6f);

            rb.linearVelocity += jumpVector;


            //rb.AddForce(transform.up * 500);
        }
    }

    private void CheckIsAir()
    {
        if (Physics.Raycast(transform.position, transform.up*-1, out var hit, 1))
        {
            Debug.DrawRay(transform.position, transform.up*-1);
            if (hit.collider.CompareTag("Ground"))
            {
                isAir = false;
            }
            else
            {
                isAir = true;
            }
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
        M_WishDir.y = 0;
        
        
        Vector3 right = transform.right;
        right *= K_Input.x;
        Vector3 forward = transform.forward;
        forward *= K_Input.z;

        K_WishDir = right + forward;

        //scalar ts
        K_WishDir *= WishDirAmplifier;
        M_WishDir *= M_WishDirAmplifier;

        //ground movement
        if (K_Input.magnitude != 0 && !isAir)
        {
            
            
            Vector3 wishDir = new Vector3(K_WishDir.x * MaxSpeed, rb.linearVelocity.y, K_WishDir.z * MaxSpeed);
            Vector3 newVelocityDir = Vector3.MoveTowards(rb.linearVelocity, wishDir, maxGroundVelocityChange);
            rb.linearVelocity = newVelocityDir;
        }
        else if (isAir) // air control / steering
        {

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            

            if (horizontalVelocity.sqrMagnitude > 0.01f) // if i have momentum
            {
                Vector3 wishDir = M_WishDir * horizontalVelocity.magnitude; // calc wish dir with same magnitude as current dir

              //  horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, wishDir, maxAirVelocityChange); // move currDir towards wishDir
              horizontalVelocity = Vector3.RotateTowards(horizontalVelocity, wishDir * MaxAirSpeed,
                  airAcceleration * Time.fixedDeltaTime, 0);

            }
            HandleGravity(); // maybe we want different gravity acceleration in diff situations ? :3
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
            
            Vector3 wishDir = new Vector3(K_WishDir.x * MaxSpeed, rb.linearVelocity.y, K_WishDir.z * MaxSpeed);
            Vector3 newVelocityDir = Vector3.MoveTowards(rb.linearVelocity, wishDir, maxGroundVelocityChange);
            rb.linearVelocity = newVelocityDir;
        }
        
        
        
        
        
        
    }

    private void HandleGravity()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y - gravityAcceleration * Time.fixedDeltaTime, rb.linearVelocity.z);
    }
    
    
    


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