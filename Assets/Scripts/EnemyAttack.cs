using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public int polyDamage;
    public CapsuleCollider attackHitbox;
    [HideInInspector] public bool isStunned;
    private Animator animator;
    private bool canParry;
    private int stunnedHash;

    [Header("Audio")]
    public AudioClip attackClip;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        stunnedHash = Animator.StringToHash("OnStunned");

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(stunnedHash, isStunned);

        if (isStunned) {
            attackHitbox.enabled = false;
        }
    }

    void Unstun() {
        isStunned = false;
    }

    void CanBeParriedStart() {
        canParry = true;
    }

    void AttackStart() {
        audioSource.PlayOneShot(attackClip);
        attackHitbox.enabled = true;
    }

    void CanBeParriedEnd() {
        canParry = false;
    }

    void AttackEnd() {
        attackHitbox.enabled = false;
    }

    public bool GetCanParry(){
        return canParry;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.root.gameObject.CompareTag("Player") && !other.isTrigger) {
            other.transform.root.GetComponent<PlayerLevelScript>().LosePoly(polyDamage);
            other.transform.root.GetComponent<PlayerParry>().Hit();
        }
    }
}
