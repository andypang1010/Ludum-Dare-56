using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public bool isStunned;
    public CapsuleCollider attackHitbox;
    public PlayerLevelScript playerLevel;
    public int polyDamage;
    private Animator animator;
    private bool canParry;
    private int stunnedHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        stunnedHash = Animator.StringToHash("OnStunned");
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
            playerLevel.LosePoly(polyDamage);
            other.transform.root.GetComponent<PlayerParry>().Hit();
            print("Hit");
        }
    }
}
