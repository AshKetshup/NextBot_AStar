using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool jumpReady;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Check Ground")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    private Vector2 input;
    private Vector3 moveDirection;
    private Rigidbody rigidBody;


    private void movePlayer() {
        moveDirection = orientation.forward * input.y + orientation.right * input.x;

        float multiplier = ((grounded) ? 1f : airMultiplier);
        rigidBody.AddForce(moveDirection.normalized * moveSpeed * 10f * multiplier, ForceMode.Force);
    }

    private void getInput() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(jumpKey) && jumpReady && grounded) {
            Console.WriteLine("JUMP!");
            jumpReady = false;
            jump();
            Invoke(nameof(jumpReset), jumpCooldown);
        }
    }

    private void controlSpeed() {
        Vector3 flatVelocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        if (flatVelocity.magnitude > moveSpeed) {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rigidBody.velocity = new Vector3(limitedVelocity.x, rigidBody.velocity.y, limitedVelocity.z);
        }
    }

    private void jump() {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void jumpReset() {
        jumpReady = true;
    }

    // Start is called before the first frame update
    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
        jumpReady = true;
    }


    private void FixedUpdate() {
        movePlayer();
    }

    // Update is called once per frame
    private void Update() {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        getInput();
        controlSpeed();

        // Handle dragging
        rigidBody.drag = ( grounded )
            ? groundDrag
            : 0f;
    }
}
