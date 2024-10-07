using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    public BoxCollider parryCollider;
    private bool isParrying, isHit;
    private Animator animator;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private int parryHash, hitHash;

    [Header("Audio")]
    public AudioClip parrySuccess;
    public AudioClip parryFailure;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        parryCollider = GetComponent<BoxCollider>();
        parryCollider.enabled = false;

        audioSource = GetComponent<AudioSource>();

        parryHash = Animator.StringToHash("OnParry");
        hitHash = Animator.StringToHash("OnHit");
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.Instance.GetParryDown()
        && !isParrying && !isHit

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

    void SuccessfulParry()
    {
        audioSource.PlayOneShot(parrySuccess);
        EndParry();
    }

    void EndParry()
    {
        isParrying = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        parryCollider.enabled = false;
        animator.SetBool(parryHash, false);
    }

    public void Hit()
    {
        if (isHit) return;

        audioSource.PlayOneShot(parryFailure);
        animator.SetBool(hitHash, true);
    }

    public void StartHit()
    {
        isHit = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void EndHit()
    {
        isHit = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator.SetBool(hitHash, false);
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
                GetComponent<PlayerLevelScript>().GainPoly(2 * enemyAttack.polyDamage);
                enemyAttack.isStunned = true;
            }
        }
    }
}
