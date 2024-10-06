using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteal : MonoBehaviour
{
    private bool canSteal;
    private bool isStealing;
    private Animator animator;
    private int stealHash;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    public BoxCollider stealCollider;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        stealHash = Animator.StringToHash("OnSteal");

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        print(InputController.Instance.GetStealDown()
        && !isStealing && canSteal
        && playerMovement.movementState == PlayerMovement.MovementState.IDLE);

        if (InputController.Instance.GetStealDown()
        && !isStealing && canSteal
        && playerMovement.movementState == PlayerMovement.MovementState.IDLE)
        {
            animator.SetBool(stealHash, true);
        }
    }

    void StartSteal()
    {
        isStealing = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void EndSteal()
    {
        isStealing = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator.SetBool(stealHash, false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.CompareTag("Enemy"))
        {
            if (other.transform.root.TryGetComponent(out EnemyAI enemyAI)

            && (enemyAI.currentState == EnemyAI.ActionState.IDLE || enemyAI.currentState == EnemyAI.ActionState.PATROL))
            {
                canSteal = true;
            }
        }

        else
        {
            canSteal = false;
        }
    }
}
