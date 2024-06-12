using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationHunter : MonoBehaviour {
    public Transform orientation;

    // Update is called once per frame
    void Update()
        => transform.forward = ( orientation.position - transform.position ).normalized;
}
