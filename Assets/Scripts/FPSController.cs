using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 1f;

    private CharacterController controller;
    private Camera playerCamera;
    private float yaw = 0f;
    private float pitch = 0f;
    private bool isCrouching = false;
    private Weapon currentWeapon;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting = false;

    void Start()
    {
        InitializeComponents();
        SetupPlayerInput();
        LockCursor(true);
        SetCrouchState(false);
        currentWeapon = GetComponentInChildren<Weapon>();
    }
    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void InitializeComponents()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void SetupPlayerInput()
    {
        if (GetComponent<PlayerInput>() == null)
        {
            gameObject.AddComponent<PlayerInput>();
        }
    }

    private void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void SetCrouchState(bool crouching)
    {
        isCrouching = crouching;
        controller.height = isCrouching ? crouchHeight : standHeight;

        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = isCrouching ? 0.3f : 0.6f;
        playerCamera.transform.localPosition = camPos;
    }

    private void HandleMouseLook()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = GetCurrentSpeed();
        controller.Move(move * speed * Time.deltaTime);
    }

    private float GetCurrentSpeed()
    {
        if (isCrouching) return crouchSpeed;
        if (isSprinting && moveInput.y > 0) return sprintSpeed;
        return walkSpeed;
    }
    public void OnShoot(InputValue value)
    {
        if (currentWeapon != null)
        {
            currentWeapon.Shoot();
        }
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            SetCrouchState(!isCrouching);
        }
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }
}