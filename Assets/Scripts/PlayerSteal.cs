using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteal : MonoBehaviour
{
    public BoxCollider stealCollider;

    [Header("Audio")]
    public AudioClip stealClip;
    private AudioSource audioSource;

    private bool canSteal;
    private bool isStealing;
    private Animator animator;
    private int stealHash;
    private EnemyAI currentTarget;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private PlayerLevelScript playerLevel;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerLevel = GetComponent<PlayerLevelScript>();
        stealHash = Animator.StringToHash("OnSteal");

        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (InputController.Instance.GetStealDown()
        && !isStealing && canSteal
        && (playerMovement.movementState == PlayerMovement.MovementState.IDLE
        || playerMovement.movementState == PlayerMovement.MovementState.CROUCH))
        {
            animator.SetBool(stealHash, true);
        }
    }

    void StartSteal()
    {
        audioSource.PlayOneShot(stealClip);
        isStealing = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void EndSteal()
    {
        isStealing = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (currentTarget != null) {
            playerLevel.GainPoly(currentTarget.stealCount);
            currentTarget.wasStolen = true;
        }

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

                if (isStealing) {
                    currentTarget = enemyAI;
                    enemyAI.UISteal.SetActive(false);
                }

                else {
                    currentTarget = null;
                    enemyAI.UISteal.SetActive(true);
                }
            }
        }

        else
        {
            canSteal = false;
            currentTarget = null;
        }
    }
}
