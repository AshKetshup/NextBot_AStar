using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {
    public bool walkable;
    public Vector3 worldPosition;
    public Vector2Int gridPosition;

    public Node parent;
    public int heapIndex { get; set; }

    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;


    // Manhattan Cost
    public static float diagonalCost = 1.4142135624f;
    public static float directCost = 1f;

    public Node(bool _walkable, Vector3 _worldPosition, Vector2Int _gridPosition) {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridPosition = _gridPosition;
    }

    public int CompareTo(Node other) {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(other.hCost);

        return -compare;
    }
}
