using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public float speed;
    public float rotationSpeed = 2500;
    private int rotateInput = 0;

    private PlayerInput controls;
    private Vector2 moveInput;
    private Rigidbody rb;

    public float maxRotationSpeed = 2500*2; // Max degrees per second
    public float accelerationRate = 50f; // Speed gain per second
    public float decelerationRate = 100f; // Speed loss per second

    private float rotationInput = 0f;
    private float currentSpeed = 0f;

    [SerializeField] private bool moveForward = false;

    [SerializeField] private AnimationClip ani;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        // move the plane forward at a constant rate
        if(moveForward)
            transform.Translate(Vector3.forward * speed);

        float rotationInput = moveInput.x;

        Debug.Log("rotationInput"+rotationInput);

        if(rotationInput != 0.0f)
		{
            rb.angularVelocity = new Vector3(rotationSpeed * rotationInput, 0, 0);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

    }
}
