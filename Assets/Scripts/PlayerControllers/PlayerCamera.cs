using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    public Vector2 sensitivity;
    public Transform orientation;

    private Vector2 rotation;

    // Start is called before the first frame update
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update() {
        Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity * Time.deltaTime;
        rotation += new Vector2(-mouse.y, mouse.x);
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        orientation.rotation = Quaternion.Euler(0, rotation.y, 0);
    }
}
