using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public enum MovementState {
        walking,
        sprinting,
        crouching,
        air
    };
    [Header("Movement")]
    public MovementState state;

    [Header("Walking/Sprinting")]
    public float sprintSpeed;
    public float walkSpeed;
    public float groundDrag;
    private bool grounded;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool jumpReady;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftControl;
    public KeyCode crouchKey = KeyCode.LeftShift;

    [Header("Check Ground")]
    public float playerHeight;
    public LayerMask whatIsGround;

    public Transform orientation;

    private Vector2 input;
    private Vector3 moveDirection;
    private Rigidbody rigidBody;
    private float moveSpeed;

    private void StateHandler() {
        if (Input.GetKey(crouchKey)) {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        } else if (grounded && Input.GetKey(sprintKey)) {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        } else if (grounded) {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        } else {
            state = MovementState.air;
        }
    }

    private bool OnSlope() {
        bool rayCast = Physics.Raycast(
            transform.position,
            Vector3.down,
            out slopeHit,
            playerHeight * 0.5f
        );

        if (rayCast) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return ( angle < maxSlopeAngle && angle != 0 );
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
        => Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;

    private void MovePlayer() {
        moveDirection = orientation.forward * input.y + orientation.right * input.x;

        if (OnSlope()) {
            rigidBody.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        } else if (grounded) {
            rigidBody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        } else if (!grounded) {
            rigidBody.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void GetInput() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(jumpKey) && jumpReady && grounded) {
            jumpReady = false;
            Jump();
            Invoke(nameof(JumpReset), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rigidBody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void ControlSpeed() {
        Vector3 flatVelocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        if (flatVelocity.magnitude > moveSpeed) {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rigidBody.velocity = new Vector3(limitedVelocity.x, rigidBody.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump() {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpReset() => jumpReady = true;


    // Start is called before the first frame update
    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
        jumpReady = true;

        startYScale = transform.localScale.y;
    }

    private void FixedUpdate() => MovePlayer();

    // Update is called once per frame
    private void Update() {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, whatIsGround);
        GetInput();
        ControlSpeed();
        StateHandler();

        // Handle dragging
        rigidBody.drag = ( grounded )
            ? groundDrag
            : 0f;
    }
}
