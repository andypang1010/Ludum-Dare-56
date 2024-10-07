using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState {
        IDLE,
        WALK,
        RUN,
        CROUCH
    }

    [Header("Movement")]
    public float sprintSpeed;
    public float walkSpeed;
    public float groundDrag;
    public MovementState movementState;
    public float PlayerHeight { get; private set; } = 1.8f;
    private float moveSpeed;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchScale;

    private Animator animator;
    private int idleHash, walkHash, crouchIdleHash, runHash;
    private int velXHash, velZHash;

    Rigidbody rb;
    Vector3 moveDirection;
    float horizontalInput, verticalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        idleHash = Animator.StringToHash("OnIdle");
        walkHash = Animator.StringToHash("OnWalk");
        crouchIdleHash = Animator.StringToHash("OnCrouch");
        runHash = Animator.StringToHash("OnRun");

        velXHash = Animator.StringToHash("Vel_X");
        velZHash = Animator.StringToHash("Vel_Z");
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
        SetAnimationBool();
    }

    void FixedUpdate() {
        Move();
    }

    void GetInput()
    {
        Vector2 movement = InputController.Instance.GetWalkDirection();

        horizontalInput = movement.x;
        verticalInput = movement.y;

        if (GetComponent<PlayerLevelScript>().currentLevel < 3) {
            return;
        }

        if (InputController.Instance.GetCrouchDown())
        {
            Crouch();
            return;
        }


        if ((!InputController.Instance.GetCrouchHold())
        && !Physics.Raycast(transform.position, Vector3.up, PlayerHeight * 0.5f + 0.2f))
        {
            // GetComponent<CapsuleCollider>().height = PlayerHeight;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.9f, 0);
        }
    }

    void HandleMovementState() {
        if (InputController.Instance.GetCrouchHold()) {
            movementState = MovementState.CROUCH;
            moveSpeed = crouchSpeed;
        }

        else if (InputController.Instance.GetSprint()) {
            movementState = MovementState.RUN;
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
        rb.AddForce(10 * moveSpeed * moveDirection, ForceMode.Force);
    }

    
    void SetDrag()
    {
        rb.drag = groundDrag;
    }

    void SpeedControl() {
        Vector3 rawVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // Clamp x and z axis velocity
        if (rawVelocity.magnitude > moveSpeed) {
            Vector3 clampedVelocity = rawVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
        }
    }
    
    public void Crouch()
    {
        // Shrink to crouch size
        // transform.localScale = new Vector3(transform.localScale.x, crouchScale, transform.localScale.z);
        GetComponent<CapsuleCollider>().height = PlayerHeight * crouchScale;
        GetComponent<CapsuleCollider>().center = new Vector3(0, 0.65f, 0);

        // Apply downward force so doesn't float
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
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

    void SetAnimationBool() {
        
        animator.SetFloat(velXHash, InputController.Instance.GetWalkDirection().x);
        animator.SetFloat(velZHash, InputController.Instance.GetWalkDirection().y);
        
        switch (movementState)
        {
            case MovementState.IDLE:
                animator.SetBool(idleHash, true);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, false);
                animator.SetBool(crouchIdleHash, false);
                break;

            case MovementState.WALK:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, true);
                animator.SetBool(runHash, false);
                animator.SetBool(crouchIdleHash, false);
                break;

            case MovementState.RUN:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, true);
                animator.SetBool(crouchIdleHash, false);
                break;

            case MovementState.CROUCH:
                if (moveDirection.magnitude > 0) {
                    animator.SetBool(idleHash, false);
                    animator.SetBool(walkHash, true);
                    animator.SetBool(runHash, false);
                    animator.SetBool(crouchIdleHash, true);
                }
                else {
                    animator.SetBool(idleHash, false);
                    animator.SetBool(walkHash, false);
                    animator.SetBool(runHash, false);
                    animator.SetBool(crouchIdleHash, true);
                }

                break;
        }
    }
}
