using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnemyMovement : MonoBehaviour
{
    public enum MovementState
    {
        IDLE,
        WALK,
        RUN,
        CHASE,
        ATTACK
    }

    [Header("Movement")]
    public NavMeshAgent agent;
    public float sprintSpeed;
    public float walkSpeed;
    public float groundDrag;
    [HideInInspector] public MovementState movementState;
    private float moveSpeed;

    [Header("Slope Check")]
    public float maxSlopeAngle;
    // public bool Grounded { get; private set; }
    RaycastHit slopeHit;
    public float EnemyHeight { get; private set; } = 1.8f;
    bool exitingSlope;

    private Animator animator;
    private int idleHash, walkHash, crouchIdleHash, runHash;
    private int velXHash, velZHash;
    // For Patrolling and Moving Towards Player
    public EnemyAI enemyAI;
    public EnemyAttack enemyAttack;
    private Vector3 destination;
    public Vector3 walkpoint;
    public bool walkPointSet;
    public float walkPointRange;

    public Rigidbody rb;
    Vector3 moveDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        idleHash = Animator.StringToHash("OnIdle");
        walkHash = Animator.StringToHash("OnWalk");
        runHash = Animator.StringToHash("OnRun");
        velXHash = Animator.StringToHash("Vel_X");
        velZHash = Animator.StringToHash("Vel_Z");
    }

    void Update()
    {

        SpeedControl();
        SetDrag();

        HandleMovementState();
        SetAnimationBool();
    }

    void FixedUpdate()
    {
        if (enemyAI.GetCurrentState() == EnemyAI.ActionState.PATROL)
        {
            movementState = MovementState.WALK;
            // Move();
        }
        else if (enemyAI.GetCurrentState() == EnemyAI.ActionState.CHASE)
        {
            movementState = MovementState.WALK;
            // Move();
        }
        else if (enemyAI.GetCurrentState() == EnemyAI.ActionState.ATTACK)
        {
            movementState = MovementState.ATTACK;
            // Move();
        }
    }

    public Vector3 GetDestination()
    {
        return destination;
    }

    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
        // destination = position;
    }

    void HandleMovementState()
    {

    }

    void Move()
    {
        moveDirection = destination.normalized;

        // Apply force perpendicular to slope's normal if on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(20 * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);

            // Apply downward force to keep player on slope
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        rb.AddForce(10 * moveSpeed * moveDirection, ForceMode.Force);
    }


    void SetDrag()
    {
        rb.drag = groundDrag;
    }

    void SpeedControl()
    {
        // Prevents enemies from exceeding move speed on slopes
        if (OnSlope() && !exitingSlope && rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        else
        {
            Vector3 rawVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            // Clamp x and z axis velocity
            if (rawVelocity.magnitude > moveSpeed)
            {
                Vector3 clampedVelocity = rawVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
            }
        }
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, EnemyHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public Vector3 GetMoveVelocity()
    {
        if (rb == null)
        {
            return Vector3.zero;
        }

        return new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    public MovementState GetMovementState()
    {
        return movementState;
    }

    void SetAnimationBool()
    {

        animator.SetFloat(velXHash, InputController.Instance.GetWalkDirection().normalized.x);
        animator.SetFloat(velZHash, InputController.Instance.GetWalkDirection().normalized.y);

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
        }
    }
}
