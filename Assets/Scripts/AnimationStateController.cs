using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour {
    public KeyCode sprintKey = KeyCode.LeftControl;
    public Transform playerOrientation;

    private Animator animator;
    private int isWalkingHash, isRunningHash;
    private Vector2 inputAxis;

    // Start is called before the first frame update
    private void Start() {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    private void Update() {
        transform.position = new(playerOrientation.position.x, transform.position.y, playerOrientation.position.z);
        transform.rotation = playerOrientation.rotation;

        inputAxis = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        // Input
        bool isInputAxis = ( inputAxis.magnitude > 0f );
        bool isRunKeyPressed = Input.GetKey(sprintKey);

        if (isWalking != isInputAxis)
            animator.SetBool(isWalkingHash, isInputAxis);

        if (isRunning != isRunKeyPressed)
            animator.SetBool(isRunningHash, isRunKeyPressed);
    }
}
