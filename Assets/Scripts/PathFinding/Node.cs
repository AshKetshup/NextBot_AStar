using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public bool walkable;
    public Vector3 worldPosition;
    public Vector2Int gridPosition;

    // Manhattan Cost
    public static float diagonalCost = 1.4142135624f;
    public static float directCost = 1f;

    public float gCost;
    public float hCost;

    public Node parent;

    public Node(bool _walkable, Vector3 _worldPosition, Vector2Int _gridPosition) {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridPosition = _gridPosition;
    }

    public Node(bool _walkable, Vector3 _worldPosition, int _gridPosX, int _gridPosY) {
        new Node(_walkable, _worldPosition, new Vector2Int(_gridPosX, _gridPosY));
    }

    public float fCost {
        get { return gCost + hCost; }
    }
}
