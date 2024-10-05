using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    public float parryWindow = 0.5f;  // Time window for a successful parry
    private bool isParrying = false;
    private float parryStartTime;
    private Animator animator;
    private BoxCollider parryCollider;
    private Rigidbody rb;
    private int parryHash;
    private PlayerMovement playerMovement;

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

        // if (isParrying && Time.time > parryStartTime + parryWindow)
        // {
        //     isParrying = false;
        //     animator.SetBool(parryHash, false);
        //     rb.constraints = RigidbodyConstraints.FreezeRotation;
        //     parryCollider.enabled = false;
        // }
    }

    void TryToParry()
    {
        isParrying = true;
        parryStartTime = Time.time;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        parryCollider.enabled = true;
    }

    public void SuccessfulParry()
    {
        Debug.Log("Parry Successful!");
        EndParry();
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    public void EndParry()
    {
        isParrying = false;
        animator.SetBool(parryHash, false);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
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
