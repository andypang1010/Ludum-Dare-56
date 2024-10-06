using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }

    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode altCrouchKey = KeyCode.LeftCommand;
    public KeyCode stealKey = KeyCode.E;
    public KeyCode parryKey = KeyCode.Space;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one InputController in scene");
        }

        Instance = this;
    }

    public Vector2 GetWalkDirection()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    // public Vector2 GetLookDirection() {
    //     return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    // }

    public bool GetSprint()
    {
        return Input.GetKey(sprintKey);
    }

    public bool GetCrouchDown()
    {
        return Input.GetKeyDown(crouchKey) ^ Input.GetKeyDown(altCrouchKey);
    }

    public bool GetCrouchHold()
    {
        return Input.GetKey(crouchKey) ^ Input.GetKey(altCrouchKey);
    }

    public bool GetCrouchUp()
    {
        return Input.GetKeyUp(crouchKey) ^ Input.GetKeyUp(altCrouchKey);
    }

    public bool GetStealDown()
    {
        return Input.GetKeyDown(stealKey);
    }

    public bool GetParryDown()
    {
        return Input.GetKeyDown(parryKey);
    }
}
