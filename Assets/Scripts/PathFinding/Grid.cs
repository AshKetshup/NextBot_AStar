using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Grid : MonoBehaviour {

    public LayerMask whatIsUnwalkable;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    public List<Node> path;

    private Node[,] grid;
    private float nodeDiameter;
    private Vector2Int gridSize;

    private void gridCreate() {
        grid = new Node[gridSize.x, gridSize.y];
        Vector3 worldBottomLeft =
            transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y / 2);

        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++) {
                Vector3 worldPoint =
                    worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool isWalkable = !(Physics.CheckSphere(worldPoint, nodeRadius, whatIsUnwalkable));
                grid[x, y] = new Node(isWalkable, worldPoint, new Vector2Int(x, y));
            }
    }

    public List<Node> getNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                Vector2Int check = node.gridPosition + new Vector2Int(x, y);
                if (check.x >= 0 && check.x < gridSize.x && check.y >= 0 && check.y < gridSize.y)
                    neighbours.Add(grid[check.x, check.y]);
            }
        }

        return neighbours;
    }

    public Node nodeFromWorldPoint(Vector3 worldPosition) {
        Vector2 percent = new Vector2(
            Mathf.Clamp01((worldPosition.x + gridWorldSize.x/2) / gridWorldSize.y),
            Mathf.Clamp01((worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y)
        );

        int posX = Mathf.RoundToInt(( gridSize.x - 1 ) * percent.x);
        int posY = Mathf.RoundToInt(( gridSize.y - 1 ) * percent.y);


        return grid[posX, posY];
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null) {
            foreach (Node node in grid) {
                Gizmos.color =
                ( node.walkable ) ? Color.white :
                Color.red;

                if (path != null && path.Contains(node))
                    Gizmos.color = Color.black;


                Gizmos.DrawCube(node.worldPosition, Vector3.one * ( nodeDiameter + .1f ));
            }
        }
    }


    private void Start() {
        nodeDiameter = nodeRadius * 2;
        gridSize.x = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSize.y = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridCreate();
    }
}
