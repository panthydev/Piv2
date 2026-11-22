using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    
    //NOTES
    // TICK IS ON WALL, DO THE REST WITH ATTACHING TO WALK, FREEZING CONSTRAINTS, ONLY ONE JUMP OFF, GIVE JUMP BOOST ALONG WALLS NORMAL VECTOR
    
    // fix all these variables at some point
    
    public bool IsOnWall;
    public GameObject hitWall;
    public GameObject currentWall;
    private CapsuleCollider playerCollider;
    public bool isAir;
    private float _verticalAngle;
    private float _horizontalAngle = 0f;
    public float jumpForceUp = 10;
    public float jumpForceForward = 4f;
    public float SlideVelocityControl = 1f;
    public float LandingBoost = 5;
    public float SlideMinSpeedThreshold;
    public InputAction moveInput;
    public InputAction mouseInput;
    public InputAction jumpInput;
    public InputAction slideInput;
    private Vector3 K_Input;
    private Vector3 K_WishDir;
    private Vector3 M_WishDir;
    private Vector3 _currentDir;
    private Vector2 _mDelta;
    private float _jumpValue;
    private float _slideValue;

    private float _upperLookLimit = 60;
    private float _lowerLookLimit = -89f;

    public float gravityAcceleration = 9.8f;
    public float acceleration = 1.5f;
    public float airAcceleration = 1.5f;
    public float airResistance = 2;
    public float MaxSpeed = 10;
    public float MaxAirSpeed = 20;
    public float maxSlideSpeed = 20;
    public float tempSlideSpeed = 0;

    public Rigidbody rb;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        

        _verticalAngle = _mainCamera.transform.rotation.eulerAngles.x;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        moveInput.Enable();
        mouseInput.Enable();
        jumpInput.Enable();
        slideInput.Enable();
        
        
        
            playerCollider.height = 2f;
            playerCollider.center = new Vector3(
                playerCollider.center.x, 0f, playerCollider.center.z);
        
        
    }

    private void Update()
    {
        GetPlayerInput();
        CalculateLook();
    }

    private void FixedUpdate()
    {
        CheckIsAir();
      //  CheckForAir();
        CheckForWallRun();
        HandleSlide();
        HandleJump();
        CalculatePlayerDirection();
    }


    public bool _isSliding;
    private void HandleSlide()
    {
        bool shouldSlide = _slideValue > 0 && (rb.linearVelocity.magnitude > SlideMinSpeedThreshold || JustHitGround);
        
        if (shouldSlide && JustHitGround)
        {
                print("Gave speed bost");
                
                rb.linearVelocity = new Vector3(rb.linearVelocity.x * LandingBoost, rb.linearVelocity.y, // maybe give boost always, ? and not just from fall
                    rb.linearVelocity.z * LandingBoost);
                
            
                JustHitGround = false;
            
        }

        if (shouldSlide != _isSliding)
        {
            _isSliding = shouldSlide;

            if (_isSliding)
            {
                playerCollider.height = 1.5f;
                playerCollider.center = new Vector3(playerCollider.center.x, -0.25f, playerCollider.center.z);
                Debug.Log($"Bottom: resizing");

                Transform camTrans = _mainCamera.GetComponent<Transform>();
                camTrans.transform.localPosition = new Vector3(camTrans.transform.localPosition.x,
                    camTrans.transform.localPosition.y - 0.5f, camTrans.transform.localPosition.z);
            }
            else
            {
                
                Transform camTrans = _mainCamera.GetComponent<Transform>();
                camTrans.transform.localPosition = new Vector3(camTrans.transform.localPosition.x,
                    camTrans.transform.localPosition.y + 0.5f, camTrans.transform.localPosition.z);
                playerCollider.height = 2;
                playerCollider.center = new Vector3(playerCollider.center.x, 0, playerCollider.center.z);
            }
        }
    }

    private float OldYValue; //compare against new to see if in air
    private void CheckForAir()
    {
        
        
        if (transform.position.y != OldYValue)
        {
            isAir = true;
        }
        else
        {
            isAir = false;
        }

        OldYValue = rb.linearVelocity.y;
    }

    public float jumpCoolDownTimer = 1f;
    public float jumpCoolDownTimerResetValue = 1f;
    public bool JustHitGround;

    private void HandleJump()
    {
        jumpCoolDownTimer -= Time.fixedDeltaTime;
        
        
        if (_jumpValue == 1 && !isAir && !IsOnWall) //IsOnWall is temp maybe
        {
            jumpCoolDownTimer = jumpCoolDownTimerResetValue;
            isAir = true;
            
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            

            Vector3 ForwardBoost = horizontalVelocity.normalized * jumpForceForward;
            

            rb.linearVelocity += new Vector3(ForwardBoost.x, jumpForceUp, ForwardBoost.z);
        }
    }

    private void CheckIsAir()
    {
        if (Physics.Raycast(transform.position, transform.up*-1.1f, out var hit, 1.1f))
        {
            Debug.DrawRay(transform.position, transform.up*-1);
            if (hit.collider.CompareTag("Ground"))
            {
                if (isAir)
                {
                    JustHitGround = true;
                }
                isAir = false; // hit ground
            }
            else
            {
                isAir = true;
            }
        }
        else
        {
            isAir = true;
        }
    }

    private void GetPlayerInput()
    {
        K_Input = moveInput.ReadValue<Vector2>().normalized;
        K_Input.z = K_Input.y;
        K_Input.y = 0;

        _mDelta = mouseInput.ReadValue<Vector2>();
        _jumpValue = jumpInput.ReadValue<float>();
        _slideValue = slideInput.ReadValue<float>();
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

        //ground movement
        if (K_Input.magnitude != 0 && !isAir && !IsOnWall)
        {
            float targetMaxSpeed = MaxSpeed;
            
            
            if (!_isSliding)
            {
                tempSlideSpeed = maxSlideSpeed;
            }
            if (_isSliding)
            {
                maxGroundVelocityChange *= SlideVelocityControl;
                
                tempSlideSpeed = Mathf.Lerp(tempSlideSpeed, MaxSpeed, 0.01f);
                targetMaxSpeed = tempSlideSpeed;
            }
           
            
            Vector3 wishDir = new Vector3(K_WishDir.x * targetMaxSpeed, rb.linearVelocity.y, K_WishDir.z * targetMaxSpeed);
            Vector3 newVelocityDir = Vector3.MoveTowards(rb.linearVelocity, wishDir, maxGroundVelocityChange);
            rb.linearVelocity = newVelocityDir;
           // HandleGravity();
        }
        else if (isAir && !IsOnWall) // air control / steering
        {

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            

            if (horizontalVelocity.sqrMagnitude > 0.01f && K_Input.magnitude > 0) // if i have momentum
            {
                float speed = horizontalVelocity.magnitude;

                float speedFactor = Mathf.InverseLerp(0f, MaxAirSpeed, speed);
                float steeringAmount = airAcceleration * speedFactor * Time.fixedDeltaTime;

                Vector3 wishDir = K_WishDir.normalized * speed;
                //Vector3 wishDir = M_WishDir * speed; // calc wish dir with same magnitude as current dir
                
                
               horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, wishDir, steeringAmount); // move currDir towards wishDir
            //  horizontalVelocity = Vector3.RotateTowards(horizontalVelocity, wishDir * MaxAirSpeed,
              //    airAcceleration * Time.fixedDeltaTime, 0);

            }
           // HandleGravity(); // maybe we want different gravity acceleration in diff situations ? :3
            //always apply air resistance

            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity,
                Vector3.zero,
                airResistance * Time.fixedDeltaTime);


            rb.linearVelocity = new Vector3(horizontalVelocity.x,
                rb.linearVelocity.y,
                horizontalVelocity.z);
        } else if (IsOnWall && isAir)
        {
            //
        }
        else
        {
            
            Vector3 wishDir = new Vector3(K_WishDir.x * MaxSpeed, rb.linearVelocity.y, K_WishDir.z * MaxSpeed);
            Vector3 newVelocityDir = Vector3.MoveTowards(rb.linearVelocity, wishDir, maxGroundVelocityChange);
            rb.linearVelocity = newVelocityDir;
            
        }
        HandleGravity();
    }

    private void HandleGravity()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y - gravityAcceleration * Time.fixedDeltaTime, rb.linearVelocity.z);
    }

    private void CheckForWallRun()
    {
        //this method should only check for walls and keep hitWall obj null or filled.

        
        
        RaycastHit tempHit = new RaycastHit();

        GameObject TempLeftWall = null;
        GameObject TempRightWall = null;

        Ray leftRay = new Ray(transform.position, transform.right * -1);
        Ray rightRay = new Ray(transform.position, transform.right);
        
        
        Debug.DrawRay(leftRay.origin, leftRay.direction, Color.red);
        Debug.DrawRay(rightRay.origin, rightRay.direction, Color.blue);
        
        
        if (Physics.Raycast(rightRay, out var rightHit, 1f))
        {
            TempRightWall = rightHit.transform.gameObject;
        }
        if (Physics.Raycast(leftRay, out var leftHit,1))
        {
            TempLeftWall = leftHit.transform.gameObject;
        }
        
        SetHitWall(TempLeftWall, TempRightWall, leftHit, rightHit); // gets set to null if no walls present.
        
        IsOnWall = currentWall == null ? IsOnWall = false : IsOnWall = true;






        // do ray cast left and right
        // if it hits, always save it.
        //if in the air, set the CURRENT WALL to be equal to the saved wall... this keeps them seperate
        // turn off "is in air" bool


        //if raycast doesnt hit, make "hitWall" null and current wall null.



        // somewhere else, when hitwall != null, constrain/freeze local RIGHT axis (maybe set player RIGHT axis movement to 0 while on wall), make gravity lower, pin player velocity along the wall,
        // maybe we should check camera forward vector against wall normal vector, camera forward vector needs to be within that 90 degree angle, else detach from wall.
        // when jumping, un-constrain RIGHT axis, give perform jump as normal but with higher values for more boost ?
    }

    private void SetHitWall(GameObject TempLeftWall, GameObject TempRightWall, RaycastHit leftHit, RaycastHit rightHit)
    {
        if (TempLeftWall != null && TempRightWall != null)
        {
            
            hitWall = leftHit.distance > rightHit.distance ? rightHit.transform.gameObject : leftHit.transform.gameObject; // select closest wall
            
            if (isAir)
            {
                currentWall = hitWall;
                isAir = false;
            }

        }
        else if(TempLeftWall != null && TempRightWall == null)
        {
            hitWall = TempLeftWall;
            if (isAir)
            {
                currentWall = hitWall;
                isAir = false;
            }
            
        }
        else if(TempLeftWall == null && TempRightWall != null)
        {
            hitWall = TempRightWall;
            if (isAir)
            {
                currentWall = hitWall;
                isAir = false;
            }
        }
        else
        {
            hitWall = null;
            currentWall = null;
        }
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
- add walljumps
- add slide visuals
- add wall hold/grab (optional)
- maybe fix animations ?
- fix object pickup / set objHolder to be at object origin, THEN parent object to objHolder. move/rotate objHolder up/down with mouse 
*/