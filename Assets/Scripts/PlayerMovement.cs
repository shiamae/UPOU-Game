using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    [Header("Mouse Look")]
    public float lookSpeed = 100f;
    public float lookXLimit = 85f;

    [Header("Crouch")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("Footsteps")]
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private float footstepTimer;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        moveInput = moveInput.normalized;

        bool isRunning = Keyboard.current.leftShiftKey.isPressed;

        float currentSpeed = walkSpeed;

        if (Keyboard.current.rKey.isPressed)
        {
            characterController.height = crouchHeight;
            currentSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            currentSpeed = isRunning ? runSpeed : walkSpeed;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float verticalVelocity = moveDirection.y;

        moveDirection =
            (forward * moveInput.y * currentSpeed) +
            (right * moveInput.x * currentSpeed);

        moveDirection.y = verticalVelocity;

        if (characterController.isGrounded)
        {
            if (moveDirection.y < 0)
            {
                moveDirection.y = -2f;
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame && canMove)
            {
                moveDirection.y = jumpPower;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        HandleFootsteps(isRunning, moveInput);
    }

    void HandleMouseLook()
    {
        if (!canMove || playerCamera == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        rotationX -= mouseDelta.y * lookSpeed * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation =
            Quaternion.Euler(rotationX, 0f, 0f);

        transform.Rotate(
            Vector3.up,
            mouseDelta.x * lookSpeed * Time.deltaTime
        );
    }

    private void HandleFootsteps(bool isRunning, Vector2 moveInput)
    {
        // Only play footsteps when moving on the ground
        if (!characterController.isGrounded || moveInput == Vector2.zero)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer += Time.deltaTime;

        float interval = isRunning ? runStepInterval : walkStepInterval;

        if (footstepTimer >= interval)
        {
            footstepTimer = 0f;

            string ground = GetGroundType();

            if (AudioManager.Instance == null)
                return;

            if (ground == "Grass")
            {
                AudioManager.Instance.PlayGrassFootstep();
            }
            else
            {
                AudioManager.Instance.PlayConcreteFootstep();
            }
        }
    }

    private string GetGroundType()
    {
        RaycastHit hit;

        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, Vector3.down, out hit, 3f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grass"))
                return "Grass";

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Concrete"))
                return "Concrete";
        }

        return "";
    }
}