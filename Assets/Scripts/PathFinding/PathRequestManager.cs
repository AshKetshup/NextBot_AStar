using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour {
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    private static PathRequestManager instance;
    private PathFinding pathfinding;

    private bool isProcessingPath;

    private void Awake() {
        instance = this;
        pathfinding = GetComponent<PathFinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext() {
        if (isProcessingPath || pathRequestQueue.Count <= 0)
            return;

        currentPathRequest = pathRequestQueue.Dequeue();
        isProcessingPath = true;
        pathfinding.StartFindingPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
}
