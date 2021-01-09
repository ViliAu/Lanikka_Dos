﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    /* User data */
    [Header("Movement speeds")]
    public float speed = 15;
    [SerializeField] private float sprintSpeed = 30f;
    [SerializeField] private float crouchSpeed = 13f;

    [Header("Ground movement")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 10f;

    [Header("Air stuff")]
    [SerializeField] private float airAcceleration = 1f;
    [SerializeField] private float gravity = 5f;
    [SerializeField] private float maximumGravity = 120f;
    [SerializeField] private float jumpHeight = 15f;
    [Header("Ladder")]
    [Tooltip("How far the palyer can grab a ladder")]
    [SerializeField] private float ladderWidth = 0.1f;
    [Tooltip("How fast the player climbs ladders")]
    [SerializeField] private float climbSpeed = 1f;
    [SerializeField] private float ladderMaxAngle = 10f;
    [SerializeField] private float ladderJumpForce = 10f;

    [Header("Ground mask")]
    [SerializeField] private LayerMask groundMask = default;

    public bool IsGrounded {get; private set;}
    public bool CanUncrouch {get; private set;}
    public bool IsLaddered {get; private set;}

    private float controllerHeight = 2f;
    private float ladderLookModifier = 1f;

    /* Memory data */
    [HideInInspector] public Vector3 velocity;
    private CharacterController controller;
    private Transform head;
    private Collider[] ldrCols = null;
    private Transform ladder;

    /* Initialize vars */
    private void Start() {
        controller = transform.GetComponent<CharacterController>();
        velocity = Vector3.zero;
        head = transform.Find("Head");
    }

    private void Update() {
        StateCheck();
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
        Vector3 dir = EntityManager.Player.Player_Input.input;
        // Rotate input
        dir = transform.rotation * EntityManager.Player.Player_Input.input;
        if (!IsGrounded && !IsLaddered) {
            AirAcceleration(dir);
            return;
        }

        // Laddered
        if (IsLaddered) {
            ladderLookModifier = Vector3.SignedAngle(EntityManager.Player.Player_Camera.head.forward, transform.forward, transform.right) / ladderMaxAngle;
            ladderLookModifier = DUtil.Clamp1Neg1(ladderLookModifier);
            ladderLookModifier = Vector3.Angle(ladder.forward, transform.forward) > 45 ? ladderLookModifier *= -1 : ladderLookModifier;
            dir = LadderClimb(dir);
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

    private Vector3 LadderClimb(Vector3 dir) {
        float vel = Vector3.Dot(dir, ladder.forward);
        velocity.y = vel * climbSpeed * ladderLookModifier;
        dir *= (1-vel);
        return dir;
    }

    /* Decelerates player */
    private void Deceleration() {
        if (!IsGrounded && !IsLaddered) {
            return;
        }
        velocity.x = Mathf.Lerp(velocity.x, 0, deceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, deceleration * Time.deltaTime);
    }

    /* Handles gravity */
    private void ApplyGravity() {
        if (!IsLaddered) {
            if (IsGrounded) {
                if (velocity.y < -gravity)
                    velocity.y = -gravity;
            }
            else {
                velocity.y = Mathf.Clamp(velocity.y - gravity * Time.deltaTime, -maximumGravity, 1000);
                if (velocity.y > 0 && !CanUncrouch)
                    velocity.y = 0;
            }
        }
    }

    /* Handles jumping */
    private void Jump() {
        if (EntityManager.Player.Player_Input.jumped) {
            if (IsGrounded) {
                velocity.y = jumpHeight;
            }
            else if (IsLaddered) {
                velocity.y = jumpHeight;
                velocity += -ladder.forward * ladderJumpForce;
                IsLaddered = false;
            }
        }            
    }

    private void Crouch() {
        if (head == null) {
            Debug.LogError(" No player head found...");
            return;
        }
        // Crouching
        if (EntityManager.Player.Player_Input.crouched) {
            controllerHeight = Mathf.Lerp(controller.height, 1, 5 * Time.deltaTime);
            controller.height = controllerHeight;
        }
        // Uncrouching
        else if (CanUncrouch) {
            controllerHeight = Mathf.Lerp(controller.height, 2, 5 * Time.deltaTime);
            controller.height = controllerHeight;
            // Smoothing.. maybe rework
            if (controllerHeight < 1.9f) {
                controller.Move(Vector3.up * controllerHeight * Time.deltaTime);
            }
        }
    }

    /* Applies velocity vector to char controller (moves the player) */
    private void ApplyVelocity() {
        controller.Move(velocity * Time.deltaTime);
    }

    //Ground check
    private void StateCheck() {
        RaycastHit hit;
        float cMod = Mathf.Abs(2-controllerHeight) * 0.5f;
        Vector3 radius = new Vector3(0,controller.radius,0);
        Vector3 upperPos = transform.position + Vector3.up * (controller.height + cMod) - radius;
        Vector3 lowerPos = transform.position + radius + Vector3.up * cMod;

        // Check if we hit a ladder
        ldrCols = Physics.OverlapCapsule(lowerPos + Vector3.up * ladderWidth, upperPos, controller.radius + ladderWidth, LayerMask.GetMask("Ladder"), QueryTriggerInteraction.Collide);
        IsLaddered = ldrCols.Length != 0;
        if (IsLaddered)
            ladder = ldrCols[0].transform;

        //Check we're grounded
        IsGrounded = Physics.CapsuleCast(upperPos, lowerPos, controller.radius, -Vector3.up, out hit, controller.skinWidth + 0.005f, groundMask, QueryTriggerInteraction.Ignore);

        // Check if we can uncrouch
        float crouchCeiling = IsGrounded ? 2 * cMod : 0;
        CanUncrouch = !Physics.CapsuleCast(lowerPos, upperPos, controller.radius, Vector3.up, out hit, crouchCeiling + controller.skinWidth + 0.005f, groundMask, QueryTriggerInteraction.Ignore);
        
    }
}