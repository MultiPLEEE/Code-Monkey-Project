using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;

    private bool isWalking;
    
    // Update is called once per frame
    private void Update()
    {
        Vector2 inputVector = new Vector2(0, 0);
        
        if (Keyboard.current.wKey.isPressed)
        {
            inputVector.y = +1;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            inputVector.y = -1;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            inputVector.x = -1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            inputVector.x = +1;
        }
        
        inputVector.Normalize();
        
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        
        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
