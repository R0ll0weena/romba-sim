using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] float maxRayLength = 0.2f; // Adjust based on character size
    [SerializeField] private float collisionPadding = 0.01f;

    private PlayerInput controls;
    private Vector2 moveInput;
    private Rigidbody rb; // get ref from editor is better, but lazy solution here

    void Awake()
    {
        controls = new PlayerInput();
    }

    void OnEnable()
    {
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        // Rotate towards movement direction
        if (moveInput.sqrMagnitude > 0.01f) // Prevents rotation when not moving
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Translation
        /*if (IsGrounded())
		{
            //rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }*/

        // Translation
        if (IsGrounded())
        {
            
            Vector3 allowedMove = GetAllowedMove(move);
            //rb.MovePosition(rb.position + allowedMove  * Time.fixedDeltaTime );
            rb.linearVelocity = new Vector3(allowedMove.x, rb.linearVelocity.y, allowedMove.z);
            //rb.linearVelocity = new Vector3(move.x * allowedMove.x , rb.linearVelocity.y, move.z * allowedMove.z);
        }
    }

    private Vector3 GetAllowedMove(Vector3 move)
    {
        Vector3 allowedMove = move;
        float movementDistance = moveSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(move.x) > 0.01f && HitsWall(Vector3.right * Mathf.Sign(move.x), movementDistance + collisionPadding))
        {
            allowedMove.x = 0f;
        }

        if (Mathf.Abs(move.z) > 0.01f && HitsWall(Vector3.forward * Mathf.Sign(move.z), movementDistance + collisionPadding))
        {
            allowedMove.z = 0f;
        }

        return allowedMove;
    }

    private bool HitsWall(Vector3 direction, float distance)
    {
        RaycastHit[] hits = rb.SweepTestAll(direction, distance, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, maxRayLength);
    }
}