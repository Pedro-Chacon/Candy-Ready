using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Config Player")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] public static int moneyScore = 0;

    [Header("Reference Inputs")]
    [SerializeField] InputActionReference moveAction;

    Vector2 movement;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    
    void Update()
    {
        ReadInput();
        Movement();
    }

    void ReadInput()
    {
        movement = moveAction.action.ReadValue<Vector2>().normalized;
    }

    void Movement()
    {
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
    }
}
