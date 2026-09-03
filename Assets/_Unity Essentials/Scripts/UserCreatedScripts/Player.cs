using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] float maxRayLength = 0.2f; // Adjust based on character size

    private PlayerInput controls;
    private Vector2 moveInput;
    private Rigidbody2D rb; // get ref from editor is better, but lazy solution here

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
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0) * moveSpeed;

        // Rotate towards movement direction
        if (moveInput.sqrMagnitude > 0.01f) // Prevents rotation when not moving
        {
            Vector2 moveDirection = move.normalized; // Ensure it's a unit vector
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg; // Convert to degrees

            // Smoothly interpolate towards target angle
            float newRotation = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);

            rb.rotation = newRotation; // Apply rotation to Rigidbody2D
        }
        else
		{
            rb.angularVelocity = 0;
        }

        // translate
        if (move.x != 0 | move.y != 0)
        {
            rb.linearVelocity = new Vector2(move.x, move.y);
        }
		else
		{
            rb.linearVelocity = new Vector2(0, 0);
        }
    }
}