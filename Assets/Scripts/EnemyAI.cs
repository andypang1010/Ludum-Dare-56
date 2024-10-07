using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum ActionState
    {
        IDLE,
        PATROL,
        CHASE,
        ATTACK
    }

    [Header("Pathfinding")]
    public ActionState currentState;
    public Vector3 targetPosition;
    public float maxDetectTime;
    private float lastDetectedTime;
    private bool detectedBefore;
    private NavMeshAgent agent;
    private GameObject player;
    private Transform cameraTransform;

    [Header("Patrolling")]
    public float patrolSpeed;
    public float chaseSpeed;
    public List<Transform> walkpoints;
    public int currentWalkpointIndex = 0;


    [Header("Sensors")]
    public float fovAngle = 150f; // Field of view angle (90 degrees)
    public float sightRange = 10f; // Max distance the enemy can see
    public float listenRange = 10f;
    public float attackRange = 2f;
    private PlayerMovement playerMovement;

    [Header("Polygon")]
    public bool wasStolen;
    public int stealCount;

    private Animator animator;
    private int idleHash, walkHash, runHash, attackHash;

    private EnemyAttack enemyAttack;

    [Header("UI Elements")]
    public GameObject UIExclamationMark;
    public GameObject UISteal;

    void Start()
    {
        player = GameObject.Find("PLAYER");
        cameraTransform = GameObject.Find("CAMERA").transform;

        playerMovement = GameObject.Find("PLAYER").GetComponent<PlayerMovement>();

        agent = GetComponent<NavMeshAgent>();


        enemyAttack = GetComponent<EnemyAttack>();

        animator = GetComponent<Animator>();
        idleHash = Animator.StringToHash("OnIdle");
        walkHash = Animator.StringToHash("OnWalk");
        runHash = Animator.StringToHash("OnRun");
        attackHash = Animator.StringToHash("OnAttack");

        // Add current position as a walkpoint
        GameObject homeWalkpoint = new GameObject("WALKPOINT 0");
        homeWalkpoint.transform.position = transform.position;

        walkpoints.Add(homeWalkpoint.transform);

        SetCurrentState(ActionState.IDLE);
        UIExclamationMark.SetActive(false);
    }

    void Update()
    {
        RotateUI();

        // print("Player Detected?: " + PlayerDetected());

        if (wasStolen) {
            Invoke(nameof(DisableWasStolen), 10f);
        }

        if (enemyAttack.isStunned)
        {
            detectedBefore = false;
            StopMoving();
            SetAnimationBool();
            return;
        }

        if ((PlayerDetected() || (detectedBefore && Time.time - lastDetectedTime < maxDetectTime) || wasStolen)
        && player.transform.root.gameObject.GetComponent<PlayerLevelScript>().currentLevel > 2)
        {
            
            UISteal.SetActive(false);

            if (DistanceToPlayer() <= attackRange)
            {
                SetCurrentState(ActionState.ATTACK);
            }

            else
            {
                SetCurrentState(ActionState.CHASE);
                agent.speed = chaseSpeed;
            }

            UIExclamationMark.SetActive(true);
            agent.SetDestination(player.transform.position);
        }

        // Player not detected
        else
        {
            // Patrol
            if (walkpoints.Count > 1)
            {
                SetCurrentState(ActionState.PATROL);
                agent.speed = patrolSpeed;

                if (detectedBefore && Time.time - lastDetectedTime > maxDetectTime)
                {
                    FindNearestPatrolPoint();

                    detectedBefore = false;
                }

                if (Vector3.Distance(transform.position, walkpoints[currentWalkpointIndex].position) < 1f)
                {
                    currentWalkpointIndex = (currentWalkpointIndex + 1) % walkpoints.Count;
                }

                targetPosition = walkpoints[currentWalkpointIndex].position;
            }

            // Idle
            else
            {
                SetCurrentState(ActionState.IDLE);
                targetPosition = transform.position;
            }

            agent.SetDestination(targetPosition);
            UIExclamationMark.SetActive(false);
        }

        SetAnimationBool();

    }

    private void RotateUI()
    {
        UIExclamationMark.transform.LookAt(cameraTransform.position + cameraTransform.rotation * Vector3.forward, Vector3.up);
        UISteal.transform.LookAt(cameraTransform.position + cameraTransform.rotation * Vector3.forward, Vector3.down);
    }

    void SetCurrentState(ActionState state)
    {
        currentState = state;
    }

    void FindNearestPatrolPoint()
    {
        Transform closestWalkpoint = null;
        float closestDistance = float.MaxValue;

        foreach (Transform walkpoint in walkpoints)
        {
            float distance = Vector3.Distance(walkpoint.position, transform.position);

            if (closestWalkpoint != null)
            {

                // If closer than previous closest distance
                if (distance < closestDistance)
                {

                    // Set as temporary closest patrol point
                    closestWalkpoint = walkpoint;
                    closestDistance = distance;
                }
            }

            else
            {
                closestWalkpoint = walkpoint;
                closestDistance = distance;
            }
        }

        // Go to closest patrol point
        currentWalkpointIndex = walkpoints.IndexOf(closestWalkpoint);
    }

    #region Detection
    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    private bool PlayerIsRunning()
    {
        return playerMovement.GetMovementState() == PlayerMovement.MovementState.RUN
        && playerMovement.GetMoveVelocity().magnitude > 0;
    }

    private bool PlayerIsWalking()
    {
        return playerMovement.GetMovementState() == PlayerMovement.MovementState.WALK
        && playerMovement.GetMoveVelocity().magnitude > 0;
    }

    public bool PlayerInFieldOfView()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleBetweenEnemyAndPlayer = Vector3.Angle(
            player.transform.position.y * transform.up + transform.forward,
            directionToPlayer);

        return angleBetweenEnemyAndPlayer <= fovAngle / 2f;
    }

    public bool PlayerVisible()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightRange, ~LayerMask.GetMask("Enemy")))
        {
            // print("Hit object: " + hit.transform.root.gameObject);
            if (hit.transform.root.gameObject == player.transform.gameObject)
            {
                // print("Is Visible?: " + true);
                return true;
            }
            // print("Is Visible?: " + false);
            return false;
        }

        // print("Is Visible?: " + false);
        return false;
    }

    private bool PlayerDetected()
    {
        if ((PlayerInFieldOfView() && PlayerVisible() && DistanceToPlayer() <= sightRange)
        || (PlayerIsRunning() && DistanceToPlayer() <= listenRange)
        || (PlayerIsWalking() && DistanceToPlayer() <= listenRange * 0.5f)
        )
        {
            detectedBefore = true;
            lastDetectedTime = Time.time;

            return true;
        }

        return false;
    }
    #endregion

    #region Animation
    private void StopMoving()
    {
        agent.isStopped = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void ContinueMoving()
    {
        agent.isStopped = false;

        GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    private void SetAnimationBool()
    {
        switch (currentState)
        {
            case ActionState.IDLE:
                animator.SetBool(idleHash, true);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, false);

                StopMoving();

                break;

            case ActionState.PATROL:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, true);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, false);

                ContinueMoving();

                break;

            case ActionState.CHASE:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, true);
                animator.SetBool(attackHash, false);

                ContinueMoving();

                break;

            case ActionState.ATTACK:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, enemyAttack.isStunned ? false : true);

                StopMoving();

                break;
        }
    }
    #endregion

    void DisableWasStolen() {
        wasStolen = false;
    }
}