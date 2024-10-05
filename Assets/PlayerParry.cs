using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    public float parryWindow = 0.5f;  // Time window for a successful parry
    private bool isParrying = false;
    private float parryStartTime;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.Instance.GetParryDown())
        {
            TryToParry();
        }
        if (isParrying && Time.time > parryStartTime + parryWindow)
        {
            isParrying = false;
        }
    }

    void TryToParry()
    {
        isParrying = true;
        parryStartTime = Time.time;
    }
    public void SuccessfulParry()
    {
        Debug.Log("Parry Successful!");
        isParrying = false;
    }

    public bool IsParrying()
    {
        return isParrying;
    }
}
