using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public Transform target;

    public float speed = 6f;
    Vector3[] path;
    int targetIndex;

    private void Update() {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if (!pathSuccessful)
            return;

        path = newPath;
        targetIndex = 0;
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    private IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];
        while (true) {
            if (transform.position == currentWaypoint) {
                targetIndex++;

                if (targetIndex >= path.Length)
                    yield break;

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos() {
        if (path == null)
            return;

        for (int i = targetIndex; i < path.Length; i++) {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(path[i], Vector3.one);
            Gizmos.DrawLine(
                ( i == targetIndex ) ? transform.position : path[i - 1],
                path[i]
            );
        }
    }
}
