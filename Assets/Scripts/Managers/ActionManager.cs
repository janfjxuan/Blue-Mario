using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public UnityEvent jump;
    public UnityEvent jumpRelease;
    public UnityEvent<int> moveCheck;
    public UnityEvent attack;

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jump.Invoke();
        }
        else if (context.canceled)
        {
            jumpRelease.Invoke();
        }
    }

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float raw = context.ReadValue<float>();
            int faceRight = raw > 0 ? 1 : -1;
            moveCheck.Invoke(faceRight);
        }
        if (context.canceled)
        {
            moveCheck.Invoke(0);
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) attack.Invoke();
    }

    public void OnPointAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 point = context.ReadValue<Vector2>();
        }
    }
}