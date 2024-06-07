using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.Windows;
using System;

public class PathFinding : MonoBehaviour {

    public Transform playerOrientation;
    public Transform hunterOrientation;

    private Grid grid;

    void findPath(Vector3 startPos, Vector3 targetPos) {
        Node startNode = grid.nodeFromWorldPoint(startPos);
        Node targetNode = grid.nodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                bool condition =
                    (openSet[i].fCost  < currentNode.fCost) ||
                    (openSet[i].fCost == currentNode.fCost) &&
                    (openSet[i].hCost  < currentNode.hCost)
                ;

                if (condition) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                retracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.getNeighbours(currentNode)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                float newToNeigbourCost = currentNode.gCost + getDistance(currentNode, neighbour);
                if (newToNeigbourCost < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newToNeigbourCost;
                    neighbour.hCost = getDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void retracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    float getDistance(Node startNode, Node endNode) {
        float distanceX = Mathf.Abs(startNode.gridPosition.x - endNode.gridPosition.x);
        float distanceY = Mathf.Abs(startNode.gridPosition.y - endNode.gridPosition.y);

        Func<float, float, float, float, float> distance =
            (minor, major, costDiag, costDirect) =>  costDiag * minor + costDirect * ( major - minor );

        return distance(Mathf.Min(distanceX, distanceY), Mathf.Max(distanceX, distanceY), Node.diagonalCost, Node.directCost);
    }
    private void Awake() {
        grid = GetComponent<Grid>();
    }

    private void Update() {
        findPath(hunterOrientation.position, playerOrientation.position);
    }
}
