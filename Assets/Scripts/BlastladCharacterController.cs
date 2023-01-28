using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using System;
using Cinemachine;

public enum CharacterState
{
    Default,
    Sliding,
    longJumping,
    wallBump,
    GroundPounding,
    shooting
}

public struct PlayerInputs
{
    public float moveAxisForward;
    public float moveAxisRight;
    public Quaternion cameraRotation;

    public bool jumpButtonDown;//if the jump button has been pressed
    public bool slideButtonDown;// if the slide button has been pressed
    public bool shootButtonDown;//if the Shooting Button has been pressed
}

public class BlastladCharacterController : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor Motor;

    [Header("Testing GameObjects")]
    public GameObject defaultModel;
    public GameObject slideModel;


    [SerializeField]
    Transform followPoint;

    [SerializeField]
    //TutorialCamera cam;

    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f;
    public float StableMovementSharpness = 15;
    public float OrientationSharpness = 10;
    public float MaxStableDistanceFromLedge = 5f;
    [Range(0f, 180f)]
    public float MaxStableDenivelationAngle = 180f;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 10f;
    public float AirAccelerationSpeed = 5f;
    public float Drag = 0.1f;

    [Header("Jumping")]
    public bool AllowJumpingWhenSliding = false;//this is for slopes
    public float maxJumpHeight = 3;// Max jump if held down
    public float minJumpHeight = 1;// min jump if released
    public float timeToJumpApex = .4f;//Time to reach max height
    public float maxJumpUpSpeed = 10f;// Jump up speed
    public float minJumpUpSpeed = 3f;// Jump speed when released + velocity
    public float currentJumpUpSpeed;// the jump speed being applied by the class at this moment...appplies to the current character Y direction only 
    public float JumpScalableForwardSpeed = 10f;// for the player charactes horizontal movement on a jump
    public float JumpPreGroundingGraceTime = 0f;// if you can buffer a jump right before you land
    public float JumpPostGroundingGraceTime = 0f;// if you can perform a jummp right after you leave ground

    [Header("Sliding")]
    public float slideSpeed = 5;// The speed for the player's slide
    public float slidePreGroundingGraceTime = 0f;//If you can buffer a slide right before you land
    public float maxStableSlideSpeed = 10f;
    public float slidePressTime = 0;
    public float _minSlideTime = .1f;
    public float slideTimeCoolDown = .1f;

    [Header("LongJumping")]
    public float longJumpSpeed = 5;
    public float longJumpVerticalSpeed = 6;

    [Header("WallBump")]
    public float BumpAirMoveSpeed = 2f;
    public float wallBumpStunTime = 0.1f;
    float timeSinceWallHit = Mathf.Infinity;
    private bool canMoveFromWall = false;

    [Header("GroundPound")]
    public float groundPoundSpeed = 3f;
    public float groundPoundHangTime = 0.1f;

    [Header("Shooting")]
    public float MaxStableShootSpeed = 7f;
    public float StableShootSharpness = 15;
    public float OrientationShootSharpness = 10;
    public float MaxStableDistanceFromLedgeShoot = 5f;

    [Header("Air Movement Shoot")]
    public float MaxAirMoveSpeedShoot = 10f;
    public float AirAccelerationSpeedShoot = 5f;
    [SerializeField]
    PlayerActions playerActions;

    [Header("Misc")]
    public List<Collider> IgnoredColliders = new List<Collider>();
    public bool OrientTowardsGravity = false;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;

    public CharacterState CurrentCharacterState { get; private set; }

    private Collider[] _probedColliders = new Collider[8];
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;

    private bool justLanded = false;
    private bool _jumpRequested = false;//Step 1: A Jump is Requested...reset to false after Step 2
    private float _timeSinceJumpRequested = Mathf.Infinity;// As well as setting this to 0
    private bool _jumpConsumed = false;//Step 2: after jump velocity is applied say we consumed it
    private bool _jumpedThisFrame = false;//Always set equal to false every velocity update BEFORE it checks if a jump was requested...After step 2 say we jumped this frame
    private bool _dropRequested = false;
    private bool _dropConsumed = false;
    private bool _slideRequested = false;
    private float _timeSinceSlideRequested = Mathf.Infinity;
    private float _timeSinceSlideStarted = 0;
    private float _slideCoolDownTimer = 0;
    private bool _slideConsumed = false;
    private bool _slideThisFrame = false;
    private bool _isSlideButtonDown = false;
    private bool _canSlide = true;
    private float _timeSinceLastAbleToJump = 0f;
    private Vector3 _internalVelocityAdd = Vector3.zero;
    private Vector3 wallHitNormal;
    bool wallHit = false;

    public bool _longJumpRequested = false;
    public float _timeSinceLongJumpRequested = Mathf.Infinity;
    public bool _longJumpedThisFrame = false;
    public bool _longJumpConsumed = false;

    private bool _shouldBeCrouching = false;
    private bool _isCrouching = false;

    private bool _canGroundPound = true;
    private bool _mustStopHorizontalVelocity = false;
    private float timeSincePoundPressed = 0f;
    private bool isNowGroundPounding = false;

    private Vector3 _currentSlideDirection;
    private bool _isStopped;
    private bool _mustStopVelocity = false;
    private float _timeSinceStartedCharge = 0;
    private float _timeSinceStopped = 0;

    bool isShooting = false;

    private void Start()
    {
        playerActions = new PlayerActions();

        // Assign to motor
        Motor.CharacterController = this;

        //This set of 4 lines deterines the local gravity based off of the heights of the jumps and it's speeds
        Gravity.y = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpUpSpeed = Mathf.Abs(Gravity.y) * timeToJumpApex;
        minJumpUpSpeed = Mathf.Sqrt(2 * Math.Abs(Gravity.y) * minJumpHeight);
        Debug.Log("Gravity" + Gravity + " Jump Velocity" + maxJumpUpSpeed);

        // Handle initial state
        TransitionToState(CharacterState.Default);
    }

    /// <summary>
    /// Handles movement state transitions and enter/exit callbacks
    /// </summary>
    public void TransitionToState(CharacterState newState)
    {
        CharacterState tmpInitialState = CurrentCharacterState;
        OnStateExit(tmpInitialState, newState);
        CurrentCharacterState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    public void ActivateLowProfile()
    {
        _shouldBeCrouching = true;
        if (!_isCrouching)
        {
            _isCrouching = true;
            Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
            followPoint.position = new Vector3(followPoint.transform.position.x, (followPoint.transform.position.y - 0.5f), followPoint.transform.position.z);
            defaultModel.SetActive(false);
            slideModel.SetActive(true);
        }
    }

    /// <summary>
    /// Event when entering a state
    /// </summary>
    ///
    [SerializeField]
    Transform orientationFromPlayer;
    public void OnStateEnter(CharacterState state, CharacterState fromState)
    {
        switch (state)
        {
            case CharacterState.Default:
                {
                    if (!_isCrouching)
                    {
                        Debug.Log("THERE");
                        defaultModel.SetActive(true);
                        slideModel.SetActive(false);
                    }
                    _shouldBeCrouching = false;//new here
                    break;
                }
            case CharacterState.Sliding:
                {
                    if (_moveInputVector != Vector3.zero)
                    {
                        _currentSlideDirection = _moveInputVector;
                    }
                    else
                    {
                        _currentSlideDirection = _lookInputVector;
                    }
                    _timeSinceStopped = 0f;
                    ActivateLowProfile();
                    break;
                }
            case CharacterState.longJumping:
                {
                    Debug.Log("HERE");
                    defaultModel.SetActive(false);
                    slideModel.SetActive(true);
                    break;
                }
            case CharacterState.wallBump:
                {
                    canMoveFromWall = false;
                    timeSinceWallHit = 0;
                    break;
                }
            case CharacterState.GroundPounding:
                {
                    _canGroundPound = false;//you can't ground pound if you are already groundpounding
                    _mustStopHorizontalVelocity = true;
                    timeSincePoundPressed = 0f;
                    break;
                }
            case CharacterState.shooting:
                {
                    isShooting = true;
                    break;
                }
        }
    }

    private IEnumerator BeginWallStun(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        canMoveFromWall = true;
    }
    /// <summary>
    /// Event when exiting a state
    /// </summary>
    public void OnStateExit(CharacterState state, CharacterState toState)
    {
        switch (state)
        {
            case CharacterState.Default:
                {
                    break;
                }
            case CharacterState.Sliding:
                {
                    break;
                }
            case CharacterState.longJumping:
                {
                    _longJumpConsumed = false;
                    _longJumpedThisFrame = false;
                    _timeSinceLongJumpRequested = 0;
                    break;
                }
            case CharacterState.wallBump:
                {
                    canMoveFromWall = false;
                    break;
                }
            case CharacterState.GroundPounding:
                {
                    _canGroundPound = true;//after leaving gp you can than GroundPound again under normal circumstances
                    isNowGroundPounding = false;
                    break;
                }
            case CharacterState.shooting:
                {
                    isShooting = false;
                    break;
                }
        }
    }

    /// <summary>
    /// This is called every frame by MyPlayer in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref PlayerInputs inputs)
    {
        // Handle state transition from input


        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.moveAxisRight, 0f, inputs.moveAxisForward), 1f);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, Motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);


        //NEW DOWN
        if (inputs.shootButtonDown && !isShooting)
        {
            TransitionToState(CharacterState.shooting);
        }
        else if (!inputs.shootButtonDown && isShooting)
        {
            TransitionToState(CharacterState.Default);
        }
        //NEW UP^^^

        if (inputs.slideButtonDown && !_slideConsumed && Motor.GroundingStatus.IsStableOnGround && _canSlide)
        {
            _slideRequested = true;
            _moveInputVector = cameraPlanarRotation * moveInputVector;
            Debug.Log(_moveInputVector);
            _timeSinceSlideRequested = 0f;
            _timeSinceSlideStarted = 0;
            TransitionToState(CharacterState.Sliding);
        }
        else if (inputs.slideButtonDown && !Motor.GroundingStatus.IsStableOnGround && _canGroundPound && CurrentCharacterState != CharacterState.wallBump)//if drill was pressed and currently in air (Drill ground pound) *All new below* and we are not already ground pounding - can groundpound
        {
            TransitionToState(CharacterState.GroundPounding);
        }
        //^^^^^^^All new


        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    // Move and look inputs
                    _moveInputVector = cameraPlanarRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;

                    // Jumping input
                    if (inputs.jumpButtonDown)
                    {
                        _jumpRequested = true;
                        _timeSinceJumpRequested = 0f;
                        currentJumpUpSpeed = maxJumpUpSpeed;
                        Debug.Log("JUMP IS HELD");
                    }

                    break;
                }
            case CharacterState.Sliding:
                {
                    if (inputs.jumpButtonDown)
                    {
                        _longJumpRequested = true;
                        _timeSinceLongJumpRequested = 0f;
                        Debug.Log("Drill Jump");
                    }
                    break;
                }
            case CharacterState.wallBump:
                {
                    // Move and look inputs
                    _moveInputVector = cameraPlanarRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;
                    break;
                }
            case CharacterState.shooting:
                {
                    _moveInputVector = cameraPlanarRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;//I think this is only used for rotation
                    break;
                }
        }
    }

    public void SetJumpSpeed(bool maxOrMin)//Primarily used to cancel the jump when the button is released
    {
        if (!Motor.GroundingStatus.IsStableOnGround)
        {
            currentJumpUpSpeed = minJumpUpSpeed;
            _dropRequested = true;
        }
    }

    public Vector3 GetVelocity() { return Motor.Velocity; }//Retuns current velocity(Cant change it tho)

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called before the character begins its movement update
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    break;
                }
            case CharacterState.Sliding:
                {

                    // Update times
                    _timeSinceSlideStarted += deltaTime;
                    if (_timeSinceSlideStarted > _minSlideTime /*&& !_isSlideButtonDown*/)
                    {
                        EndSlide(false);
                    }
                    break;
                }
            case CharacterState.wallBump:
                {
                    timeSinceWallHit += deltaTime;
                    if (timeSinceWallHit > wallBumpStunTime)
                    {
                        canMoveFromWall = true;
                    }
                    if (!canMoveFromWall)
                    {
                        _moveInputVector = Vector3.zero;
                    }

                    break;
                }
            case CharacterState.GroundPounding:
                {
                    timeSincePoundPressed += deltaTime;
                    if (timeSincePoundPressed > groundPoundHangTime)
                    {
                        isNowGroundPounding = true;
                    }
                    //something to wait before the pound takes place
                    break;
                }
        }


        if (!_canSlide)
        {
            _slideCoolDownTimer += deltaTime;
            if (_slideCoolDownTimer >= slideTimeCoolDown)
            {
                _canSlide = true;
            }
        }
    }


    public void EndSlide(bool transistionLJ)//maybe with a state 
    {
        _slideConsumed = false;
        _slideRequested = false;
        _slideCoolDownTimer = 0;
        _canSlide = false;
        TransitionToState(CharacterState.Default);
        if (transistionLJ)
        {
            //TransitionToState(CharacterState.longJumping);
        }
    }
    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    if (_moveInputVector != Vector3.zero && OrientationSharpness > 0f)
                    {
                        //Debug.Log("DEFAULT");
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _moveInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }
                    break;
                }
            case CharacterState.Sliding:
                {
                    if (_slideRequested)
                    {

                        if (_currentSlideDirection == Vector3.zero)
                        {
                            _currentSlideDirection = Motor.CharacterForward;
                        }
                        else
                        {
                            currentRotation = Quaternion.LookRotation(_currentSlideDirection, Motor.CharacterUp);
                        }
                    }
                    break;
                }
            case CharacterState.wallBump:
                {
                    if (_moveInputVector != Vector3.zero && OrientationSharpness > 0f && canMoveFromWall)
                    {
                        Debug.Log("WALLBUMP STATE");
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _moveInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }
                    break;
                }
            case CharacterState.shooting:
                {
                    if (OrientationShootSharpness > 0f)
                    {
                        var forward = Camera.main.transform.forward;

                        Vector3 desiredMoveDirection = forward;
                        Quaternion lookAtRotation = Quaternion.LookRotation(desiredMoveDirection);
                        Quaternion lookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                        currentRotation = Quaternion.Slerp(transform.rotation, lookAtRotationOnly_Y, .3f);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 targetMovementVelocity = Vector3.zero;
        switch (CurrentCharacterState)
        {

            case CharacterState.Default:
                {
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        // Reorient velocity on slope
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;

                        if (justLanded)
                        {
                            Debug.Log("THIS WAS REACHED");
                            targetMovementVelocity = reorientedInput * 0;
                            currentVelocity = new Vector3(0, currentVelocity.y, 0);//or zero
                            justLanded = false;

                        }
                        else
                        {
                            // Smooth movement Velocity
                            targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        }

                    }
                    else// in air
                    {
                        // Add move input
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                            // Prevent climbing on un-stable slopes with air movement
                            if (Motor.GroundingStatus.FoundAnyGround)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                            }

                            Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                            currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }

                    // Handle jumping
                    {
                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;
                        if (_jumpRequested && currentJumpUpSpeed == maxJumpUpSpeed)
                        {
                            // See if we actually are allowed to jump (Jump has not already been uses and is on ground or within coyote time values)
                            if (!_jumpConsumed && ((Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                            {
                                // Calculate jump direction before ungrounding current character orientation up
                                Vector3 jumpDirection = Motor.CharacterUp;
             
                                Debug.Log(jumpDirection + " curret velocity");
                                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                {
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;//seems to be for ledges?
                                }
                             

                                // Makes the character skip ground probing/snapping on its next update. 
                                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                Motor.ForceUnground();

                                // Add to the return velocity and reset jump state
                                currentVelocity += (jumpDirection * currentJumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                            }
                        }
                        else if (_dropRequested && currentJumpUpSpeed == minJumpUpSpeed && currentVelocity.y > 0f)
                        {
                            Debug.Log("JUMP MOMENT CANCEL");
                            if (_jumpConsumed && !_dropConsumed)
                            {
                                Vector3 postdropVelocity = currentVelocity + ((Motor.GroundingStatus.GroundNormal * currentJumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp));
                                if (postdropVelocity.y >= currentVelocity.y)
                                {
                                    _dropConsumed = true;
                                }
                                else
                                {
                                    currentVelocity += (Motor.GroundingStatus.GroundNormal * currentJumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                    _dropConsumed = true;
                                }
                            }

                            _dropRequested = false;
                        }
                    }

                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }
                    break;
                }
            case CharacterState.Sliding:
                {
                    bool hitWall = false;
                    if (wallHit)
                    {
                        Vector3 bump;
                        // Calculate jump direction before ungrounding
                        currentVelocity = Vector3.zero;

                        Vector3 bumpDirection = wallHitNormal;

                        Vector3 jumpDirection = Motor.CharacterUp;
                        if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                        {
                            jumpDirection = Motor.GroundingStatus.GroundNormal;//seems to be for ledges?
                        }

                        // Makes the character skip ground probing/snapping on its next update. 
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.

                        Motor.ForceUnground();


                        // Add to the return velocity and reset jump state
                        currentVelocity += (jumpDirection * 6) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        currentVelocity += (bumpDirection * 5);

                        wallHit = false;
                        hitWall = true;
                        EndSlide(false);
                        TransitionToState(CharacterState.wallBump);//ahbvduydavdyuvauyvdwvyuwvuywvwauyvwavwavywayvw
                    }



                    _slideThisFrame = false;
                    _timeSinceSlideRequested += deltaTime;
                    if (_slideRequested)
                    {
                        Debug.Log(currentVelocity);
                        currentVelocity = Vector3.zero;
                        //if you have not already slid and are on ground currently..this is for the inital slide
                        if (!_slideConsumed && Motor.GroundingStatus.IsStableOnGround)
                        {
                            Vector3 slideDirection = Motor.CharacterForward;

                            //currentVelocity += (slideDirection * slideSpeed) - Vector3.Project(currentVelocity, Motor.CharacterForward);
                            currentVelocity = Vector3.Lerp(currentVelocity, ((slideDirection * slideSpeed) - Vector3.Project(currentVelocity, Motor.CharacterForward)), 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));

                            _slideRequested = false;
                            _slideConsumed = true;
                            _slideThisFrame = true;
                            _timeSinceSlideStarted = 0f;
                        }
                    }

                    if (Motor.GroundingStatus.IsStableOnGround && !_slideThisFrame && _slideConsumed && !hitWall)
                    {
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
                        //currentVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
                        targetMovementVelocity = Motor.CharacterForward * maxStableSlideSpeed;
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                    }

                    _longJumpedThisFrame = false;
                    _timeSinceLongJumpRequested += deltaTime;
                    if (_longJumpRequested)//should cahnge
                    {
                        if (!_longJumpConsumed && ((Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                        {

                            Vector3 longJumpVertDirection = Motor.CharacterUp;
                            if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                            {
                                longJumpVertDirection = Motor.GroundingStatus.GroundNormal;//seems to be for ledges?
                            }

                            Vector3 longJumpDirection = Motor.CharacterForward;

                            Motor.ForceUnground();

                            // Add to the return velocity and reset jump state
                            currentVelocity += (longJumpDirection * longJumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                            currentVelocity += (longJumpVertDirection * longJumpVerticalSpeed);

                            Debug.Log("SLDIE JUMP");

                            _longJumpRequested = false;
                            _longJumpedThisFrame = true;
                            _longJumpConsumed = true;
                            EndSlide(true);
                            TransitionToState(CharacterState.longJumping);
                        }
                    }


                    break;
                }
            case CharacterState.longJumping:
                {
                    bool hitWall = false;
                    if (wallHit)
                    {
                        Vector3 bump;
                        // Calculate jump direction before ungrounding
                        currentVelocity = Vector3.zero;

                        Vector3 bumpDirection = wallHitNormal;

                        Vector3 jumpDirection = Motor.CharacterUp;
                        if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                        {
                            jumpDirection = Motor.GroundingStatus.GroundNormal;//seems to be for ledges?
                        }

                        // Makes the character skip ground probing/snapping on its next update. 
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.

                        Motor.ForceUnground();


                        // Add to the return velocity and reset jump state
                        currentVelocity += (jumpDirection * 6) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        currentVelocity += (bumpDirection * 5);

                        wallHit = false;
                        hitWall = true;
                        _longJumpConsumed = false;
                        TransitionToState(CharacterState.wallBump);//was default
                    }



                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                    }
                    else
                    {
                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }
                    break;
                }
            case CharacterState.wallBump:
                {
                    // Add move input
                    if (_moveInputVector.sqrMagnitude > 0f && canMoveFromWall)
                    {
                        targetMovementVelocity = _moveInputVector * BumpAirMoveSpeed;

                        // Prevent climbing on un-stable slopes with air movement
                        if (Motor.GroundingStatus.FoundAnyGround)
                        {
                            Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                            targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                        }

                        Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                        currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                    }


                    if (Motor.GroundingStatus.IsStableOnGround)
                    {

                    }
                    else
                    {
                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }
                    break;
                }
            case CharacterState.GroundPounding:
                {

                    if (_mustStopHorizontalVelocity)
                    {
                        currentVelocity = Vector3.zero;
                        _mustStopHorizontalVelocity = false;
                    }

                    if (isNowGroundPounding)
                    {
                        // When charging, velocity is always constant
                        float previousX = currentVelocity.x;
                        float previousZ = currentVelocity.z;
                        currentVelocity = new Vector3(previousX, -groundPoundSpeed, previousZ);
                        currentVelocity += Gravity * deltaTime;
                    }
                    break;
                }
            case CharacterState.shooting:
                {
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        // Reorient velocity on slope
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;

                        if (justLanded)
                        {
                            Debug.Log("THIS WAS REACHED");
                            targetMovementVelocity = reorientedInput * 0;
                            currentVelocity = new Vector3(0, currentVelocity.y, 0);//or zero
                            justLanded = false;

                        }
                        else
                        {
                            // Smooth movement Velocity
                            targetMovementVelocity = reorientedInput * MaxStableShootSpeed;
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableShootSharpness * deltaTime));
                        }

                    }
                    else// in air
                    {
                        // Add move input
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * MaxAirMoveSpeedShoot;

                            // Prevent climbing on un-stable slopes with air movement
                            if (Motor.GroundingStatus.FoundAnyGround)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                            }

                            Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                            currentVelocity += velocityDiff * AirAccelerationSpeedShoot * deltaTime;
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {
        if (_slideRequested && _timeSinceSlideRequested > slidePreGroundingGraceTime)
        {
            _slideRequested = false;
        }


        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    // Handle jump-related values
                    {
                        // Handle jumping pre-ground grace period
                        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                        {
                            _jumpRequested = false;
                        }

                        if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                        {
                            // If we're on a ground surface, reset jumping values
                            if (!_jumpedThisFrame)
                            {
                                _jumpConsumed = false;
                                _dropConsumed = false;
                            }
                            _timeSinceLastAbleToJump = 0f;
                        }
                        else
                        {
                            // Keep track of time since we were last able to jump (for grace period)
                            _timeSinceLastAbleToJump += deltaTime;
                        }
                    }

                    break;
                }
            case CharacterState.Sliding:
                {

                    break;
                }
            case CharacterState.longJumping:
                {

                    break;
                }
        }

        // Handle uncrouching
        if (_isCrouching && !_shouldBeCrouching)
        {
            // Do an overlap test with the character's standing height to see if there are any obstructions
            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
            if (Motor.CharacterCollisionsOverlap(
                    Motor.TransientPosition,
                    Motor.TransientRotation,
                    _probedColliders) > 0)
            {
                // If obstructions, just stick to crouching dimensions
                Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
            }
            else
            {
                // If no obstructions, uncrouch
                _isCrouching = false;
                defaultModel.SetActive(true);
                slideModel.SetActive(false);
                followPoint.transform.position = new Vector3(followPoint.transform.position.x, followPoint.transform.position.y + .5f, followPoint.transform.position.z);
            }
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (IgnoredColliders.Contains(coll))
        {
            return false;
        }
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    break;
                }
            case CharacterState.Sliding:
                {
                    // Detect being stopped by obstructions
                    if (!hitStabilityReport.IsStable)//ground was here
                    {
                        wallHitNormal = hitNormal;
                        wallHit = true;
                        Debug.Log("Wall Bump");
                    }
                    break;
                }
            case CharacterState.longJumping:
                {
                    Debug.Log("WALL BUMP");
                    if (!Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                    {
                        wallHitNormal = hitNormal;
                        wallHit = true;
                    }
                    break;
                }
        }
    }


    public void OnLanded()
    {
        if (_moveInputVector == Vector3.zero)
        {

            justLanded = true;
        }

        switch (CurrentCharacterState)
        {
            case CharacterState.longJumping:
                {
                    Debug.Log("LANDED");
                    _longJumpConsumed = false;
                    TransitionToState(CharacterState.Default);
                    break;
                }
            case CharacterState.wallBump:
                {
                    _longJumpConsumed = false;
                    TransitionToState(CharacterState.Default);
                    break;
                }
            case CharacterState.GroundPounding:// eventually should have some form of small delay after landing with gp before the olayer can move again
                {
                    TransitionToState(CharacterState.Default);
                    break;
                }
        }
    }

    public void OnLeaveStableGround()
    {
   
    }

    public void AddVelocity(Vector3 velocity)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    _internalVelocityAdd += velocity;
                    break;
                }
            case CharacterState.shooting:
                {
                    _internalVelocityAdd += velocity;
                    break;
                }
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {

        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
        {
            //  Debug.Log("POST GROUND REACHED");

            OnLeaveStableGround();
        }
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }
}

