using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Animator animator;
    private bool isAttacking;

    private bool canParry;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
