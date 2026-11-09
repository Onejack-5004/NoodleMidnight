
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 80f;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;

    private bool cursorLocked = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        LockCursor();
    }

    void Update()
    {
        HandleCursor();
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        if (Keyboard.current == null)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        // WASD Movement
        if (Keyboard.current.wKey.isPressed)
            vertical += 1f;

        if (Keyboard.current.sKey.isPressed)
            vertical -= 1f;

        if (Keyboard.current.aKey.isPressed)
            horizontal -= 1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal += 1f;

        Vector3 moveDirection =
            transform.right * horizontal +
            transform.forward * vertical;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movement =
            moveDirection * moveSpeed +
            Vector3.up * verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (!cursorLocked || Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Horizontal Look
        float mouseX = mouseDelta.x * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        // Vertical Look
        float mouseY = mouseDelta.y * mouseSensitivity;

        cameraPitch -= mouseY;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            -maxLookAngle,
            maxLookAngle
        );

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    void HandleCursor()
    {
        if (Keyboard.current == null)
            return;

        // ESC: Unlock mouse temporarily for testing
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (cursorLocked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }

        // Click to lock mouse again
        if (!cursorLocked &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            LockCursor();
        }
    }
}
