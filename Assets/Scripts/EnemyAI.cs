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

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float fovAngle = 150f; // Field of view angle (90 degrees)
    public float sightDistance = 5f; // Max distance the enemy can see

    public float attackRange = 2f;

    //For switching states
    public float detectRange;

    private Animator animator;
    private int idleHash, walkHash, runHash, attackHash;
    private int velXHash, velZHash;

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

        velXHash = Animator.StringToHash("Vel_X");
        velZHash = Animator.StringToHash("Vel_Z");

        // Add current position as a walkpoint
        GameObject homeWalkpoint = new GameObject("WALKPOINT 0");
        homeWalkpoint.transform.position = transform.position;

        walkpoints.Add(homeWalkpoint.transform);
    }

    void Update()
    {
        if (!enemyAttack.isStunned) {
            
            if (IsPlayerInFieldOfView() && IsPlayerVisible())
            {}
            // TODO: If player is seen, then check distance to go into Attack or Chase; else, just patrol or stand around
            float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

            // Attack
            if (distanceToPlayer <= attackRange)
            {
                SetCurrentState(ActionState.ATTACK);

                StopMoving(false);
                agent.SetDestination(playerTransform.position);
            }

            // Chase
            else if (distanceToPlayer <= detectRange)
            {
                SetCurrentState(ActionState.CHASE);

                ContinueMoving();
                agent.SetDestination(playerTransform.position);
            }

            // Player not seen
            else
            {
                if (walkpoints.Count > 1) {
                    SetCurrentState(ActionState.PATROL);

                    ContinueMoving();
                    Patrol();
                }

                else {

                    StopMoving(false);
                    SetCurrentState(ActionState.IDLE);
                }
            }
        }

        else {
            StopMoving(true);
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


    }

}