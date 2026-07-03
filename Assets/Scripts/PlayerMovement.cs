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
}