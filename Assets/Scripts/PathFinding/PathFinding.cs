using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PathFinding : MonoBehaviour
{
    public enum MODE { useHeap, useList, useBFS, useDFS, useDijkstra };
    public MODE mode;

    private PathRequestManager requestManager;
    private Grid grid;

    public void StartFindingPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(mode switch
        {
            MODE.useList => FindPathUsingList(startPos, targetPos),
            MODE.useHeap => FindPathUsingHeap(startPos, targetPos),
            MODE.useBFS => BFS(startPos, targetPos),
            MODE.useDFS => DFS(startPos, targetPos),
            MODE.useDijkstra => Dijkstra(startPos, targetPos),
            _ => FindPathUsingHeap(startPos, targetPos)
        });
    }

    private IEnumerator FindPathUsingList(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    bool condition =
                        (openSet[i].fCost < currentNode.fCost) ||
                        (openSet[i].fCost == currentNode.fCost) &&
                        (openSet[i].hCost < currentNode.hCost)
                    ;

                    if (condition)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    float newToNeigbourCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newToNeigbourCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newToNeigbourCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private IEnumerator BFS(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                // for (int i = 1; i < openSet.Count; i++) {
                //     bool condition =
                //         (openSet[i].fCost  < currentNode.fCost) ||
                //         (openSet[i].fCost == currentNode.fCost) &&
                //         (openSet[i].hCost  < currentNode.hCost)
                //     ;

                //     if (condition) {
                //         currentNode = openSet[i];
                //     }
                // }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    float newToNeigbourCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newToNeigbourCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newToNeigbourCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }


    private IEnumerator FindPathUsingHeap(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    float newToNeigbourCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newToNeigbourCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newToNeigbourCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private IEnumerator DFS(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Stack<Node> openSet = new Stack<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Push(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.Pop();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    float newToNeigbourCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newToNeigbourCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newToNeigbourCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Push(neighbour);

                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }


    private IEnumerator Dijkstra(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            List<Node> openSet = new List<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            Dictionary<Node, float> distances = new Dictionary<Node, float>();

            distances[startNode] = 0;
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                foreach (Node node in openSet)
                {
                    if (distances[node] < distances[currentNode])
                    {
                        currentNode = node;
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    float newCost = distances[currentNode] + GetDistance(currentNode, neighbour);
                    if (!distances.ContainsKey(neighbour) || newCost < distances[neighbour])
                    {
                        distances[neighbour] = newCost;
                        neighbour.parent = currentNode;
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                    
                }
            }
        }

        yield return null;
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }



    private IEnumerator Dijkstramau(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            Dictionary<Node, float> distances = new Dictionary<Node, float>();
            Dictionary<Node, Node> parents = new Dictionary<Node, Node>();

            distances[startNode] = 0;
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                foreach (Node node in openSet)
                {
                    if (distances[node] < distances[currentNode])
                    {
                        currentNode = node;
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    float newCost = distances[currentNode] + GetDistance(currentNode, neighbour);
                    if (!distances.ContainsKey(neighbour) || newCost < distances[neighbour])
                    {
                        distances[neighbour] = newCost;
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        yield return null;

        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }
    private Node FindNodeWithLowestDistance(List<Node> nodes, Dictionary<Node, float> distances)
    {
        Node minNode = nodes[0];
        float minDist = distances[minNode];

        foreach (Node node in nodes)
        {
            if (distances.ContainsKey(node) && distances[node] < minDist)
            {
                minNode = node;
                minDist = distances[node];
            }
        }

        return minNode;
    }


    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = path[i - 1].gridPosition - path[i].gridPosition;
            if (directionNew != directionOld)
                waypoints.Add(path[i].worldPosition);
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    private float GetDistance(Node startNode, Node endNode)
    {
        float distanceX = Mathf.Abs(startNode.gridPosition.x - endNode.gridPosition.x);
        float distanceY = Mathf.Abs(startNode.gridPosition.y - endNode.gridPosition.y);

        Func<float, float, float, float, float> distance =
            (minor, major, costDiag, costDirect) => costDiag * minor + costDirect * (major - minor);

        return distance(Mathf.Min(distanceX, distanceY), Mathf.Max(distanceX, distanceY), Node.diagonalCost, Node.directCost);
    }


    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

}
