using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AttackState
    {
        PATROL,
        CHASE,
        ATTACK
    }
    public NavMeshAgent agent;
    public AttackState currentState;
    //Patrolling
    public Transform playerTransform;
    public Vector3 walkpoint;
    public bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float fovAngle = 150f; // Field of view angle (90 degrees)
    public float sightDistance = 5f; // Max distance the enemy can see

    public float attackRange = 2f;

    //For switching states
    public float detectRange;

    // public EnemyMovement enemyMovement;
    public EnemyAttack enemyAttack;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        playerTransform = GameObject.Find("PLAYER").transform;
    }
    void Update()
    {
        float distanceToPlayer = (playerTransform.position - transform.position).magnitude;
        if (IsPlayerInFieldOfView() && IsPlayerVisible())
        {
            if (distanceToPlayer <= attackRange)
            {
                switchState(AttackState.ATTACK);
                agent.SetDestination(playerTransform.position);
                // enemyMovement.SetDestination(playerTransform.position);
            }
            else
            {
                switchState(AttackState.CHASE);
                agent.SetDestination(playerTransform.position);
                // enemyMovement.SetDestination(playerTransform.position);
            }
        }
        else
        {
            switchState(AttackState.PATROL);

            if (!walkPointSet)
            {
                SearchWalkPoint();
            }

            if (walkPointSet)
            {
                float distanceToWalkPoint = Vector3.Distance(transform.position, walkpoint);
                if (distanceToWalkPoint < 1f)
                {
                    walkPointSet = false;
                }
            }

            agent.SetDestination(walkpoint);
        }
    }

    public void switchState(AttackState state)
    {
        this.currentState = state;
    }

    public AttackState getState()
    {
        return currentState;
    }

    public void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        walkPointSet = true;
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