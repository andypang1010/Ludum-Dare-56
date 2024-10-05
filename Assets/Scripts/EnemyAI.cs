using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Callbacks;
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
    public Transform playerTransform;
    private NavMeshAgent agent;

    [Header("Patrolling")]
    public List<Transform> walkpoints;
    public int currentWalkpointIndex = 0;


    [Header("Sensors")]
    public float fovAngle = 150f; // Field of view angle (90 degrees)
    public float sightDistance = 10f; // Max distance the enemy can see
    public float attackRange = 2f;

    private Animator animator;
    private int idleHash, walkHash, runHash, attackHash;

    private EnemyAttack enemyAttack;

    void Start()
    {
        playerTransform = GameObject.Find("PLAYER").transform;

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
    }

    void Update()
    {
        if (!enemyAttack.isStunned) {
            
            if (IsPlayerInFieldOfView() && IsPlayerVisible()) {
                float distanceToPlayer = (playerTransform.position - transform.position).magnitude;

                // Attack
                if (distanceToPlayer <= attackRange)
                {
                    agent.SetDestination(playerTransform.position);

                    SetCurrentState(ActionState.ATTACK);
                    StopMoving();
                }

                // Chase
                else if (distanceToPlayer <= sightDistance)
                {
                    agent.SetDestination(playerTransform.position);

                    SetCurrentState(ActionState.CHASE);
                    ContinueMoving();
                }
            }
            
            else
            {
                if (walkpoints.Count > 1) {
                    Patrol();
                    ContinueMoving();

                    SetCurrentState(ActionState.PATROL);
                }

                else {
                    SetCurrentState(ActionState.IDLE);

                    StopMoving();
                }
            }
        }

        else {
            StopMoving();
        }

        SetAnimationBool();
    }

    public void SetCurrentState(ActionState state)
    {
        currentState = state;
    }

    public ActionState GetCurrentState()
    {
        return currentState;
    }

    private void Patrol() {
        if (Vector3.Distance(transform.position, walkpoints[currentWalkpointIndex].position) < 1f) {
            currentWalkpointIndex = (currentWalkpointIndex + 1) % walkpoints.Count;
        }

        agent.SetDestination(walkpoints[currentWalkpointIndex].position);
    }

    private void StopMoving() {
        agent.isStopped = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void ContinueMoving() {
        agent.isStopped = false;

        GetComponent<Rigidbody>().constraints = 
            RigidbodyConstraints.FreezeRotationX 
            | RigidbodyConstraints.FreezeRotationZ;
    }

    private void SetAnimationBool() {
        switch (currentState) {
            case ActionState.IDLE:
                animator.SetBool(idleHash, true);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, false);

                break;

            case ActionState.PATROL:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, true);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, false);

                break;

            case ActionState.CHASE:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, true);
                animator.SetBool(attackHash, false);

                break;

            case ActionState.ATTACK:
                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, true);

                break;
        }
    }
    public bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleBetweenEnemyAndPlayer <= fovAngle / 2f;
    }

    public bool IsPlayerVisible()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightDistance))
        {
            if (hit.transform == playerTransform)
            {
                return true;
            }
        }
        return false;
    }
}