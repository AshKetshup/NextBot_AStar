using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour {
    [Header("References")]
    public  Transform playerObj;
    private new Rigidbody rigidbody;
    private PlayerMovement pm;

    [Header("Sliding")]
    public  float maxSlideTime;
    public  float slideForce;
    private float slideTimer;

    public  float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftAlt;
    private Vector2 inputAxis;

    private bool sliding;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update() {
        inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(slideKey) && ( inputAxis.x != 0 || inputAxis.y != 0 ))
            StartSlide();

        if (Input.GetKeyUp(slideKey) && sliding)
            StopSlide();
    }

    private void FixedUpdate() {
        if (sliding)
            SlidingMovement();
    }

    private void StartSlide() {
        sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement() {
        Vector3 inputDirection = playerObj.right * inputAxis.x + playerObj.forward * inputAxis.y;
        rigidbody.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide() {
        sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
