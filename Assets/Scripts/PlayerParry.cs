using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    private bool isParrying;
    private Animator animator;
    private PlayerMovement playerMovement;
    public BoxCollider parryCollider;
    private Rigidbody rb;
    private int parryHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        parryCollider = GetComponent<BoxCollider>();
        parryCollider.enabled = false;

        parryHash = Animator.StringToHash("OnParry");
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.Instance.GetParryDown()
        && !isParrying

        // Check if player can parry
        && (playerMovement.movementState == PlayerMovement.MovementState.IDLE
        || playerMovement.movementState == PlayerMovement.MovementState.WALK))
        {
            animator.SetBool(parryHash, true);

            return;
        }
    }

    void StartParry()
    {
        isParrying = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        parryCollider.enabled = true;
    }

    public void SuccessfulParry()
    {
        // Debug.Log("Parry Successful!");
        EndParry();
    }

    public void EndParry()
    {
        isParrying = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        parryCollider.enabled = false;
        animator.SetBool(parryHash, false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.CompareTag("Enemy"))
        {
            if (other.transform.root.TryGetComponent(out EnemyAttack enemyAttack)

            && enemyAttack.GetCanParry()
            && isParrying == true)
            {
                SuccessfulParry();
                enemyAttack.isStunned = true;
            }
        }
    }
}
