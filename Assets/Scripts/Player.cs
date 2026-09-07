using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public static Player Instance { get; set; }
    
    // private static Player instance; // Это то же что и выше
    // public static Player Instance
    // {
    //     get
    //     { 
    //         return instance;
    //     }
    //     set
    //     {
    //         instance = value;
    //     }
    // }
    
    public static Player instanceField;

    public static Player GetInstanceField()
    {
        return instanceField;
    }

    public static void SetInstanceField(Player instanceField)
    {
         Player.instanceField = instanceField;
    }
    
    
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float playerRadius = .7f;
    [SerializeField] private float playerHeigh = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Awake()
    {
        if (Instance != null )
        {
            Debug.Log("There is more than one Player instance!");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }
    
    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }
 
    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;    
        }
        
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }

            // ClearCounter clearCounter = raycastHit.transform.GetComponent<ClearCounter>();
            // if (clearCounter != null) {} // Это то же что и выше, просто null проверка делается сама внутри if 
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeigh, playerRadius, moveDir, moveDistance);

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        else
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeigh, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                transform.position += moveDirX * moveDistance;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeigh, playerRadius, moveDirZ, moveDistance);
                
                if (canMove)
                {
                    transform.position += moveDirZ * moveDistance;
                }
            }
        }

        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
    
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs()
        {
            selectedCounter = selectedCounter
        });
    }
}
