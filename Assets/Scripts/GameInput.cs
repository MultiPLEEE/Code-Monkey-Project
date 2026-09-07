using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);

        // if (OnInteractAction != null)
        // {
        //     OnInteractAction(this, EventArgs.Empty); // Это то же что и выше, просто null проверка делается компактнее
        // } 
    }
    
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        
        inputVector.Normalize();
        
        return inputVector;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
