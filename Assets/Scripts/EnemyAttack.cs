using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool isStunned;
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
    }

    void Unstun() {
        isStunned = false;
    }

    void CanBeParriedStart() {
        canParry = true;
    }

    void CanBeParriedEnd() {
        canParry = false;
    }
    public bool GetCanParry(){
        return canParry;
    }
}
