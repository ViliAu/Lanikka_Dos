using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    /* User data */
    public float speed = 15;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float crouchSpeed = 13f;

    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float airAcceleration = 1f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float gravity = 5f;
    [SerializeField] private float maximumGravity = 120f;
    [SerializeField] private float jumpHeight = 15f;

    [Header("Ground mask")]
    [SerializeField] private LayerMask groundMask = default;

    public bool IsGrounded {get; private set;}

    private float controllerHeight = 2f;

    /* Memory data */
    [HideInInspector] public Vector3 velocity;
    private CharacterController controller;
    private Transform head;

    /* Initialize vars */
    private void Start() {
        controller = transform.GetComponent<CharacterController>();
        velocity = Vector3.zero;
        head = transform.Find("Head");
    }

    private void Update() {
        GroundCheck();
        Acceleration();
        Deceleration();
        ApplyGravity();
        Jump();
        Crouch();
        ApplyVelocity();

        if (transform.position.y < -100)
            transform.position = Vector3.zero;
    }

    /* Creates a velocity vector from given input and current speed */ 
    private void Acceleration() {
        Vector3 dir = transform.rotation * EntityManager.Player.Player_Input.input;

        if (!IsGrounded) {
            AirAcceleration(dir);
            return;
        }
        // Check the correct speed (crouch, sprint or normal)
        // Crouching
        if (EntityManager.Player.Player_Input.crouched) {
            velocity.x = Mathf.Lerp(velocity.x, crouchSpeed * dir.x, acceleration * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, crouchSpeed * dir.z, acceleration * Time.deltaTime);
        }
        // Sprinting
        else if (EntityManager.Player.Player_Input.sprinting) {
            velocity.x = Mathf.Lerp(velocity.x, sprintSpeed * dir.x, acceleration * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, sprintSpeed * dir.z, acceleration * Time.deltaTime);
        }
        // Walking
        else {
            velocity.x = Mathf.Lerp(velocity.x, speed * dir.x, acceleration * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, speed * dir.z, acceleration * Time.deltaTime);
        }
    }

    private void AirAcceleration(Vector3 dir) {
        if (EntityManager.Player.Player_Input.input == Vector3.zero) {
            return;
        }
        if (velocity.magnitude > speed) {
            if ((velocity + dir).magnitude > velocity.magnitude) {
                return;
            }
        }
        velocity.x = Mathf.Lerp(velocity.x, speed * dir.x, airAcceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, speed * dir.z, airAcceleration * Time.deltaTime);
    }

    /* Decelerates player */
    private void Deceleration() {
        if (!IsGrounded) {
            return;
        }
        velocity.x = Mathf.Lerp(velocity.x, 0, deceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, deceleration * Time.deltaTime);
    }

    /* Handles gravity */
    private void ApplyGravity() {
        if (IsGrounded) {
            if (velocity.y < 0)
                velocity.y = 0;
        }
        else {
            velocity.y = Mathf.Clamp(velocity.y - gravity * Time.deltaTime, -maximumGravity, 1000);
        }
    }

    /* Handles jumping */
    private void Jump() {
        if (EntityManager.Player.Player_Input.jumped && IsGrounded)
            velocity.y = jumpHeight;
    }

    private void Crouch() {
        if (head == null) {
            Debug.LogError(" No player head found...");
            return;
        }
        if (EntityManager.Player.Player_Input.crouched) {
            controllerHeight = Mathf.Lerp(controller.height, 1, 5 * Time.deltaTime);
            controller.height = controllerHeight;
        }
        else {
            controllerHeight = Mathf.Lerp(controller.height, 2, 5 * Time.deltaTime);
            controller.height = controllerHeight;
            
            if (controllerHeight < 1.95f) {
                transform.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
            }
        }
    }

    /* Applies velocity vector to rigidbody (moves the player) */
    private void ApplyVelocity() {
        controller.Move(velocity * Time.deltaTime);
    }

    //Ground check
    private void GroundCheck() {                                                   
        RaycastHit hit;
        float cMod = Mathf.Abs((2-controllerHeight)*0.5f);
        Vector3 radius = new Vector3(0,controller.radius,0);
        Vector3 upperPos = transform.position + controller.center + radius;
        Vector3 lowerPos = transform.position + radius + Vector3.up * cMod;
        Debug.DrawLine(Vector3.zero, lowerPos);

        //Check if we hit something
        IsGrounded = Physics.CapsuleCast(upperPos, lowerPos, controller.radius, -Vector3.up, out hit, 0.085f, groundMask, QueryTriggerInteraction.Ignore);
    }
}