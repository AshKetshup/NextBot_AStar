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


    private void MovePlayer() {
        moveDirection = orientation.forward * input.y + orientation.right * input.x;

        float multiplier = ((grounded) ? 1f : airMultiplier);
        rigidBody.AddForce(moveDirection.normalized * moveSpeed * 10f * multiplier, ForceMode.Force);
    }

    private void GetInput() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(jumpKey) && jumpReady && grounded) {
            Debug.Log("JUMP!");
            jumpReady = false;
            Jump();
            Invoke(nameof(JumpReset), jumpCooldown);
        } /* else
            Debug.Log(string.Format("Input.GetKey(jumpKey) = {0}, jumpReady = {1}, grounded = {2}", Input.GetKey(jumpKey), jumpReady, grounded)); */
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
    }

    private void FixedUpdate() => MovePlayer();

    // Update is called once per frame
    private void Update() {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        GetInput();
        ControlSpeed();

        // Handle dragging
        rigidBody.drag = ( grounded )
            ? groundDrag
            : 0f;
    }
}
