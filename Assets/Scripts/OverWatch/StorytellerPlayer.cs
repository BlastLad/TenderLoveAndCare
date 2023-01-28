using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using UnityEngine.InputSystem;
using System.Linq;
using Cinemachine;
public class StorytellerPlayer : MonoBehaviour
{
    public Transform playerCamera;
    public Transform followPoint;
    [SerializeField]
    public BlastladCharacterController character;

    int paintBrushChargeMax = 2;
    public int currentPaintBrushCharges;
    bool isUsingPaintbrush = false;

    private Vector3 _lookInputVector = Vector3.zero;
    public Transform cameraLookAt;

    private float xRotation;
    [SerializeField]
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    public Transform cameraTransform;

    PlayerActions playerActions;
    public Vector2 cameraMovement;
    bool isCamMoving;

    Vector2 movementVector;
    bool isMoving;
    bool hasJumped;
    bool isSliding = false;
    bool isShooting = false;
    public Transform orientation;

    bool slideButtonHasBeenPressed = false;

    [SerializeField]
    //VacuumController vacuum;

    private void Awake()
    {
        currentPaintBrushCharges = paintBrushChargeMax;

        playerActions = new PlayerActions();

        cameraTransform = Camera.main.transform;

        playerActions.Tutorial.Camera.performed += ctx =>
        {
            cameraMovement = ctx.ReadValue<Vector2>();
            isCamMoving = cameraMovement.x != 0 || cameraMovement.y != 0;
        };
        playerActions.Tutorial.Camera.canceled += ctx =>
        {
            cameraMovement = ctx.ReadValue<Vector2>();
            isCamMoving = cameraMovement.x != 0 || cameraMovement.y != 0;
        };
        playerActions.Tutorial.Movement.started += ctx =>
        {
            movementVector = ctx.ReadValue<Vector2>();
            isMoving = movementVector.x != 0 || movementVector.y != 0;
        };
        playerActions.Tutorial.Movement.performed += ctx =>
        {
            movementVector = ctx.ReadValue<Vector2>();
            isMoving = movementVector.x != 0 || movementVector.y != 0;
        };
        playerActions.Tutorial.Movement.canceled += ctx =>
        {
            movementVector = ctx.ReadValue<Vector2>();
            isMoving = movementVector.x != 0 || movementVector.y != 0;
        };
        playerActions.Tutorial.Shoot.performed += ctx =>
        {
            if (!isUsingPaintbrush)
            {
                ShootButtonDown(true);
            }
            else
            {
                Paint();
            }
        };
        playerActions.Tutorial.Shoot.canceled += ctx =>
        {
            if (!isUsingPaintbrush)
            {
                ShootButtonDown(false);
            }
        };

        playerActions.Tutorial.Jump.canceled += ctx => CancelJump();
        playerActions.Tutorial.Slide.performed += ctx =>
        {
            if (currentPaintBrushCharges > 0)
            {
                EquipPaintBrush();
            }
        };
        //playerActions.Tutorial.Movement.performed += ctx =>  Debug.Log(ctx.ReadValue<Vector2>());
    }

    void Paint()
    {

    }
    public void EquipPaintBrush()
    {
        isUsingPaintbrush = true;
        ShootButtonDown(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        HandleCharacterInput();
    }

    private void FixedUpdate()
    {


    }
    private void LateUpdate()
    {
        HandleCameraInput();
    }

    private void HandleCharacterInput()
    {
        PlayerInputs pcharacterInputs = new PlayerInputs();

        pcharacterInputs.moveAxisForward = movementVector.y;
        pcharacterInputs.moveAxisRight = movementVector.x;
        pcharacterInputs.cameraRotation = cameraTransform.transform.rotation;
        pcharacterInputs.jumpButtonDown = playerActions.Tutorial.Jump.triggered;
        pcharacterInputs.shootButtonDown = isShooting;

        character.SetInputs(ref pcharacterInputs);
    }

    public Vector2 GetMouseDelta()
    {
        return playerActions.Tutorial.Camera.ReadValue<Vector2>();
    }

    private float desiredX;
    private void Look()
    {
        float mouseX = GetMouseDelta().x * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = GetMouseDelta().y * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = cameraTransform.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        cameraTransform.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    public void ShootButtonDown(bool val)
    {
        //hsvy
        if (val)
        {
            isShooting = true;
        }
        else if (!val)
        {
            isShooting = false;
        }
    }

    bool isAiming = false;
    public bool shooting
    {
        get { return isShooting; }
    }

    public void CancelJump()
    {
        character.SetJumpSpeed(false);
    }
    private void HandleCameraInput()
    {
        float axisVertical = cameraMovement.y;
        float axisHorizontal = cameraMovement.x;


        _lookInputVector = new Vector3(axisHorizontal, axisVertical, 0);
    }

    private void OnEnable()
    {
        playerActions.Enable();
    }

    private void OnDisable()
    {
        playerActions.Disable();
    }
}
