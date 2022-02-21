using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;

    [Header("Movement Parameters")]
    [SerializeField] private float WalkSpeed = 3.0f;
    [SerializeField] private float Gravity = 10.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float LookSpeedX = 3.0f;
    [SerializeField, Range(1, 10)] private float LookSpeedY = 3.0f;
    [SerializeField, Range(1, 180)] private float UpperLookLimit = 90.0f;
    [SerializeField, Range(1, 180)] private float DownLookLimit = 90.0f;

    private Camera PlayerCamera;
    private CharacterController charactercontroller;

    private Vector3 MoveDirection;
    private Vector2 CurrentInnput;

    private float RotationX = 0;
    // Start is called before the first frame update
    void Awake()
    {
        invoke();
    }

    private void invoke()
    {
        PlayerCamera = GetComponentInChildren<Camera>();
        charactercontroller = GetComponentInChildren<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.visible)
            invoke();

        if (CanMove)
        {
            HandleMovement();
            HandleMouseLook();
            ApplyFinalmoves();
        }
    }
    private void HandleMovement()
    {
        CurrentInnput = new Vector2(WalkSpeed * Input.GetAxis("Vertical"), WalkSpeed * Input.GetAxis("Horizontal"));

        float Movedir = MoveDirection.y;
        MoveDirection = (transform.TransformDirection(Vector3.forward) * CurrentInnput.x) + (transform.TransformDirection(Vector3.right) * CurrentInnput.y);
        MoveDirection.y = Movedir;
    }
    private void HandleMouseLook()
    {
        RotationX -= Input.GetAxis("Mouse Y") * LookSpeedY;
        RotationX = Mathf.Clamp(RotationX, -UpperLookLimit, DownLookLimit);
        PlayerCamera.transform.localRotation = Quaternion.Euler(RotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * LookSpeedX, 0);
    }
    private void ApplyFinalmoves()
    {
        if (!charactercontroller.isGrounded)
        {
            MoveDirection.y -= Gravity * Time.deltaTime;
        }
        charactercontroller.Move(MoveDirection * Time.deltaTime);
    }
}
