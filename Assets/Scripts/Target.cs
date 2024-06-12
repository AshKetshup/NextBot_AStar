using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour {
    [Header("References")]
    public  Transform playerObj;
    private new Rigidbody rigidbody;
    private PlayerMovement pm;

    [Header("Audio Source/Clips")]
    public AudioSource audioSource;

    [Header("Miscellanious")]
    public float range;
    public LayerMask whatIsFloor;
    private RaycastHit hit;

    // Start is called before the first frame update
    private void Start() { }

    // Update is called once per frame
    private void Update() {
        //audioSource.pitch = Random.Range(.8f, 1f);
        Footstep();

    }

    public void Footstep() {
        Vector2 inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool difference = (inputAxis.x != 0 || inputAxis.y != 0);
        bool grounded = Physics.Raycast(playerObj.position, playerObj.transform.up * -1, out hit, range, whatIsFloor);

        if (grounded && difference) {
            audioSource.enabled = true;
            return;
        }

        audioSource.enabled = false;
    }
}
