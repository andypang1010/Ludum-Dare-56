using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState {
        IDLE,
        WALK,
        SPRINT,
        CROUCH
    }

    [Header("Movement")]
    public float sprintSpeed;
    public float walkSpeed;
    public float groundDrag;
    [HideInInspector] public MovementState movementState;
    private float moveSpeed;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchScale;
    private float defaultScale;

    [Header("Slope Check")]
    public float maxSlopeAngle;
    // public bool Grounded { get; private set; }
    RaycastHit slopeHit;
    public float playerHeight { get; private set; } = 1.8f;
    bool exitingSlope;

    [Header("Step Check")]
    public bool stepClimbEnabled;
    public GameObject rayLower;
    public GameObject rayUpper;
    public float stepHeight;
    public float stepSmoothing;

    Rigidbody rb;
    Vector3 moveDirection;
    float horizontalInput, verticalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rayUpper.transform.position = rayLower.transform.position + stepHeight * Vector3.up;
    }

    void Update()
    {
        
        // Check if is grounded
        // Grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);
        // exitingSlope = !Grounded;

        GetInput();
        SpeedControl();
        SetDrag();
        HandleMovementState();
    }

    void FixedUpdate() {
        Move();

        if (stepClimbEnabled) {
            StepClimb();
        }
    }

    void GetInput()
    {
        Vector2 movement = InputController.Instance.GetWalkDirection();
        horizontalInput = movement.x;
        verticalInput = movement.y;

        if (InputController.Instance.GetCrouchDown())
        {
            Crouch();
            return;
        }


        if ((!InputController.Instance.GetCrouchHold())
        && !Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.2f))
        {
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        }
    }

    void HandleMovementState() {
        if (InputController.Instance.GetCrouchHold()
        || (movementState == MovementState.CROUCH 
        && !InputController.Instance.GetCrouchHold()
        && Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.2f))) {
        // && Physics.CapsuleCast(transform.position + GetComponentInChildren<CapsuleCollider>().bounds.center + Vector3.down * playerHeight * 0.5f, transform.position + GetComponentInChildren<CapsuleCollider>().bounds.center + Vector3.up * playerHeight * 0.5f, 0.5f, Vector3.up, 0.5f))) {
            movementState = MovementState.CROUCH;
            moveSpeed = crouchSpeed;
        }

        else if (InputController.Instance.GetSprint()) {
            movementState = MovementState.SPRINT;
            moveSpeed = sprintSpeed;
        }


        else if (InputController.Instance.GetWalkDirection().magnitude > 0) {
            movementState = MovementState.WALK;
            moveSpeed = walkSpeed;
        }

        else {
            movementState = MovementState.IDLE;
            moveSpeed = 0;
        }
    }

    void Move() {
        moveDirection = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;

        // Apply force perpendicular to slope's normal if on slope
        if (OnSlope() && !exitingSlope) {
            rb.AddForce(20 * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);

            // Apply downward force to keep player on slope
            if (rb.velocity.y > 0) {
                rb.AddForce(Vector3.down * (movementState == MovementState.CROUCH ? 40f : 80f), ForceMode.Force);
            }
        }

        rb.AddForce(10 * moveSpeed * moveDirection, ForceMode.Force);
    }

    
    void SetDrag()
    {
        rb.drag = groundDrag;
    }

    void SpeedControl() {
        // Prevents player from exceeding move speed on slopes
        if (OnSlope() && !exitingSlope && rb.velocity.magnitude > moveSpeed) {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        else {
            Vector3 rawVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            // Clamp x and z axis velocity
            if (rawVelocity.magnitude > moveSpeed) {
                Vector3 clampedVelocity = rawVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
            }
        }
    }

    
    public void Crouch()
    {
        // Shrink to crouch size
        transform.localScale = new Vector3(transform.localScale.x, crouchScale, transform.localScale.z);

        // Apply downward force so doesn't float
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    void StepClimb() {
        Debug.DrawRay(rayLower.transform.position, rayLower.transform.forward * 0.1f, Color.red);
        Debug.DrawRay(rayUpper.transform.position, rayUpper.transform.forward * 0.2f, Color.red);

        if (Physics.Raycast(rayLower.transform.position, rayLower.transform.forward, out _, 0.15f)
        && !Physics.Raycast(rayUpper.transform.position, rayUpper.transform.forward, out _, 0.3f)) {
            rb.position += new Vector3(0f , stepSmoothing, 0f);
        }
    }

    bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public Vector3 GetMoveVelocity() {
        if (rb == null) {
            return Vector3.zero;
        }
        
        return new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }
    
    public MovementState GetMovementState() {
        return movementState;
    }
}
