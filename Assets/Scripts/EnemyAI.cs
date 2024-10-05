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
    public float detectRange, attackRange;

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

    private void Patrol() {
        if (Vector3.Distance(transform.position, walkpoints[currentWalkpointIndex].position) < 1f) {
            currentWalkpointIndex = (currentWalkpointIndex + 1) % walkpoints.Count;
        }

        agent.SetDestination(walkpoints[currentWalkpointIndex].position);
    }

    private void StopMoving(bool freezeRotY) {
        agent.isStopped = true;

        GetComponent<Rigidbody>().constraints = 
            RigidbodyConstraints.FreezePosition 
            | (freezeRotY ? RigidbodyConstraints.FreezeRotation : 

            RigidbodyConstraints.FreezeRotationX 
            | RigidbodyConstraints.FreezeRotationZ);
    }

    private void ContinueMoving() {
        agent.isStopped = false;

        GetComponent<Rigidbody>().constraints = 
            RigidbodyConstraints.FreezeRotationX 
            | RigidbodyConstraints.FreezeRotationZ;
    }

    private void SetAnimationBool() {

        Vector3 velocity = agent.velocity;
        velocity -= transform.up * velocity.y;
        print("Velocity in x and z: " + velocity);

        animator.SetFloat(velXHash, velocity.x);
        animator.SetFloat(velZHash, velocity.z);

        switch (currentState) {
            case ActionState.IDLE:
                agent.isStopped = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                animator.SetBool(idleHash, true);
                animator.SetBool(walkHash, false);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, false);

                break;

            case ActionState.PATROL:
                agent.isStopped = false;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                animator.SetBool(idleHash, false);
                animator.SetBool(walkHash, true);
                animator.SetBool(runHash, false);
                animator.SetBool(attackHash, false);

                break;
            case ActionState.CHASE:
                agent.isStopped = false;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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

}